using UnityEngine;

[CreateAssetMenu(fileName = "MercenaryData", menuName = "Scriptable Objects/MercenaryData")]
public class MercenaryData : ScriptableObject
{
    [Header("Info")]
    public string mercenaryName;

    [TextArea]
    public string recruitDialog; 

    [Header("Role")]
    public CharacterRoleData role;

    [Header("Overworld Prefab")]
    public GameObject overworldPrefab;

    [Header("Recruit Cost")]
    public int hireCost; 
}
