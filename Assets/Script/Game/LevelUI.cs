using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    private void Update()
    {
        if (LevelData.Instance == null) return;

        int level = LevelData.Instance.partyLevel;
        levelText.text = $"Level {level}";
    }
}

