using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BattleUI : MonoBehaviour
{
    [Header("Action Panel")]
    public GameObject actionPanel;
    public Button attackButton;
    public Button skillButton;
    public Button itemButton;

    [Header("Skill Panel")]
    public GameObject skillPanel;
    public SkillButton[] skillButtons;
    public Button backButton;

    [Header("Target Panel")]
    public GameObject targetPanel;

    [Header("Win/Lose Panel")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("HP Bars")]
    public Transform partyHPContainer;
    public Transform enemyHPContainer;
    public GameObject hpBarPrefab;
    public GameObject unitPanelPrefab;
    public Transform[] partyPanelSlots;
    public Transform[] enemyPanelSlots;

    [Header("Damage Text")]
    public GameObject damageTextPrefab;
    public Canvas worldCanvas;

    [Header("Message")]
    public TextMeshProUGUI messageText;
    private Coroutine messageCoroutine;

    private BattleUnit currentUnit;
    private Dictionary<BattleUnit, BattleUnitPanel> panels = new Dictionary<BattleUnit, BattleUnitPanel>();
    private System.Action<BattleUnit> onTargetSelected;

    private void Start()
    {
        attackButton.onClick.AddListener(OnAttackButton);
        skillButton.onClick.AddListener(OnSkillButton);
        if (backButton != null)
            backButton.onClick.AddListener(() => {
                skillPanel.SetActive(false);
                actionPanel.SetActive(true);
            });

        winPanel.SetActive(false);
        losePanel.SetActive(false);
        skillPanel.SetActive(false);
        actionPanel.SetActive(false);
    }

    public void ShowMessage(string message, float duration = 1.5f)
    {
        if (messageText == null) return;
        if (messageCoroutine != null) StopCoroutine(messageCoroutine);
        messageCoroutine = StartCoroutine(ShowMessageCoroutine(message, duration));
    }

    IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        yield return new WaitForSeconds(duration);
        messageText.gameObject.SetActive(false);
    }


    public void ShowPlayerActions(BattleUnit unit)
    {
        currentUnit = unit;
        actionPanel.SetActive(true);
        skillPanel.SetActive(false);

        attackButton.interactable = true;
        skillButton.interactable = true;
        itemButton.interactable = true;
    }

    public void HidePlayerActions()
    {
        skillPanel.SetActive(false);

        attackButton.interactable = false;
        skillButton.interactable = false;
        itemButton.interactable = false;
    }   

    void OnAttackButton()
    { 
        attackButton.interactable = false;
        skillButton.interactable = false;
        itemButton.interactable = false;

        SelectTarget(true, target =>
        {
            attackButton.interactable = true;
            skillButton.interactable = true;
            itemButton.interactable = true;

            BattleManager.Instance.PlayerAttack(target);
        });
    }

    void OnSkillButton()
    {
        actionPanel.SetActive(true);
        skillPanel.SetActive(true);

        var skills = currentUnit.characterRef.RoleData.skills;
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (i < skills.Count)
            {
                int idx = i;
                skillButtons[i].gameObject.SetActive(true);

                bool canAfford = currentUnit.currentMP >= skills[idx].manaCost;

                skillButtons[i].button.interactable = canAfford;

                if (skillButtons[i].skillNameText != null)
                    skillButtons[i].skillNameText.text = $"{skills[idx].skillName}\n(MP:{skills[idx].manaCost})";

                if (skillButtons[i].descriptionText != null)
                    skillButtons[i].descriptionText.text = skills[idx].description;

                if (skillButtons[i].iconImage != null && skills[idx].icon != null)
                    skillButtons[i].iconImage.sprite = skills[idx].icon;

                skillButtons[i].button.onClick.RemoveAllListeners();
                skillButtons[i].button.onClick.AddListener(() => OnSkillSelected(skills[idx]));
            }
            else
            {
                skillButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnSkillSelected(SkillData skill)
    {
        skillPanel.SetActive(false);

        if (skill.targetType == TargetType.Self || skill.targetType == TargetType.AllAllies
            || skill.targetType == TargetType.AllEnemies)
        {
            BattleManager.Instance.PlayerSkill(null, skill);
        }
        else
        {
            SelectTarget(true, target =>
            {
                BattleManager.Instance.PlayerSkill(target, skill);
            });
        }
    }

    void SelectTarget(bool enemyOnly, System.Action<BattleUnit> callback)
    {
        
        onTargetSelected = callback;
        BattleTargetSelector.Instance.StartSelection(enemyOnly, callback);
    }

    public void SetupHPBars(List<BattleUnit> units)
    {
        int partyIdx = 0, enemyIdx = 0;

        foreach (var unit in units)
        {
            if (unitPanelPrefab == null) continue;

            Transform slot;
            if (unit.isEnemy)
            {
                if (enemyIdx >= enemyPanelSlots.Length) continue;
                slot = enemyPanelSlots[enemyIdx++];
            }
            else
            {
                if (partyIdx >= partyPanelSlots.Length) continue;
                slot = partyPanelSlots[partyIdx++];
            }

            GameObject panelObj = Instantiate(unitPanelPrefab, slot);
            panelObj.transform.localPosition = Vector3.zero;

            BattleUnitPanel panel = panelObj.GetComponent<BattleUnitPanel>();

            GameObject portraitPrefab = null;
            if (unit.characterRef != null)
                portraitPrefab = unit.characterRef.GetOverridePrefab() ?? unit.characterRef.RoleData?.characterPrefab;
            else if (unit.enemyRef != null)
                portraitPrefab = unit.enemyRef.RoleData?.enemyPrefab;

            panels[unit] = panel;
            panel.Setup(unit.unitName, unit.currentHP, unit.maxHP,
                        unit.currentMP, unit.maxMP, portraitPrefab, !unit.isEnemy);
        }
    }

    public void UpdateAllHPBars(List<BattleUnit> units)
    {
        foreach (var unit in units)
        {
            if (panels.ContainsKey(unit))
            {
                panels[unit].UpdateHP(unit.currentHP, unit.maxHP);
                panels[unit].UpdateMP(unit.currentMP, unit.maxMP);
            }
        }
    }

    public void SetActivePanel(BattleUnit unit, bool active)
    {
        if (panels.ContainsKey(unit))
            panels[unit].SetActive(active);
    }

    public void ShowDamageText(GameObject target, int damage)
    {
        if (damageTextPrefab == null || worldCanvas == null) return;

        GameObject canvasObj = Instantiate(damageTextPrefab, worldCanvas.transform);
        TextMeshProUGUI tmp = canvasObj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp == null) return;

        tmp.text = damage.ToString();
        tmp.color = new Color(1f, 0.2f, 0.2f, 1f);
        tmp.fontSize = 48;

        Canvas innerCanvas = canvasObj.GetComponent<Canvas>();
        if (innerCanvas != null) Destroy(innerCanvas);

        RectTransform rt = canvasObj.GetComponent<RectTransform>();
        Vector2 screenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            worldCanvas.GetComponent<RectTransform>(),
            Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up * 0.5f),
            worldCanvas.worldCamera,
            out screenPos
        );
        rt.anchoredPosition = screenPos;

        StartCoroutine(AnimateDamageText(rt, tmp, canvasObj));
    }

    IEnumerator AnimateDamageText(RectTransform rt, TextMeshProUGUI tmp, GameObject textObj)
    {
        float elapsed = 0f;
        Vector2 startPos = rt.anchoredPosition;
        Color startColor = tmp != null ? tmp.color : Color.red;

        while (elapsed < 1f)
        {
            if (rt == null) yield break;
            elapsed += Time.deltaTime;
            rt.anchoredPosition = startPos + Vector2.up * (elapsed * 100f);
            if (tmp != null)
                tmp.color = new Color(startColor.r, startColor.g, startColor.b, 1f - elapsed);
            yield return null;
        }

        if (textObj != null) Destroy(textObj);
    }

    public void RemoveUnitPanel(BattleUnit unit)
    {
        if (panels.ContainsKey(unit))
        {
            StartCoroutine(FadeOutPanel(panels[unit].gameObject));
            panels.Remove(unit);
        }
    }

    IEnumerator FadeOutPanel(GameObject panel)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - (elapsed / 0.5f);
            yield return null;
        }

        Destroy(panel);
    }

    public void ShowWinPanel() => winPanel.SetActive(true);
    public void ShowLosePanel() => losePanel.SetActive(true);
}