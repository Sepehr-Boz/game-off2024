using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int hp = 10;
    [SerializeField] protected int maxHP = 10;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float moveSpeed = 1;
    [SerializeField] protected float visionRadius = 5;
    [SerializeField] protected float attackCooldown = 0.05f;

    // reference to view
    [SerializeField] protected EnemyView view;

    // // reference to sprites for different animations
    // [Header("Sprites")]

    // [SerializeField] protected Sprite[] idle;
    // [SerializeField] protected Sprite[] falling, walking, attacking;



    protected Vector2 moveDir = Vector2.zero;
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
        // check if dead
        if (hp <= 0)
        {
            GetComponent<Animator>().SetBool("isDead", true);
            return;
        }

        // print(moveDir);
        // StopAllCoroutines();

        if (IsFalling())
        {
            GetComponent<Animator>().SetBool("isFalling", true);
            // if (GetComponent<Rigidbody2D>().linearVelocityY <= -1)
            //     StartCoroutine(IterateThroughSprites(falling));
            return;
        }
        else
            GetComponent<Animator>().SetBool("isFalling", false);
        // StartCoroutine(IterateThroughSprites(idle));

        if (view.player != null)
        {
            GetComponent<Rigidbody2D>().AddForceX((view.player.transform.position.x - transform.position.x) * 100 * moveSpeed * Time.deltaTime);
            GetComponent<Animator>().SetBool("isAttacking", true);

            // if see player then face towards the player, stay still and shoot
            GetComponent<SpriteRenderer>().flipX = view.player.transform.position.x < transform.position.x;
            // StartCoroutine(IterateThroughSprites(attacking));
            //GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position +
            //(new Vector2(view.player.transform.position.x - transform.position.x, 0) * moveSpeed * Time.deltaTime));
        }
        // GetComponent<Rigidbody2D>().linearVelocityX = (view.player.transform.position.x - transform.position.x) * moveSpeed * Time.deltaTime;
        else
        {
            GetComponent<Rigidbody2D>().AddForceX(moveDir.x * 100 * moveSpeed * Time.deltaTime, ForceMode2D.Force);
            GetComponent<Animator>().SetBool("isAttacking", false);
            GetComponent<Animator>().SetBool("isWalking", true);
            // StartCoroutine(IterateThroughSprites(walking));
            // GetComponent<Rigidbody2D>().linearVelocityX = moveDir.x * moveSpeed * Time.deltaTime;
            // GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + (moveDir * moveSpeed * Time.deltaTime));
            // check if collided with a vertical wall, if so then change the x direction to the opposite
            // throw out raycast in the direction moving in to see if hit wall/about to hit wall
            ChangeDirection();

            if (moveDir.x < 0)
                // GetComponent<Animator>().SetBool("isLeft", true);
                GetComponent<SpriteRenderer>().flipX = true;
            else
                // GetComponent<Animator>().SetBool("isLeft", false);
                GetComponent<SpriteRenderer>().flipX = false;
        }

        // min max the velocity to only go up to 5
        if (GetComponent<Rigidbody2D>().linearVelocityX > 0)
            GetComponent<Rigidbody2D>().linearVelocityX = math.min(GetComponent<Rigidbody2D>().linearVelocityX, 5);
        else
            GetComponent<Rigidbody2D>().linearVelocityX = math.max(GetComponent<Rigidbody2D>().linearVelocityX, -5);

        if (GetComponent<Rigidbody2D>().linearVelocityY > 0)
            GetComponent<Rigidbody2D>().linearVelocityY = math.min(GetComponent<Rigidbody2D>().linearVelocityY, 5);
        else
            GetComponent<Rigidbody2D>().linearVelocityY = math.max(GetComponent<Rigidbody2D>().linearVelocityY, -5);
    }

    protected bool IsFalling()
    {
        return GetComponent<Rigidbody2D>().linearVelocityY < 0;
        // RaycastHit2D rayHit = Physics2D.Raycast((Vector2)transform.position + Vector2.down, Vector2.down, 1.1f);
        // return (rayHit && rayHit.transform.CompareTag("Wall"));
    }

    protected virtual void JumpBack()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveDir.x, 1) * moveSpeed, ForceMode2D.Impulse);
    }

    protected virtual void ChangeDirection()
    {
        RaycastHit2D rayHit = Physics2D.Raycast((Vector2)transform.position, moveDir, 1.1f, LayerMask.NameToLayer("Enemy"));
        // print(rayHit.collider.gameObject.name);
        Debug.DrawLine((Vector2)transform.position, (Vector2)transform.position + moveDir, Color.green, 0.1f);

        if (rayHit && rayHit.collider.CompareTag("Wall"))
        {
            moveDir.x = -moveDir.x;
            // StartCoroutine(BoostMoveAtStart());
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            JumpBack();
        }
        else if (other.gameObject.CompareTag("Wall"))
            ChangeDirection();
    }

    protected IEnumerator BoostMoveAtStart()
    {
        moveDir.x *= 2;
        yield return new WaitForSeconds(0.5f);
        moveDir.x /= 2;
    }


    public void Die()
    {
        // stay in death animation for 2 seconds, make collider trigger, then destroy self
        // Destroy(GetComponent<Rigidbody2D>());

        IEnumerator WaitThenFunc(float seconds, Action func)
        {
            yield return new WaitForSeconds(seconds);
            func();
        }

        // disable collider, disable rigidbody to keep in place
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        StartCoroutine(WaitThenFunc(3f, () => Destroy(this.gameObject)));
    }
}
