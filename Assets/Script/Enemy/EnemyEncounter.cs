using UnityEngine;
using System.Collections;

public class EnemyEncounter : MonoBehaviour
{
    [Header("Enemy dalam battle (isi 1-4)")]
    public EnemyData[] enemiesInBattle;

    [Header("Visual Overworld")]
    public GameObject overworldPrefab;

    [Header("Random Spawn Area di BattleScene")]
    public bool randomSpawn = true;

    [Header("Boss? (jika ya, menang = ending)")]
    public bool isBoss = false;

    [HideInInspector] public string enemyId;

    private bool triggered = false;
    private GameObject visualInstance;

    private void Awake()
    {
        enemyId = $"enemy_{transform.position.x}_{transform.position.y}";
    }

    private void Start()
    {
        if (DefeatedEnemyTracker.IsDefeated(enemyId))
        {
            gameObject.SetActive(false);
            return;
        }

        SpawnVisual();
    }

    void SpawnVisual()
    {
        if (overworldPrefab == null) return;

        visualInstance = Instantiate(overworldPrefab, transform);
        visualInstance.transform.localPosition = Vector3.zero;
        visualInstance.transform.localRotation = Quaternion.identity;

        var rb = visualInstance.GetComponentInChildren<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        var animator = visualInstance.GetComponentInChildren<Animator>();
        if (animator != null) animator.SetBool("1_Move", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;
        if (BattleData.Instance == null) return;

        triggered = true;
        StartCoroutine(EncounterSequence(other));
    }

    IEnumerator EncounterSequence(Collider2D other)
    {
        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (FadeEffect.Instance != null)
            yield return StartCoroutine(FadeEffect.Instance.FadeOut());
        else
            yield return new WaitForSeconds(0.3f);

        BattleData.Instance.enemies = enemiesInBattle;
        BattleData.Instance.randomSpawn = randomSpawn;
        BattleData.Instance.currentEnemyId = enemyId;
        BattleData.Instance.isBossBattle = isBoss;
        BattleData.Instance.partyLastPosition = other.transform.position;

        BattleData.Instance.partyBattleData.Clear();
        for (int i = 0; i < PartyData.Instance.partyMembers.Count; i++)
        {
            CharacterRoleData member = PartyData.Instance.partyMembers[i].roleData;
            GameObject overridePrefab = PartyData.Instance.partyMembers[i].overridePrefab;
            BattleData.Instance.partyBattleData.Add(new PartyMemberBattleData
            {
                roleData = member,
                overridePrefab = overridePrefab
            });
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
    }
}