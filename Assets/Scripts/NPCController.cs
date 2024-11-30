using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// SHOULD NOT attack player at all, should only move left and right and turn when a wall has been hit
/// </summary>
public class NPCController : MonoBehaviour
{
    [SerializeField] protected int hp = 10;
    [SerializeField] protected int maxHP = 10;
    [SerializeField] protected float moveSpeed = 1;
    protected Vector2 moveDir = Vector2.zero;

    void Start()
    {
        moveDir = new Vector2(1, 0);
        GetComponent<Rigidbody2D>().linearVelocityY = -0.1f;
    }

    protected virtual void FixedUpdate()
    {
        // check if dead
        if (hp <= 0)
        {
            GetComponent<Animator>().SetBool("isDead", true);
            return;
        }

        GetComponent<Rigidbody2D>().AddForceX(moveDir.x * 100 * Time.deltaTime, ForceMode2D.Impulse);
        GetComponent<Animator>().SetBool("isWalking", true);

        GetComponent<SpriteRenderer>().flipX = moveDir.x < 0;
        ChangeDirection();

        // min max the velocity to only go up to 5
        if (GetComponent<Rigidbody2D>().linearVelocityX > 0)
            GetComponent<Rigidbody2D>().linearVelocityX = math.min(GetComponent<Rigidbody2D>().linearVelocityX, moveSpeed);
        else
            GetComponent<Rigidbody2D>().linearVelocityX = math.max(GetComponent<Rigidbody2D>().linearVelocityX, -moveSpeed);

        if (GetComponent<Rigidbody2D>().linearVelocityY > 0)
            GetComponent<Rigidbody2D>().linearVelocityY = math.min(GetComponent<Rigidbody2D>().linearVelocityY, moveSpeed);
        else
            GetComponent<Rigidbody2D>().linearVelocityY = math.max(GetComponent<Rigidbody2D>().linearVelocityY, -moveSpeed);
    }

    protected virtual void ChangeDirection()
    {
        RaycastHit2D rayHit = Physics2D.Raycast((Vector2)transform.position, moveDir, 1.1f, LayerMask.NameToLayer("Enemy"));
        Debug.DrawLine((Vector2)transform.position, (Vector2)transform.position + (moveDir * 1.1f), Color.green, 0.1f);

        if (rayHit && rayHit.collider.CompareTag("Wall"))
        {
            moveDir.x = -moveDir.x;
        }
    }
}
