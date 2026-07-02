using UnityEngine;
using System.Collections;

public class IntroCutScene : MonoBehaviour
{
    [Header("Waypoints (world position)")]
    public Transform pointUp;
    public Transform pointLeft;

    [Header("NPC Dialogue")]
    public string npcName = "Penjaga Desa";
    public string[] dialogueLines;

    [Header("Minimap")]
    public GameObject minimapObject;

    [Header("Settings")]
    public float moveSpeed = 3f;
    public bool playOnAwake = true;
    public bool playOnce = true;

    private static bool hasPlayedStatic = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetOnPlay()
    {
        hasPlayedStatic = false;
    }

    private void Start()
    {
        if (playOnAwake)
            StartCoroutine(WaitThenPlay());
    }

    IEnumerator WaitThenPlay()
    {
        // Tunggu sampai player benar-benar ter-spawn
        float waited = 0f;
        while ((PlayerSpawner.Instance == null || PlayerSpawner.Instance.partyObjects.Count == 0) && waited < 5f)
        {
            waited += Time.unscaledDeltaTime;
            yield return null;
        }

        GameObject player = PlayerSpawner.Instance?.partyObjects.Count > 0
            ? PlayerSpawner.Instance.partyObjects[0]
            : null;

        if (player == null)
        {
            Debug.LogError("Player tidak ditemukan untuk cutscene!");
            yield break;
        }

        StartCutscene(player);
    }

    public void StartCutscene(GameObject player)
    {
        if (playOnce && hasPlayedStatic) return;
        hasPlayedStatic = true;
        StartCoroutine(RunCutscene(player));
    }

    IEnumerator RunCutscene(GameObject player)
    {
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        Character character = player.GetComponent<Character>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

        LetterboxUI.Instance?.Show();
        SetMinimapActive(false);

        yield return new WaitForSecondsRealtime(0.3f); // ← realtime

        if (pointUp != null)
            yield return StartCoroutine(MoveToPoint(player, character, pointUp.position));

        yield return new WaitForSecondsRealtime(0.2f); // ← realtime

        if (pointLeft != null)
            yield return StartCoroutine(MoveToPoint(player, character, pointLeft.position));

        character?.SetMoveAnimation(false);
        LetterboxUI.Instance?.Hide();
        SetMinimapActive(true);

        if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSecondsRealtime(0.3f); // ← realtime

        DialogueUI.Instance?.Show(npcName, dialogueLines, () =>
        {
            if (pm != null) pm.enabled = true;
        });
    }

    void SetMinimapActive(bool active)
    {
        if (minimapObject != null)
            minimapObject.SetActive(active);
        else
            Debug.LogWarning("Minimap object belum di-assign di Inspector!");
    }

    IEnumerator MoveToPoint(GameObject player, Character character, Vector3 target)
    {
        Vector3 start = player.transform.position;
        float distance = Vector3.Distance(start, target);
        if (distance < 0.01f) yield break;

        float duration = distance / moveSpeed;
        float elapsed = 0f;

        Vector3 dir = (target - start).normalized;
        character?.SetMoveAnimation(true);
        character?.SetFacingDirection(dir.x);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // ← unscaled
            player.transform.position = Vector3.Lerp(start, target, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        player.transform.position = target;
    }

    public static void ResetCutscene()
    {
        hasPlayedStatic = false;
    }
}