using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUnitPanel : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public Slider hpSlider;
    public TextMeshProUGUI mpText;
    public Slider mpSlider;
    public Image background;
    public RawImage portraitImage;
    public float portraitZoom = 2.5f;

    public Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color activeColor = new Color(0.8f, 0.8f, 0f, 0.9f);

    private PortraitCamera portraitCamera;

    public void Setup(string unitName, int hp, int maxHP, int mp, int maxMP,
                  GameObject visualPrefab = null, bool faceRight = true)
    {
        if (nameText != null) nameText.text = unitName;
        UpdateHP(hp, maxHP);
        UpdateMP(mp, maxMP);

        if (visualPrefab != null && portraitImage != null)
        {
            portraitCamera = new GameObject("PortraitCameraHost").AddComponent<PortraitCamera>();
            portraitCamera.Setup(visualPrefab, portraitImage, faceRight);
        }
    }

    public void UpdateHP(int hp, int maxHP)
    {
        float ratio = (float)hp / maxHP;
        if (hpSlider != null) hpSlider.value = ratio;
        if (hpText != null) hpText.text = $"{hp}/{maxHP}";

        var fill = hpSlider?.fillRect?.GetComponent<Image>();
        if (fill != null)
        {
            if (ratio > 0.5f) fill.color = Color.green;
            else if (ratio > 0.25f) fill.color = Color.yellow;
            else fill.color = Color.red;
        }
    }

    public void UpdateMP(int mp, int maxMP)
    {
        if (mpSlider != null) mpSlider.value = maxMP > 0 ? (float)mp / maxMP : 0;
        if (mpText != null) mpText.text = $"{mp}/{maxMP}";
    }


    public void SetActive(bool active)
    {
        if (background != null)
            background.color = active ? activeColor : normalColor;
    }
}