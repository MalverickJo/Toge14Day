using UnityEngine;
using System.Collections.Generic;

public enum SkillType
{
    Damage,
    Heal,
    Buff,
    Debuff,
    Control,
    Utility
}

public enum TargetType
{
    SingleTarget,    
    TwoTargets,      
    ThreeTargets,    
    AllEnemies,      
    Self,            
    AllAllies        
}

public enum EffectType
{
    Damage,
    Heal,
    ApplyTaunt,
    DamageReduction,
    StatBuff,
    StatDebuff
}


[System.Serializable]
public class SkillEffect
{
    public EffectType effectType;

    [Range(0, 100)]
    public float chanceToApply = 100f;

    public float value;
    public float duration;

    [Tooltip("Stat yang dipengaruhi: attack / defense / magicAttack / speed")]
    public string statTarget;
}

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("Information")]
    public string skillName;

    [TextArea(2, 5)]
    public string description;

    public SkillType skillType;

    [Header("Audio")]
    public AudioClip skillSound;

    [Header("Targeting")]
    public TargetType targetType;

    [Header("Cost")]
    public int manaCost;

    [Header("Scaling")]
    [Tooltip("Multiplier dari ATK (1 = 100% ATK)")]
    [Range(0f, 5f)]
    public float attackScaling = 1f;

    [Tooltip("Multiplier dari MAG (1 = 100% MAG)")]
    [Range(0f, 5f)]
    public float magicScaling = 0f;

    [Header("Effects")]
    public List<SkillEffect> effects;

    [Header("Visual")]
    public Sprite icon;

    [Header("Cooldown")]
    public int cooldown = 0;

}
