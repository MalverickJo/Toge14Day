using UnityEngine;

public class MercenaryNPC : MonoBehaviour
{
    public MercenaryData mercenaryData;

    private bool recruited = false;
    private bool playerNearby = false;
    private GameObject visualInstance;

    private void Start()
    {
        if (PartyData.Instance != null &&
            PartyData.Instance.recruitedNPCNames.Contains(mercenaryData.mercenaryName))
        {
            Destroy(gameObject);
            return;
        }

        SpawnVisual();
    }

    void SpawnVisual()
    {
        if (mercenaryData == null) return;
        if (mercenaryData.overworldPrefab == null)
        {
            Debug.LogWarning($"overworldPrefab kosong di {mercenaryData.mercenaryName}!");
            return;
        }

        visualInstance = Instantiate(mercenaryData.overworldPrefab, transform);
        visualInstance.transform.localPosition = Vector3.zero;
        visualInstance.transform.localRotation = Quaternion.identity;

        var rb = visualInstance.GetComponentInChildren<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        var animator = visualInstance.GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetBool("1_Move", false);
    }

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (RecruitUI.Instance == null)
            {
                Debug.LogError("RecruitUI.Instance null!");
                return;
            }
            RecruitUI.Instance.Show(mercenaryData, OnRecruited);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (recruited) return;
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
    }

    void OnRecruited()
    {
        recruited = true;
        playerNearby = false;

        if (PartyData.Instance != null)
            PartyData.Instance.AddRecruitedNPC(mercenaryData.mercenaryName);

        Destroy(gameObject);
    }
}