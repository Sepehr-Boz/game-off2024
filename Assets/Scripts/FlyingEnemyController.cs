using System;
using System.Linq;
using UnityEngine;

public class FlyingEnemyController : EnemyController
{

    protected override void Start()
    {
        base.Start();
        moveDir = new Vector2(1, -1);
    }
    protected override void FixedUpdate()
    {
        if (view.player != null)
        {
            moveDir = view.player.transform.position - transform.position;
            GetComponent<Rigidbody2D>().MovePosition(
                (transform.position + (view.player.transform.position - transform.position))
            * moveSpeed * Time.deltaTime);
        }
        else
        {
            GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + (moveDir * moveSpeed * Time.deltaTime));
            // check if collided with a vertical wall, if so then change the x direction to the opposite
            // throw out raycast in the direction moving in to see if hit wall/about to hit wall
            ChangeDirection();
        }

        GetComponent<SpriteRenderer>().flipX = moveDir.x >= 0;
    }

    protected override void ChangeDirection()
    {
        // throw out 4 new raycasts to see which direction then reverse the moveDir, ignore the enemies layer
        RaycastHit2D upRay = Physics2D.Raycast((Vector2)transform.position, Vector2.up, 1.1f, LayerMask.NameToLayer("Enemy"));
        RaycastHit2D downRay = Physics2D.Raycast((Vector2)transform.position, Vector2.down, 1.1f, LayerMask.NameToLayer("Enemy"));
        RaycastHit2D leftRay = Physics2D.Raycast((Vector2)transform.position, Vector2.left, 1.1f, LayerMask.NameToLayer("Enemy"));
        RaycastHit2D rightRay = Physics2D.Raycast((Vector2)transform.position, Vector2.right, 1.1f, LayerMask.NameToLayer("Enemy"));
        // Debug.DrawRay((Vector2)transform.position + Vector2.up, Vector2.up, Color.red);
        // Debug.DrawRay((Vector2)transform.position + Vector2.down, Vector2.down, Color.green);
        // Debug.DrawRay((Vector2)transform.position + Vector2.left, Vector2.left, Color.yellow);
        // Debug.DrawRay((Vector2)transform.position + Vector2.right, Vector2.right, Color.blue);

        // try
        // {
        //     print("UP" + upRay.collider.gameObject.name);
        //     print("DOWN" + downRay.collider.gameObject.name);
        //     print("LEFT" + leftRay.collider.gameObject.name);
        //     print("RIGHT" + rightRay.collider.gameObject.name);
        // }
        // catch (Exception) { };

        if (upRay && upRay.collider.CompareTag("Wall"))
            moveDir.y = -1;
        if (downRay && downRay.collider.CompareTag("Wall"))
            moveDir.y = 1;

        if (leftRay && leftRay.collider.CompareTag("Wall"))
            moveDir.x = 1;
        if (rightRay && rightRay.collider.CompareTag("Wall"))
            moveDir.x = -1;

        // StartCoroutine(BoostMoveAtStart());
    }

    protected override void JumpBack()
    {
        GetComponent<Rigidbody2D>().AddForce(-moveDir * 25 * moveSpeed, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            JumpBack();
        else if (other.gameObject.CompareTag("Wall"))
            ChangeDirection();
    }
}
