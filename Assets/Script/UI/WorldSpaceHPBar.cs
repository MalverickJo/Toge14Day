using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHPBar : MonoBehaviour
{
    public Image fillImage;
    public Canvas canvas;

    public void Setup(int maxHP)
    {
        fillImage.fillAmount = 1f;
    }

    public void UpdateHP(int current, int max)
    {
        float ratio = (float)current / max;
        fillImage.fillAmount = ratio;

        if (ratio > 0.5f) fillImage.color = Color.green;
        else if (ratio > 0.25f) fillImage.color = Color.yellow;
        else fillImage.color = Color.red;
    }
}