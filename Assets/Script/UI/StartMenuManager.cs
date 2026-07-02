using UnityEngine;

public class StartMenuManager : MonoBehaviour
{
    public GameObject startMenuPanel;
    public GameObject characterSelectPanel;

    private void Start()
    {
        startMenuPanel.SetActive(true);
        characterSelectPanel.SetActive(false);
    }

    public void OnStartButton()
    {
        if (LevelData.Instance != null) LevelData.Instance.Reset();
        if (PartyData.Instance != null) PartyData.Instance.Reset();
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
        IntroCutScene.ResetCutscene();
        startMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }

    public void OnBackButton()
    {
        characterSelectPanel.SetActive(false);
        startMenuPanel.SetActive(true);
    }
}
