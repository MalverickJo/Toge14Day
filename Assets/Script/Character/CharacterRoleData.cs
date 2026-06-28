using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterRoleData", menuName = "Scriptable Objects/CharacterRoleData")]
public class CharacterRoleData : ScriptableObject
{
    public CharacterRole role;
    public string roleName;

    [TextArea]
    public string description;

    public int maxHP;
    public int maxMP;
    public int attack;
    public int defense;
    public int magicAttack;
    public int magicDefense;
    public int speed;

    [Header("Skills")]
    public List<SkillData> skills = new List<SkillData>();

    [Header("Visual (Prefab)")]
    public GameObject characterPrefab;

    [Header("Animation")]
    public RuntimeAnimatorController animator;

    [Header("Stat Growth per Level")]
    public int hpGrowth = 10;
    public int mpGrowth = 5;
    public int attackGrowth = 2;
    public int defenseGrowth = 2;
    public int magicAttackGrowth = 2;
    public int magicDefenseGrowth = 2;
    public int speedGrowth = 1;
}
