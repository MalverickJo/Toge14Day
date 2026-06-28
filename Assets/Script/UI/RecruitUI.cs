using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RecruitUI : MonoBehaviour
{
    public static RecruitUI Instance;

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI goldText;
    public Button recruitButton;
    public Button cancelButton;

    private MercenaryData currentData;
    private Action onRecruitSuccess;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);

        recruitButton.onClick.AddListener(OnRecruitClicked);
        cancelButton.onClick.AddListener(Hide);
    }

    public void Show(MercenaryData data, Action onSuccess)
    {
        currentData = data;
        onRecruitSuccess = onSuccess;

        nameText.text = data.mercenaryName;
        dialogText.text = data.recruitDialog;
        costText.text = $"Biaya: {data.hireCost} Gold";
        goldText.text = $"Gold kamu: {PartyData.Instance.gold}";

        bool partyFull = PartyData.Instance.partyMembers.Count >= 4;
        bool notEnoughGold = PartyData.Instance.gold < data.hireCost;

        recruitButton.interactable = !partyFull && !notEnoughGold;
        if (partyFull)
            costText.text = "Party sudah penuh! (Max 3 mercenary)";
        else if (notEnoughGold)
            costText.text = $"Gold tidak cukup! Butuh: {data.hireCost}";

        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnRecruitClicked()
    {
        if (currentData == null) { Debug.LogError("currentData null!"); return; }

        PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();
        if (spawner == null) { Debug.LogError("PlayerSpawner tidak ditemukan!"); return; }

        if (PartyData.Instance.SpendGold(currentData.hireCost))
        {
            PartyData.Instance.AddMember(currentData.role, currentData.overworldPrefab);
            spawner.SpawnPartyMember(currentData.role, currentData.overworldPrefab);
            onRecruitSuccess?.Invoke();
            Hide();
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}