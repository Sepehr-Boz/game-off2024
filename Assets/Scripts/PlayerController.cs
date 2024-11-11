using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float jumpForce = 1;

    private bool canJumpMove = true;

    private Rigidbody2D rigidbody2D;

    void Start()
    {
        if (!rigidbody2D)
            rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
    }

    private void HandleInputs()
    {
        // if cant move then break out
        if (!canJumpMove)
            return;


        Vector2 dir = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        // if the direction upwards >= 0 then jump
        if (dir.y > 0)
        {
            Jump(dir);
        }
        else
            Move(dir);
    }

    private void Jump(Vector2 dir)
    {
        rigidbody2D.AddForce(dir * jumpForce);

    }

    private void Move(Vector2 dir)
    {
        rigidbody2D.MovePosition((Vector2)transform.position + new Vector2(dir.x, 0));
    }

    // on hitting the ground again then set can move true again
    void OnCollisionEnter2D(Collision2D other)
    {
        canJumpMove = true;
    }
}
