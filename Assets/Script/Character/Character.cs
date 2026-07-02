using UnityEngine;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    [SerializeField] private CharacterRoleData roleData;
    public CharacterRoleData RoleData => roleData;

    public int currentHP { get; private set; }
    public int currentMP { get; private set; }

    public int currentAttack => Mathf.RoundToInt(roleData.attack * GetStatMultiplier(EffectType.StatBuff, EffectType.StatDebuff, "attack"));
    public int currentDefense => Mathf.RoundToInt(roleData.defense * GetStatMultiplier(EffectType.DamageReduction, EffectType.StatDebuff, "defense"));
    public int currentMagicAttack => Mathf.RoundToInt(roleData.magicAttack * GetStatMultiplier(EffectType.StatBuff, EffectType.StatDebuff, "magicAttack"));
    public int currentSpeed => Mathf.RoundToInt(roleData.speed * GetStatMultiplier(EffectType.StatBuff, EffectType.StatDebuff, "speed"));

    public bool IsDead => currentHP <= 0;

    private List<ActiveEffect> activeEffects = new List<ActiveEffect>();

    private GameObject visualInstance;
    private Animator animator;

    public GameObject GetOverridePrefab() => overrideVisualPrefab;
    private GameObject overrideVisualPrefab;
    private void Awake()
    {
        if (roleData != null)
            InitStats();
    }
    public static GameObject worldHPBarPrefab;
    private HPBar worldHPBar;

    public void SpawnWorldHPBar()
    {
        if (worldHPBarPrefab == null) return;

        GameObject canvasObj = new GameObject("HPBarCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = new Vector3(0, -0.8f, 0);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;

        RectTransform rt = canvasObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 20);
        rt.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        GameObject bar = Instantiate(worldHPBarPrefab, canvasObj.transform);
        RectTransform barRt = bar.GetComponent<RectTransform>();
        barRt.anchorMin = Vector2.zero;
        barRt.anchorMax = Vector2.one;
        barRt.offsetMin = Vector2.zero;
        barRt.offsetMax = Vector2.zero;

        worldHPBar = bar.GetComponent<HPBar>();
        worldHPBar?.Setup(roleData.roleName, currentHP, GetMaxHP());
    }

    public void UpdateWorldHPBar()
    {
        worldHPBar?.UpdateHP(currentHP, GetMaxHP());
    }
    private int currentLevel = 1;

    public int GetCurrentAttack() => roleData.attack + (roleData.attackGrowth * (currentLevel - 1));
    public int GetCurrentDefense() => roleData.defense + (roleData.defenseGrowth * (currentLevel - 1));
    public int GetCurrentMagicAttack() => roleData.magicAttack + (roleData.magicAttackGrowth * (currentLevel - 1));
    public int GetCurrentMagicDefense() => roleData.magicDefense + (roleData.magicDefenseGrowth * (currentLevel - 1));
    public int GetCurrentSpeed() => roleData.speed + (roleData.speedGrowth * (currentLevel - 1));
    public int GetMaxHP() => roleData.maxHP + (roleData.hpGrowth * (currentLevel - 1));
    public int GetMaxMP() => roleData.maxMP + (roleData.mpGrowth * (currentLevel - 1));

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, 20);
        currentHP = GetMaxHP();
        currentMP = GetMaxMP();
    }

    public void OnLevelUp(int newLevel)
    {
        int oldMaxHP = GetMaxHP();
        currentLevel = newLevel;

        int newMaxHP = GetMaxHP();
        currentHP = Mathf.Min(currentHP + (newMaxHP - oldMaxHP), newMaxHP);
        currentMP = GetMaxMP();

        Debug.Log($"{roleData.roleName} level up ke {currentLevel}! HP: {currentHP}/{GetMaxHP()}");
    }

    void InitStats()
    {
        if (LevelData.Instance != null)
            currentLevel = LevelData.Instance.partyLevel;

        currentHP = GetMaxHP();
        currentMP = GetMaxMP();
        activeEffects.Clear();
        SpawnVisual();
    }

    public void SetRole(CharacterRoleData data)
    {
        roleData = data;
        InitStats();
    }
    public static GameObject arrowPrefab;
    private GameObject activeArrow;

    public void SetActiveIndicator(bool active)
    {
        if (visualInstance == null) return;

        if (active)
        {
            if (activeArrow == null && arrowPrefab != null)
            {
                activeArrow = Instantiate(arrowPrefab, visualInstance.transform);
                activeArrow.transform.localPosition = new Vector3(0, 1.2f, 0);
            }
            if (activeArrow != null)
                activeArrow.SetActive(true);
        }
        else
        {
            if (activeArrow != null)
                activeArrow.SetActive(false);
        }
    }
    // ─── Status Effect System ───────────────────────────

    public void ApplyEffect(SkillEffect effect)
    {
        switch (effect.effectType)
        {
            case EffectType.Damage:
                TakeDamageRaw(Mathf.RoundToInt(effect.value));
                break;

            case EffectType.Heal:
                Heal(Mathf.RoundToInt(effect.value));
                break;

            case EffectType.StatBuff:
            case EffectType.StatDebuff:
            case EffectType.DamageReduction:
            case EffectType.ApplyTaunt:
                if (Random.Range(0f, 100f) <= effect.chanceToApply)
                {
                    activeEffects.Add(new ActiveEffect(effect));
                    if (effect.effectType == EffectType.StatDebuff)
                        SetDebuffAnimation(true);
                }
                break;
        }
    }

    public void TakeDamageRaw(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        SetDamagedAnimation();
        if (IsDead) SetDeathAnimation();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(roleData.maxHP, currentHP + amount);
    }

    public bool SpendMP(int amount)
    {
        if (currentMP < amount) return false;
        currentMP -= amount;
        return true;
    }

    public void RegenMP(int amount)
    {
        currentMP = Mathf.Min(GetMaxMP(), currentMP + amount);
    }


    public void TickEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].remainingTurns--;
            if (activeEffects[i].remainingTurns <= 0)
                activeEffects.RemoveAt(i);
        }

        bool hasDebuff = activeEffects.Exists(e =>
            e.effectType == EffectType.StatDebuff);
        SetDebuffAnimation(hasDebuff);
    }


    float GetDamageReduction()
    {
        float total = 0f;
        foreach (var e in activeEffects)
            if (e.effectType == EffectType.DamageReduction)
                total += e.value / 100f;
        return Mathf.Clamp(total, 0f, 0.75f);
    }

    float GetStatMultiplier(EffectType buffType, EffectType debuffType, string statName)
    {
        float multiplier = 1f;
        foreach (var e in activeEffects)
        {
            if (e.effectType == buffType && e.statTarget == statName)
                multiplier += e.value / 100f;
            else if (e.effectType == debuffType && e.statTarget == statName)
                multiplier -= e.value / 100f;
        }
        return Mathf.Max(0.1f, multiplier);
    }

    public bool HasTaunt() => activeEffects.Exists(e => e.effectType == EffectType.ApplyTaunt);

    // ─── Visual ─────────────────────────────────────────

    public void SetOverridePrefab(GameObject prefab)
    {
        overrideVisualPrefab = prefab;
    }


    void SpawnVisual()
    {
        if (roleData.characterPrefab == null && overrideVisualPrefab == null)
        {
            Debug.LogError("Prefab belum diisi!");
            return;
        }

        if (visualInstance != null)
            Destroy(visualInstance);
        GameObject prefabToSpawn = overrideVisualPrefab != null
            ? overrideVisualPrefab
            : roleData.characterPrefab;

        visualInstance = Instantiate(prefabToSpawn, transform);
        visualInstance.transform.localPosition = Vector3.zero;
        visualInstance.transform.localRotation = Quaternion.identity;

        Animator[] animators = visualInstance.GetComponentsInChildren<Animator>(true);
        Debug.Log($"Found {animators.Length} animators in {visualInstance.name}");
        foreach (Animator anim in animators)
        {
            Debug.Log($"Animator: {anim.name}, controller: {anim.runtimeAnimatorController}");
            if (anim.runtimeAnimatorController != null)
            {
                animator = anim;
                break;
            }
        }
        Debug.Log($"Final animator: {animator}");
    }

    public void SetMoveAnimation(bool isMoving)
    {
        if (animator == null) return;
        animator.SetBool("1_Move", isMoving);
    }

    public void SetFacingDirection(float x)
    {
        if (visualInstance == null) return;
        if (x == 0) return;

        Vector3 scale = visualInstance.transform.localScale;
        scale.x = x > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        visualInstance.transform.localScale = scale;
    }

    public void SetAttackAnimation()
    {
        if (animator == null) return;
        animator.SetTrigger("2_Attack");
    }

    public void SetDamagedAnimation()
    {
        if (animator == null) return;
        animator.SetTrigger("3_Damaged");
    }

    public void SetDeathAnimation()
    {
        if (animator == null) return;
        animator.SetTrigger("4_Death");
    }

    public void SetDebuffAnimation(bool isDebuffed)
    {
        if (animator == null) return;
        animator.SetBool("5_Debuff", isDebuffed);
    }
}

// ─── ActiveEffect Class ──────────────────────────────────

[System.Serializable]
public class ActiveEffect
{
    public EffectType effectType;
    public float value;
    public int remainingTurns;
    public string statTarget;

    public ActiveEffect(SkillEffect source)
    {
        effectType = source.effectType;
        value = source.value;
        remainingTurns = Mathf.RoundToInt(source.duration);
        statTarget = source.effectType == EffectType.StatDebuff ? "defense" : "";
    }
}