using UnityEngine;
using System.Collections.Generic;

public enum EnemyRole
{
    Warrior,
    Archer,
    Warlock,
    Cleric,
    Rogue,
    Boss
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Information")]
    public EnemyRole role;
    public string enemyName;

    [TextArea]
    public string description;

    [Header("Level")]
    public int enemyLevel = 1;

    [Header("Base Stats")]
    public int maxHP;
    public int maxMP;
    public int attack;
    public int defense;
    public int magicAttack;
    public int magicDefense;
    public int speed;

    [Header("Stat Growth per Level")]
    public int hpGrowth = 10;
    public int mpGrowth = 5;
    public int attackGrowth = 2;
    public int defenseGrowth = 2;
    public int magicAttackGrowth = 2;
    public int magicDefenseGrowth = 2;
    public int speedGrowth = 1;

    [Header("Skills")]
    public List<SkillData> skills;

    [Header("Reward")]
    public int expReward = 50;
    public int goldReward = 20;

    [Header("Visual")]
    public GameObject enemyPrefab;
}
