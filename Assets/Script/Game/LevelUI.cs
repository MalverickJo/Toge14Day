using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI goldText;

    private void Update()
    {
        if (LevelData.Instance == null) return;

        int level = LevelData.Instance.partyLevel;
        levelText.text = $"Level {level}";

        if (goldText != null && PartyData.Instance != null)
            goldText.text = $"Gold: {PartyData.Instance.gold}";
    }
}

