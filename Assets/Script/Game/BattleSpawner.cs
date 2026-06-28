using UnityEngine;
using System.Collections.Generic;

public class BattleSpawner : MonoBehaviour
{
    [Header("Player Prefab")]
    public GameObject playerPrefab;

    [Header("Enemy Prefab (base)")]
    public GameObject enemyPrefab;

    [Header("Party Spawn Points (kiri, max 4)")]
    public Transform[] partySpawnPoints;

    [Header("Enemy Spawn Points (kanan)")]
    public Transform[] enemySpawnPoints;
    public Transform bossSpawnPoint;

    public List<GameObject> partyObjects = new List<GameObject>();
    public List<GameObject> enemyObjects = new List<GameObject>();

    private void Start()
    {
        SpawnParty();
        SpawnEnemies();
    }

    void SpawnParty()
    {
        var members = BattleData.Instance.partyBattleData;

        for (int i = 0; i < members.Count && i < partySpawnPoints.Length; i++)
        {
            GameObject member = Instantiate(playerPrefab, partySpawnPoints[i].position, Quaternion.identity);
            Character character = member.GetComponent<Character>();

            if (members[i].overridePrefab != null)
                character.SetOverridePrefab(members[i].overridePrefab);

            character.SetRole(members[i].roleData);

            if (LevelData.Instance != null)
                character.SetLevel(LevelData.Instance.partyLevel);

            character.SetFacingDirection(1f);

            PlayerMovement pm = member.GetComponent<PlayerMovement>();
            if (pm != null) pm.enabled = false;

            Rigidbody2D rb = member.GetComponent<Rigidbody2D>();
            if (rb != null) rb.simulated = false;

            partyObjects.Add(member);
        }
    }

    void SpawnEnemies()
    {
        EnemyData[] enemies = BattleData.Instance.enemies;
        if (enemies == null || enemies.Length == 0) return;

        bool isBoss = enemies.Length == 1 && enemies[0].role == EnemyRole.Boss;
        Transform[] slots = (BattleData.Instance.randomSpawn && !isBoss)
            ? ShuffleTransforms(enemySpawnPoints)
            : enemySpawnPoints;

        for (int i = 0; i < enemies.Length; i++)
        {
            Transform spawnPoint = isBoss ? bossSpawnPoint : slots[i];

            GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy == null)
            {
                Debug.LogError("Enemy.cs tidak ada di enemyPrefab! Tambahkan komponen Enemy.cs");
                return;
            }

            enemy.Init(enemies[i]);

            if (enemies[i].enemyPrefab != null)
            {
                GameObject visual = Instantiate(enemies[i].enemyPrefab, enemyObj.transform);
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.identity;

                visual.transform.localScale = new Vector3(
                    Mathf.Abs(visual.transform.localScale.x),
                    visual.transform.localScale.y,
                    visual.transform.localScale.z
                );

                var rb = visual.GetComponentInChildren<Rigidbody2D>();
                if (rb != null) rb.simulated = false;

                var animator = visual.GetComponentInChildren<Animator>();
                if (animator != null) animator.SetBool("1_Move", false);
            }

            enemyObjects.Add(enemyObj);
        }
    }
    Transform[] ShuffleTransforms(Transform[] original)
    {
        Transform[] shuffled = (Transform[])original.Clone();
        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            Transform temp = shuffled[i];
            shuffled[i] = shuffled[rand];
            shuffled[rand] = temp;
        }
        return shuffled;
    }
}
