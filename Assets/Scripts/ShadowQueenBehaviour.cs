using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ShadowQueenBehaviour : EnemyController
{

    private int phase = 1;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(Attack1Pattern());
    }
    protected override void FixedUpdate()
    {

        if (hp <= 0)
        {
            GetComponent<Animator>().SetBool("isDead", true);
            return;
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

    protected override void ChangeDirection()
    {
        RaycastHit2D rayHit = Physics2D.Raycast((Vector2)transform.position - new Vector2(0, 1), moveDir, 2f, LayerMask.NameToLayer("Enemy"));
        // print(rayHit.collider.gameObject.name);
        Debug.DrawLine((Vector2)transform.position - new Vector2(0, 1), (Vector2)transform.position + moveDir, Color.green, 0.1f);

        if (rayHit && rayHit.collider.CompareTag("Wall"))
        {
            moveDir.x = -moveDir.x;
            // StartCoroutine(BoostMoveAtStart());
        }
        else if (rayHit && rayHit.collider.CompareTag("Player"))
        {
            switch (phase)
            {
                case 1:
                    moveDir.x = -moveDir.x;
                    break;
                case 2:
                    // print("x");
                    GetComponent<Collider2D>().isTrigger = true;
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                    break;
                default:
                    break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
        else if (other.gameObject.CompareTag("Wall"))
            ChangeDirection();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider2D>().isTrigger = false;
            GetComponent<Rigidbody2D>().gravityScale = 1;
        }
    }

    private IEnumerator Attack1Pattern()
    {
        // move TO player, attack, then turn around
        // turn around on walls hit
        while (((float)hp / (float)maxHP) >= 0.7f)
        {
            if (view.player != null)
            {
                GetComponent<Animator>().SetBool("isAttacking", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("isAttacking", false);
                GetComponent<Animator>().SetBool("isWalking", true);
            }

            GetComponent<Rigidbody2D>().AddForceX(moveDir.x * moveSpeed * 100 * Time.deltaTime);
            GetComponent<SpriteRenderer>().flipX = moveDir.x < 0;

            ChangeDirection();

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(Attack2Pattern());
    }

    private IEnumerator Attack2Pattern()
    {
        phase = 2;
        // move THROUGH player, attacking while dashing through, then turn around on walls hit
        while ((float)hp / (float)maxHP >= 0.3f)
        {
            if (view.player != null)
            {
                GetComponent<Animator>().SetBool("isAttacking", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("isAttacking", false);
                GetComponent<Animator>().SetBool("isWalking", true);
            }

            GetComponent<Rigidbody2D>().AddForceX(moveDir.x * moveSpeed * 100 * Time.deltaTime);
            GetComponent<SpriteRenderer>().flipX = moveDir.x < 0;
            ChangeDirection();

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(Attack3Pattern());
    }

    private IEnumerator Attack3Pattern()
    {
        phase = 3;
        // play laser attack animation, on the end laser shot spawn in some colliders at where the lasers hit
        // turn around when hit walls

        phase = 2;
        // move THROUGH player, attacking while dashing through, then turn around on walls hit
        while ((float)hp / (float)maxHP < 0.3f)
        {
            if (view.player != null)
            {
                GetComponent<Animator>().SetBool("isRanged", true);
            }
            else
            {
                GetComponent<Animator>().SetBool("isRanged", false);
                GetComponent<Animator>().SetBool("isWalking", true);
            }

            // keep facing the player
            GetComponent<SpriteRenderer>().flipX = view.player ?? view.player.transform.position.x < transform.position.x;
            ChangeDirection();

            yield return new WaitForEndOfFrame();
        }
    }
}
