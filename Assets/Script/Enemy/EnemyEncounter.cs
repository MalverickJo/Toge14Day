using UnityEngine;

public class EnemyEncounter : MonoBehaviour
{
    [Header("Enemy dalam battle (isi 1-4)")]
    public EnemyData[] enemiesInBattle;

    [Header("Visual Overworld")]
    public GameObject overworldPrefab;

    [Header("Random Spawn Area di BattleScene")]
    public bool randomSpawn = true;

    private bool triggered = false;
    private GameObject visualInstance;


    private void Start()
    {
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
        if (BattleData.Instance == null) { Debug.LogError("BattleData null!"); return; }

        triggered = true;
        BattleData.Instance.enemies = enemiesInBattle;
        BattleData.Instance.randomSpawn = randomSpawn;
        BattleData.Instance.defeatedEnemyPosition = transform.position;
        BattleData.Instance.partyLastPosition = other.transform.position;

        for (int i = 0; i < PlayerSpawner.Instance.partyObjects.Count; i++)
        {
            Character c = PlayerSpawner.Instance.partyObjects[i].GetComponent<Character>();
            Debug.Log($"partyObjects[{i}]: {c?.RoleData?.roleName} | override: {c?.GetOverridePrefab()}");
        }
        for (int i = 0; i < PartyData.Instance.partyMembers.Count; i++)
        {
            Debug.Log($"partyMembers[{i}]: {PartyData.Instance.partyMembers[i].roleData.roleName}");
        }

        BattleData.Instance.enemies = enemiesInBattle;
        BattleData.Instance.randomSpawn = randomSpawn;

        BattleData.Instance.partyBattleData.Clear();
        foreach (var member in PartyData.Instance.partyMembers)
        {
            BattleData.Instance.partyBattleData.Add(new PartyMemberBattleData
            {
                roleData = member.roleData,
                overridePrefab = member.overridePrefab
            });
            Debug.Log($"Battle member: {member.roleData.roleName} | override: {member.overridePrefab}");
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
    }


}
