using UnityEngine;
using System.Collections.Generic;

public class SkillCooldownTracker 
{
    private Dictionary<SkillData, int> cooldowns = new Dictionary<SkillData, int>();

    public bool IsReady(SkillData skill)
    {
        if (!cooldowns.ContainsKey(skill)) return true;
        return cooldowns[skill] <= 0;
    }

    public int GetCooldown(SkillData skill)
    {
        if (!cooldowns.ContainsKey(skill)) return 0;
        return cooldowns[skill];
    }

    public void UseSkill(SkillData skill)
    {
        cooldowns[skill] = skill.cooldown;
    }

    public void TickCooldowns()
    {
        var keys = new List<SkillData>(cooldowns.Keys);
        foreach (var key in keys)
            if (cooldowns[key] > 0) cooldowns[key]--;
    }
}
