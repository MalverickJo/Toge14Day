using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI descriptionText;
    public Button button;

    private void Awake()
    { 
        if (button == null)
            button = GetComponent<Button>();
        if (button == null)
            button = GetComponentInChildren<Button>();
    }
}