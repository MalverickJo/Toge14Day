using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    public EnemyData RoleData => enemyData;

    public int currentHP { get; private set; }
    public int currentMP { get; private set; }
    public bool IsDead => currentHP <= 0;

    private List<ActiveEffect> activeEffects = new List<ActiveEffect>();
    public static GameObject worldHPBarPrefab;
    private HPBar worldHPBar;

    private SpriteRenderer[] cachedRenderers;
    private Color[] originalColors;

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
        worldHPBar?.Setup(enemyData.enemyName, currentHP, GetMaxHP());
    }

    public void UpdateWorldHPBar()
    {
        worldHPBar?.UpdateHP(currentHP, GetMaxHP());
    }
    public void Init(EnemyData data)
    {
        enemyData = data;
        currentHP = GetMaxHP();
        currentMP = GetMaxMP();
        activeEffects.Clear();
    }


    public void TakeDamageRaw(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);

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

    public int GetMaxMP() => enemyData.maxMP + (enemyData.mpGrowth * (enemyData.enemyLevel - 1));

    public void ApplyEffect(SkillEffect effect)
    {
        if (Random.Range(0f, 100f) <= effect.chanceToApply)
            activeEffects.Add(new ActiveEffect(effect));
    }

    public void TickEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].remainingTurns--;
            if (activeEffects[i].remainingTurns <= 0)
                activeEffects.RemoveAt(i);
        }
    }

    public int GetMaxHP() => enemyData.maxHP + (enemyData.hpGrowth * (enemyData.enemyLevel - 1));
    public int GetAttack() => enemyData.attack + (enemyData.attackGrowth * (enemyData.enemyLevel - 1));
    public int GetDefense() => enemyData.defense + (enemyData.defenseGrowth * (enemyData.enemyLevel - 1));
    public int GetMagicAttack() => enemyData.magicAttack + (enemyData.magicAttackGrowth * (enemyData.enemyLevel - 1));
    public int GetSpeed() => enemyData.speed + (enemyData.speedGrowth * (enemyData.enemyLevel - 1));


    float GetDamageReduction()
    {
        float total = 0f;
        foreach (var e in activeEffects)
            if (e.effectType == EffectType.DamageReduction)
                total += e.value / 100f;
        return Mathf.Clamp(total, 0f, 0.75f);
    }

    public SkillData GetRandomSkill()
    {
        if (enemyData.skills == null || enemyData.skills.Count == 0) return null;

        List<SkillData> affordable = enemyData.skills.FindAll(s => s.manaCost <= currentMP);
        if (affordable.Count == 0) return null;

        return affordable[Random.Range(0, affordable.Count)];
    }

    public bool HasTaunt() => activeEffects.Exists(e => e.effectType == EffectType.ApplyTaunt);

    public void SetActiveIndicator(bool active)
    {
        if (this == null || gameObject == null) return;

        if (cachedRenderers == null)
        {
            cachedRenderers = GetComponentsInChildren<SpriteRenderer>();
            originalColors = new Color[cachedRenderers.Length];
            for (int i = 0; i < cachedRenderers.Length; i++)
                originalColors[i] = cachedRenderers[i].color;
        }

        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] == null) continue;

            if (active)
            {
                Color baseColor = originalColors[i];
                cachedRenderers[i].color = new Color(
                    baseColor.r,
                    baseColor.g * 0.6f,
                    baseColor.b * 0.6f,
                    baseColor.a
                );
            }
            else
            {
                cachedRenderers[i].color = originalColors[i];
            }
        }
    }
}