using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private float jumpSpeed = 7.5f;
    [SerializeField] private float gravity = 1;
    [SerializeField] private float fallingGravity = 3;
    [SerializeField] private float revJumpMult = 1.5f;

    private Vector2 dir;
    private Vector2 absDir;
    private bool canJump = true;

    private Rigidbody2D rigidbody2D;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private Vector2 getInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    // Update is called once per frame
    void Update()
    {
        dir = getInput();
        absDir = dir.Abs();

        // move
        if (absDir.x > 0)
        {
            rigidbody2D.linearVelocityX = dir.x * moveSpeed;
        }
        else
        {
            rigidbody2D.linearVelocityX = 0;
        }

        // jump
        if (canJump && dir.y > 0)
        {
            rigidbody2D.linearVelocityY = dir.y * jumpSpeed;
            canJump = false;
        }

        // increase gravity when falling/when down pressed
        if ((rigidbody2D.linearVelocityY < 0 || dir.y < 0) && !canJump)
        {
            rigidbody2D.gravityScale = fallingGravity * (dir.y < 0 ? revJumpMult : 1);
        }
        else
        {
            rigidbody2D.gravityScale = gravity;
        }
    }

    // on hitting the ground again then set can move true again
    void OnCollisionEnter2D(Collision2D other)
    {
        canJump = true;
    }
}
