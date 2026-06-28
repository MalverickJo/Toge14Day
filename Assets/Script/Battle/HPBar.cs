using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBar : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Slider slider;

    public void Setup(string unitName, int currentHP, int maxHP)
    {
        if (nameText != null) nameText.text = unitName;
        UpdateHP(currentHP, maxHP);
    }

    public void UpdateHP(int currentHP, int maxHP)
    {
        float ratio = (float)currentHP / maxHP;
        slider.value = ratio;
        if (nameText != null) nameText.text = $"{currentHP}/{maxHP}";

        var fill = slider.fillRect?.GetComponent<Image>();
        if (fill != null)
        {
            if (ratio > 0.5f) fill.color = Color.green;
            else if (ratio > 0.25f) fill.color = Color.yellow;
            else fill.color = Color.red;
        }
    }
}