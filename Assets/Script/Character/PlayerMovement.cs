using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Character character;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        character = GetComponent<Character>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(x, y).normalized;
        rb.linearVelocity = move * moveSpeed;

        bool isMoving = x != 0 || y != 0;

        if (character != null)
        {
            character.SetMoveAnimation(isMoving);
            character.SetFacingDirection(x);
        }
    }
}
