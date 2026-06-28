using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PartyMemberInfo
{
    public CharacterRoleData roleData;
    public GameObject overridePrefab;

    public PartyMemberInfo(CharacterRoleData role, GameObject prefab = null)
    {
        roleData = role;
        overridePrefab = prefab;
    }
}

public class PartyData : MonoBehaviour
{
    public static PartyData Instance;

    public List<PartyMemberInfo> partyMembers = new List<PartyMemberInfo>();
    public int gold = 500;

    public List<string> recruitedNPCNames = new List<string>();

    public void AddRecruitedNPC(string npcName)
    {
        if (!recruitedNPCNames.Contains(npcName))
            recruitedNPCNames.Add(npcName);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Reset()
    {
        partyMembers.Clear();
        gold = 500;
    }

    public bool AddMember(CharacterRoleData role, GameObject overridePrefab = null)
    {
        if (partyMembers.Count >= 4)
        {
            Debug.Log("Party sudah penuh!");
            return false;
        }
        partyMembers.Add(new PartyMemberInfo(role, overridePrefab));
        return true;
    }

    public void RemoveMember(CharacterRoleData role, GameObject overridePrefab = null)
    {
        if (overridePrefab != null)
        {
            int idx = partyMembers.FindIndex(m => m.roleData == role && m.overridePrefab == overridePrefab);
            if (idx >= 0) partyMembers.RemoveAt(idx);
        }
        else
        {
            int idx = partyMembers.FindIndex(m => m.roleData == role);
            if (idx >= 0) partyMembers.RemoveAt(idx);
        }
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        return true;
    }
}