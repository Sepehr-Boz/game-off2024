using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int hp = 10;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float moveSpeed = 1;
    [SerializeField] protected float visionRadius = 5;
    [SerializeField] protected float attackCooldown = 0.05f;

    // reference to view
    [SerializeField] protected EnemyView view;



    protected Vector2 moveDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        if (!view)
            view = GetComponentInChildren<EnemyView>();
        view.gameObject.GetComponent<CircleCollider2D>().radius = visionRadius;

        moveDir = new Vector2(1, 0);
        GetComponent<Rigidbody2D>().linearVelocityY = -0.1f;
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (IsFalling())
            return;

        if (view.player != null)
        {
            GetComponent<Rigidbody2D>().AddForceX((view.player.transform.position.x - transform.position.x) * 100 * moveSpeed * Time.deltaTime);
            //GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position +
            //(new Vector2(view.player.transform.position.x - transform.position.x, 0) * moveSpeed * Time.deltaTime));
        }
        // GetComponent<Rigidbody2D>().linearVelocityX = (view.player.transform.position.x - transform.position.x) * moveSpeed * Time.deltaTime;
        else
        {
            GetComponent<Rigidbody2D>().AddForceX(moveDir.x * 100 * moveSpeed * Time.deltaTime, ForceMode2D.Force);
            // GetComponent<Rigidbody2D>().linearVelocityX = moveDir.x * moveSpeed * Time.deltaTime;
            // GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + (moveDir * moveSpeed * Time.deltaTime));
            // check if collided with a vertical wall, if so then change the x direction to the opposite
            // throw out raycast in the direction moving in to see if hit wall/about to hit wall
            ChangeDirection();
        }
    }

    protected bool IsFalling()
    {
        return GetComponent<Rigidbody2D>().linearVelocityY < 0;
        // RaycastHit2D rayHit = Physics2D.Raycast((Vector2)transform.position + Vector2.down, Vector2.down, 1.1f);
        // return (rayHit && rayHit.transform.CompareTag("Wall"));
    }

    protected virtual void JumpBack()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveDir.x * moveSpeed, 1) * moveSpeed, ForceMode2D.Impulse);
    }

    protected virtual void ChangeDirection()
    {
        RaycastHit2D rayHit = Physics2D.Raycast((Vector2)transform.position + moveDir, moveDir, 0.1f, LayerMask.NameToLayer("Enemy"));
        print(rayHit.collider.gameObject.name);
        Debug.DrawLine((Vector2)transform.position + moveDir, (Vector2)transform.position + (moveDir * 1.1f), Color.green, 0.1f);

        if (rayHit && rayHit.collider.CompareTag("Wall"))
            moveDir.x = -moveDir.x;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            JumpBack();
    }
}
