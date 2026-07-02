using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName;
    [TextArea(2, 5)]
    public string dialogueText;
    public Sprite npcPortrait;

    [Header("Interaction")]
    public float interactionRange = 1.5f;

    private bool playerInRange = false;
    private Transform player;

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        playerInRange = dist <= interactionRange;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            player = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            player = null;
    }

    void ShowDialogue()
    {
        Debug.Log($"{npcName}: {dialogueText}");
    }
}
