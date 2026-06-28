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
    }
    private void OnApplicationQuit()
    {
        if (selectedRole != null)
        {
            UnityEditor.EditorUtility.SetDirty(selectedRole);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
}
