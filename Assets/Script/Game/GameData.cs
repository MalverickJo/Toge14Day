using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public CharacterRoleData selectedRole;

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

    public void SetSelectedRole(CharacterRoleData role)
    {
        selectedRole = role;
        if (PartyData.Instance != null)
            PartyData.Instance.Reset();
        if (LevelData.Instance != null)
            LevelData.Instance.Reset();
        if (BattleData.Instance != null)
        {
            BattleData.Instance.partyLastPosition = Vector3.zero;
            BattleData.Instance.defeatedEnemyPosition = Vector3.zero;
            BattleData.Instance.currentEnemyId = "";
            BattleData.Instance.defeatedEnemyId = "";
            BattleData.Instance.isBossBattle = false;
            BattleData.Instance.partyBattleData.Clear();
        }
        DefeatedEnemyTracker.ClearAll();
    }

#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        if (selectedRole != null)
        {
            UnityEditor.EditorUtility.SetDirty(selectedRole);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
#endif
}