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
            GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position +
            (new Vector2(view.player.transform.position.x - transform.position.x, 0) * moveSpeed * Time.deltaTime));
        }
        // GetComponent<Rigidbody2D>().linearVelocityX = (view.player.transform.position.x - transform.position.x) * moveSpeed * Time.deltaTime;
        else
        {
            // GetComponent<Rigidbody2D>().linearVelocityX = moveDir.x * moveSpeed * Time.deltaTime;
            GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + (moveDir * moveSpeed * Time.deltaTime));
            // check if collided with a vertical wall, if so then change the x direction to the opposite
            // throw out raycast in the direction moving in to see if hit wall/about to hit wall
            ChangeDirection();
        }
    }

    protected override void ChangeDirection()
    {
        // throw out 4 new raycasts to see which direction then reverse the moveDir
        RaycastHit2D upRay = Physics2D.Raycast((Vector2)transform.position + Vector2.up, Vector2.up, 0.1f);
        RaycastHit2D downRay = Physics2D.Raycast((Vector2)transform.position + Vector2.down, Vector2.down, 0.1f);
        RaycastHit2D leftRay = Physics2D.Raycast((Vector2)transform.position + Vector2.left, Vector2.left, 0.1f);
        RaycastHit2D rightRay = Physics2D.Raycast((Vector2)transform.position + Vector2.right, Vector2.right, 0.1f);

        if (upRay && upRay.transform.CompareTag("Wall"))
            moveDir.y = -1;
        else if (downRay && downRay.transform.CompareTag("Wall"))
            moveDir.y = 1;

        if (leftRay && leftRay.transform.CompareTag("Wall"))
            moveDir.x = 1;
        else if (rightRay && rightRay.transform.CompareTag("Wall"))
            moveDir.x = -1;
    }
}
