using UnityEngine;

public class BattleUnit
{
    public string unitName;
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;
    public int attack;
    public int defense;
    public int magicAttack;
    public int magicDefense;
    public int speed;
    public bool isEnemy;
    public bool isDead => currentHP <= 0;

    public Character characterRef;
    public Enemy enemyRef;
    public GameObject gameObject;

    public SkillCooldownTracker cooldownTracker = new SkillCooldownTracker();

    public static BattleUnit FromCharacter(Character c, GameObject go)
    {
        return new BattleUnit
        {
            unitName = c.RoleData.roleName,
            currentHP = c.currentHP,
            maxHP = c.GetMaxHP(),
            currentMP = c.currentMP,
            maxMP = c.GetMaxMP(),
            attack = c.GetCurrentAttack(),
            defense = c.GetCurrentDefense(),
            magicAttack = c.GetCurrentMagicAttack(),
            magicDefense = c.GetCurrentMagicDefense(),
            speed = c.GetCurrentSpeed(),
            isEnemy = false,
            characterRef = c,
            gameObject = go
        };
    }

    public static BattleUnit FromEnemy(Enemy e, GameObject go)
    {
        return new BattleUnit
        {
            unitName = e.RoleData.enemyName,
            currentHP = e.currentHP,
            maxHP = e.GetMaxHP(),
            currentMP = e.currentMP,
            maxMP = e.GetMaxMP(),
            attack = e.GetAttack(),
            defense = e.GetDefense(),
            magicAttack = e.GetMagicAttack(),
            magicDefense = e.GetDefense(),
            speed = e.GetSpeed(),
            isEnemy = true,
            enemyRef = e,
            gameObject = go
        };
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage - (defense / 3));
        currentHP = Mathf.Max(0, currentHP - finalDamage);

        if (characterRef != null) characterRef.TakeDamageRaw(finalDamage);
        if (enemyRef != null) enemyRef.TakeDamageRaw(finalDamage);

        CameraShake.Instance?.Shake(0.15f, 0.08f);

        if (isDead)
            BattleManager.Instance.OnUnitDied(this);
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }

    public void RegenMPPercent(float percent)
    {
        int amount = Mathf.RoundToInt(maxMP * percent);
        currentMP = Mathf.Min(maxMP, currentMP + amount);
        if (characterRef != null) characterRef.RegenMP(amount);
        if (enemyRef != null) enemyRef.RegenMP(amount);
    }
}