using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance;

    public GameObject playerPrefab;
    public Transform spawnPoint;

    private GameObject leader;
    public List<GameObject> partyObjects = new List<GameObject>();
    private Dictionary<GameObject, GameObject> overridePrefabs = new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game") return;

        AudioManager.Instance?.PlayOverworldMusic();

        Debug.Log($"OnSceneLoaded - Game scene");
        Debug.Log($"PartyData count: {PartyData.Instance?.partyMembers?.Count}");

        partyObjects.Clear();
        overridePrefabs.Clear();
        leader = null;

        if (BattleData.Instance != null && !string.IsNullOrEmpty(BattleData.Instance.currentEnemyId))
        {
            string defeatedId = BattleData.Instance.currentEnemyId;
            EnemyEncounter[] encounters = FindObjectsByType<EnemyEncounter>(FindObjectsSortMode.None);
            foreach (var enc in encounters)
            {
                if (enc.enemyId == defeatedId)
                {
                    Destroy(enc.gameObject);
                    break;
                }
            }
            DefeatedEnemyTracker.MarkDefeated(defeatedId);
            BattleData.Instance.currentEnemyId = "";
        }

        Vector3 spawnPos = Vector3.zero;
        if (BattleData.Instance != null && BattleData.Instance.partyLastPosition != Vector3.zero)
        {
            spawnPos = BattleData.Instance.partyLastPosition;
            BattleData.Instance.partyLastPosition = Vector3.zero;
        }
        else
        {
            spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        }

        if (GameData.Instance == null || GameData.Instance.selectedRole == null)
        {
            Debug.LogError("GameData atau selectedRole null!");
            return;
        }

        if (!PartyData.Instance.partyMembers.Exists(m => m.roleData == GameData.Instance.selectedRole))
            PartyData.Instance.partyMembers.Insert(0, new PartyMemberInfo(GameData.Instance.selectedRole));

        SpawnAtPosition(spawnPos);

        CameraFollow cam = Camera.main?.GetComponent<CameraFollow>();
        if (cam != null && leader != null)
        {
            cam.target = leader.transform;
            Debug.Log($"Camera target assigned: {leader.name}");
        }
        else
        {
            Debug.LogError($"Camera null: {cam == null}, leader null: {leader == null}");
        }

        MinimapFollow minimapFollow = FindFirstObjectByType<MinimapFollow>();
        if (minimapFollow != null && leader != null)
        {
            minimapFollow.target = leader.transform;
            Debug.Log("Minimap follow target assigned: " + leader.name);
        }
        else
        {
            Debug.LogError($"MinimapFollow null: {minimapFollow == null}, leader null: {leader == null}");
        }

    }


    void SpawnAtPosition(Vector3 spawnPos)
    {
        for (int i = 0; i < PartyData.Instance.partyMembers.Count; i++)
        {
            var member = PartyData.Instance.partyMembers[i];

            GameObject memberObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            Character character = memberObj.GetComponent<Character>();

            if (member.overridePrefab != null)
                character.SetOverridePrefab(member.overridePrefab);

            character.SetRole(member.roleData);

            if (i == 0)
            {
                leader = memberObj;
                partyObjects.Add(leader);
            }
            else
            {
                PlayerMovement pm = memberObj.GetComponent<PlayerMovement>();
                if (pm != null) pm.enabled = false;

                Rigidbody2D rb = memberObj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    rb.simulated = false;
                }

                PartyFollow follow = memberObj.AddComponent<PartyFollow>();
                follow.SetTarget(partyObjects[partyObjects.Count - 1].transform, partyObjects.Count);
                partyObjects.Add(memberObj);
            }
        }
    }

    public GameObject GetOverridePrefab(CharacterRoleData role)
    {
        for (int i = 0; i < partyObjects.Count; i++)
        {
            Character c = partyObjects[i].GetComponent<Character>();
            if (c != null && c.RoleData == role)
                return overridePrefabs.ContainsKey(partyObjects[i]) ? overridePrefabs[partyObjects[i]] : null;
        }
        return null;
    }

    public void SpawnPartyMember(CharacterRoleData role, GameObject overridePrefab = null)
    {
        if (leader == null) { Debug.LogError("Leader null!"); return; }

        GameObject member = Instantiate(playerPrefab, leader.transform.position, Quaternion.identity);

        Character character = member.GetComponent<Character>();
        if (character == null) { Debug.LogError("Character tidak ada!"); return; }

        if (overridePrefab != null)
        {
            character.SetOverridePrefab(overridePrefab);
            overridePrefabs[member] = overridePrefab;
        }

        character.SetRole(role);

        PlayerMovement pm = member.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        Rigidbody2D rb = member.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        PartyFollow follow = member.AddComponent<PartyFollow>();
        follow.SetTarget(partyObjects[partyObjects.Count - 1].transform, partyObjects.Count);
        partyObjects.Add(member);
    }
}