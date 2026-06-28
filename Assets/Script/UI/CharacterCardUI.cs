using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCardUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI roleName;
    public TextMeshProUGUI roleStats;
    public Button selectButton;

    [Header("Preview")]
    public Transform previewAnchor;

    private GameObject spawnedPreview;

    public void Setup(CharacterRoleData roleData)
    {
        if (roleName != null) roleName.text = roleData.roleName;
        if (roleStats != null)
            roleStats.text =
                $"HP : {roleData.maxHP}\n" +
                $"MP : {roleData.maxMP}\n" +
                $"ATK: {roleData.attack}\n" +
                $"DEF: {roleData.defense}\n" +
                $"MAG: {roleData.magicAttack}\n" +
                $"SPD: {roleData.speed}";

        if (spawnedPreview != null)
            Destroy(spawnedPreview);

        if (roleData.characterPrefab != null && previewAnchor != null)
        {
            spawnedPreview = Instantiate(roleData.characterPrefab, previewAnchor);
            spawnedPreview.transform.localPosition = Vector3.zero;
            spawnedPreview.transform.localRotation = Quaternion.identity;
            spawnedPreview.transform.localScale = Vector3.one * 100f;

            SpriteRenderer[] renderers = spawnedPreview.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in renderers)
            {
                sr.sortingLayerName = "Default";
                sr.sortingOrder += 50; 
            }

            var rb = spawnedPreview.GetComponentInChildren<Rigidbody2D>();
            if (rb != null) rb.simulated = false;

            var animator = spawnedPreview.GetComponentInChildren<Animator>();
            if (animator != null)
                animator.SetBool("1_Move", false);
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() =>
            {
                GameData.Instance.selectedRole = roleData;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            });
        }
    }
}
