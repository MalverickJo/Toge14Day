using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("References")]
    public BattleSpawner spawner;
    public BattleUI battleUI;

    private List<BattleUnit> allUnits = new List<BattleUnit>();
    private List<BattleUnit> turnOrder = new List<BattleUnit>();

    private BattleUnit currentUnit;
    private int turnIndex = 0;
    public List<BattleUnit> GetAllUnits() => allUnits;
    public enum BattleState { Start, PlayerTurn, EnemyTurn, Win, Lose }
    public BattleState state;

    [Header("Arrow Indicator")]
    public GameObject arrowPrefab;

    [Header("HP Bar")]
    public GameObject worldHPBarPrefab;

    private void Awake()
    {
        Instance = this;
        Character.arrowPrefab = arrowPrefab;
        Character.worldHPBarPrefab = worldHPBarPrefab;
        Enemy.worldHPBarPrefab = worldHPBarPrefab;
    }
    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        state = BattleState.Start;
        yield return new WaitForSeconds(0.5f);

        if (spawner == null) { Debug.LogError("spawner null!"); yield break; }
        if (battleUI == null) { Debug.LogError("battleUI null!"); yield break; }

        allUnits.Clear();
        foreach (var obj in spawner.partyObjects)
        {
            Character c = obj.GetComponent<Character>();
            if (c != null) allUnits.Add(BattleUnit.FromCharacter(c, obj));
        }
        foreach (var obj in spawner.enemyObjects)
        {
            Enemy e = obj.GetComponent<Enemy>();
            if (e != null) allUnits.Add(BattleUnit.FromEnemy(e, obj));
        }

        allUnits.Sort((a, b) => b.speed.CompareTo(a.speed));
        turnOrder = new List<BattleUnit>(allUnits);

        battleUI.SetupHPBars(allUnits);
        battleUI.UpdateAllHPBars(allUnits);

        NextTurn();
    }

    void NextTurn()
    {
        foreach (var unit in allUnits)
        {
            if (unit.characterRef != null)
                unit.characterRef.SetActiveIndicator(false);
            if (unit.enemyRef != null && unit.gameObject != null)
                unit.enemyRef.SetActiveIndicator(false);
            battleUI.SetActivePanel(unit, false);
        }

        while (turnOrder[turnIndex].isDead || turnOrder[turnIndex].gameObject == null)
            turnIndex = (turnIndex + 1) % turnOrder.Count;

        currentUnit = turnOrder[turnIndex];

        if (currentUnit.gameObject != null)
        {
            currentUnit.characterRef?.SetActiveIndicator(true);
            if (currentUnit.enemyRef != null && currentUnit.gameObject != null)
                currentUnit.enemyRef.SetActiveIndicator(true);
        }

        battleUI.SetActivePanel(currentUnit, true);

        if (currentUnit.isEnemy)
        {
            state = BattleState.EnemyTurn;
            battleUI.HidePlayerActions();
            StartCoroutine(EnemyTurn());
        }
        else
        {
            state = BattleState.PlayerTurn;
            battleUI.ShowPlayerActions(currentUnit);
        }
    }

    // ─── Player Actions ────────────────────────────────

    public void PlayerAttack(BattleUnit target)
    {
        if (state != BattleState.PlayerTurn) return;

        int damage = Mathf.RoundToInt(currentUnit.attack * 1.0f);
        target.TakeDamage(damage);

        currentUnit.characterRef?.SetAttackAnimation();

        battleUI.ShowDamageText(target.gameObject, damage);
        battleUI.UpdateAllHPBars(allUnits);

        StartCoroutine(EndPlayerTurn());
    }

    public void PlayerSkill(BattleUnit target, SkillData skill)
    {
        if (state != BattleState.PlayerTurn) return;

        if (currentUnit.currentMP < skill.manaCost)
        {
            battleUI.ShowMessage("Out of Mana!");
            battleUI.ShowPlayerActions(currentUnit);
            return;
        }

        currentUnit.currentMP -= skill.manaCost;
        currentUnit.characterRef?.SpendMP(skill.manaCost);

        int damage = Mathf.RoundToInt(
            (currentUnit.attack * skill.attackScaling) +
            (currentUnit.magicAttack * skill.magicScaling)
        );

        List<BattleUnit> targets = GetSkillTargets(target, skill.targetType);

        if (targets.Count == 0)
        {
            battleUI.ShowPlayerActions(currentUnit);
            return;
        }

        foreach (var t in targets)
        {
            if (skill.skillType == SkillType.Heal)
                t.Heal(damage);
            else
                t.TakeDamage(damage);

            battleUI.ShowDamageText(t.gameObject, damage);

            foreach (var effect in skill.effects)
            {
                if (t.enemyRef != null) t.enemyRef.ApplyEffect(effect);
                if (t.characterRef != null) t.characterRef.ApplyEffect(effect);
            }
        }

        currentUnit.characterRef?.SetAttackAnimation();
        battleUI.UpdateAllHPBars(allUnits);
        StartCoroutine(EndPlayerTurn());
    }

    List<BattleUnit> GetSkillTargets(BattleUnit mainTarget, TargetType type)
    {
        List<BattleUnit> targets = new List<BattleUnit>();
        List<BattleUnit> enemies = allUnits.FindAll(u => u.isEnemy && !u.isDead);
        List<BattleUnit> allies = allUnits.FindAll(u => !u.isEnemy && !u.isDead);

        switch (type)
        {
            case TargetType.SingleTarget:
                if (mainTarget != null) targets.Add(mainTarget);
                break;

            case TargetType.TwoTargets:
                if (mainTarget != null)
                {
                    targets.Add(mainTarget);
                    int idx = enemies.IndexOf(mainTarget);
                    if (idx + 1 < enemies.Count) targets.Add(enemies[idx + 1]);
                }
                break;

            case TargetType.ThreeTargets:
                if (mainTarget != null)
                {
                    targets.Add(mainTarget);
                    int idx2 = enemies.IndexOf(mainTarget);
                    if (idx2 + 1 < enemies.Count) targets.Add(enemies[idx2 + 1]);
                    if (idx2 + 2 < enemies.Count) targets.Add(enemies[idx2 + 2]);
                }
                break;

            case TargetType.AllEnemies:
                targets.AddRange(enemies);
                break;

            case TargetType.Self:
                targets.Add(currentUnit);
                break;

            case TargetType.AllAllies:
                targets.AddRange(allies);
                break;
        }
        return targets;
    }

    IEnumerator EndPlayerTurn()
    {
        yield return new WaitForSeconds(0.8f);

        CheckBattleEnd();
        if (state == BattleState.Win || state == BattleState.Lose) yield break;
        currentUnit.characterRef?.TickEffects();

        turnIndex = (turnIndex + 1) % turnOrder.Count;
        NextTurn();
    }

    // ─── Enemy AI ──────────────────────────────────────

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        List<BattleUnit> aliveAllies = allUnits.FindAll(u => !u.isEnemy && !u.isDead);
        BattleUnit target = aliveAllies.Find(u => u.characterRef != null && u.characterRef.HasTaunt());
        if (target == null)
            target = aliveAllies[Random.Range(0, aliveAllies.Count)];

        SkillData skill = currentUnit.enemyRef?.GetRandomSkill();
        if (skill != null && Random.value > 0.4f)
        {
            List<BattleUnit> targets = GetSkillTargets(target, skill.targetType);
            int damage = Mathf.RoundToInt(
                (currentUnit.attack * skill.attackScaling) +
                (currentUnit.magicAttack * skill.magicScaling)
            );
            foreach (var t in targets)
            {
                t.TakeDamage(damage);
                battleUI.ShowDamageText(t.gameObject, damage);
            }
        }
        else
        {
            int damage = Mathf.RoundToInt(currentUnit.attack * 1.0f);
            target.TakeDamage(damage);
            battleUI.ShowDamageText(target.gameObject, damage);
        }

        battleUI.UpdateAllHPBars(allUnits);
        yield return new WaitForSeconds(0.8f);

        currentUnit.enemyRef?.TickEffects();
        CheckBattleEnd();
        if (state == BattleState.Win || state == BattleState.Lose) yield break;

        turnIndex = (turnIndex + 1) % turnOrder.Count;
        NextTurn();
    }

    // ─── Battle End ────────────────────────────────────

    void CheckBattleEnd()
    {
        bool allEnemiesDead = allUnits.TrueForAll(u => !u.isEnemy || u.isDead);
        bool allAlliesDead = allUnits.TrueForAll(u => u.isEnemy || u.isDead);

        if (allEnemiesDead)
        {
            state = BattleState.Win;
            StartCoroutine(BattleWin());
        }
        else if (allAlliesDead)
        {
            state = BattleState.Lose;
            StartCoroutine(BattleLose());
        }
    }

    IEnumerator BattleWin()
    {
        battleUI.ShowWinPanel();

        int totalEXP = 0;
        int totalGold = 0;
        foreach (var unit in allUnits)
        {
            if (unit.isEnemy && unit.isDead)
            {
                totalEXP += unit.enemyRef.RoleData.expReward;
                totalGold += unit.enemyRef.RoleData.goldReward;
            }
        }

        LevelData.Instance?.AddEXP(totalEXP);
        PartyData.Instance.gold += totalGold;

        yield return new WaitForSeconds(2f);

        PortraitCamera[] cameras = FindObjectsByType<PortraitCamera>(FindObjectsSortMode.None);
        foreach (var pc in cameras)
            Destroy(pc.gameObject);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    IEnumerator BattleLose()
    {
        battleUI.ShowLosePanel();
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
    }

    public void OnUnitDied(BattleUnit unit)
    {
        if (unit.gameObject != null)
        {
            if (unit.characterRef != null)
                unit.characterRef.SetDeathAnimation();
            StartCoroutine(DestroyAfterDelay(unit.gameObject, 1f));
        }

        battleUI.RemoveUnitPanel(unit);

        if (!unit.isEnemy && unit.characterRef != null)
        {
            CharacterRoleData deadRole = unit.characterRef.RoleData;
            GameObject deadOverride = unit.characterRef.GetOverridePrefab();
            PartyData.Instance.RemoveMember(deadRole, deadOverride);
            Debug.Log($"{deadRole.roleName} mati dan dihapus dari party!");
        }
    }

    IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null) Destroy(obj);
    }
}