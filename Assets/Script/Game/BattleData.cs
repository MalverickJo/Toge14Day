using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PartyMemberBattleData
{
    public CharacterRoleData roleData;
    public GameObject overridePrefab;
}

public class BattleData : MonoBehaviour
{
    public static BattleData Instance;
    public Vector3 playerLastPosition;
    public EnemyData[] enemies;
    public bool randomSpawn = true;
    public List<PartyMemberBattleData> partyBattleData = new List<PartyMemberBattleData>();
    public GameObject currentEncounter;
    public Vector3 defeatedEnemyPosition;
    public Vector3 partyLastPosition; 
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
}