using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    public GameObject shot;
    private bool canShoot = true;

    protected override void FixedUpdate()
    {
        // check if dead
        if (hp <= 0)
        {
            GetComponent<Animator>().SetBool("isDead", true);
            return;
        }

        if (view.player != null)
        {
            // if see player then face towards the player, stay still and shoot
            GetComponent<SpriteRenderer>().flipX = view.player.transform.position.x < transform.position.x;

            GetComponent<Animator>().SetBool("isWalking", false);
            GetComponent<Animator>().SetBool("isAttacking", true);

            // send out projectile towards player in direction of player
            if (canShoot)
            {
                GameObject newShot = Instantiate(shot, position: transform.position, rotation: Quaternion.identity);
                newShot.GetComponent<BulletBehaviour>().damage = this.damage;
                newShot.GetComponent<Rigidbody2D>().AddForceX(view.player.transform.position.x < transform.position.x ? -10 : 10, ForceMode2D.Impulse);
                StartCoroutine(Cooldown());
            }
        }
        else
        {
            GetComponent<Animator>().SetBool("isAttacking", false);
            GetComponent<Animator>().SetBool("isWalking", true);

            GetComponent<Rigidbody2D>().AddForceX(moveDir.x * 100 * moveSpeed * Time.deltaTime, ForceMode2D.Force);
            ChangeDirection();
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

    private IEnumerator Cooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(attackCooldown);
        canShoot = true;
    }
}
