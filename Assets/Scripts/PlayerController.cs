using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

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
    private bool canFall = false;

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

        if (dir.y < 0 && canFall)
        {
            FallDownPlatform();
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
        canFall = true;
    }

    void OnCollisionExit2D(Collision2D other)
    {
        canFall = false;
    }

    void FallDownPlatform()
    {
        
        IEnumerator DisableForSeconds(float seconds, TilemapCollider2D col)
        {
            col.enabled = false;
            yield return new WaitForSeconds(seconds);
            col.enabled = true;
        }


        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();

        GameObject grid = null;
        foreach (GameObject root in roots)
        {
            if (root.name == "Grid")
            {
                grid = root;
                break;
            }
        }

        foreach (Transform template in grid.transform)
        {
            foreach (Transform layer in template)
            {
                if (!layer.GetComponent<PlatformEffector2D>())
                    continue;

                if (layer.name == "Platforms")
                {
                    StartCoroutine(DisableForSeconds(1f, layer.GetComponent<TilemapCollider2D>()));
                }
            }
        }
    }


    // void FallDownPlatform()
    // {
    //     // check if on top of a platform using a raycast downwards
    //     RaycastHit2D ignoreEnRay = Physics2D.Raycast(transform.position, Vector2.down, 2f, LayerMask.NameToLayer("Enemy"));
    //     Debug.DrawLine(transform.position, (Vector2)transform.position + (Vector2.down * 2f), Color.red, 3f);

    //     print(downRay.collider);

    //     /* if on top of a platform then loop through every tilemap in the scene, get all the platforms and disable their
    //     platform effectors for 1 second until fallen through */
    //     if (downRay && downRay.collider.gameObject.name == "Platforms")
    //     {
    //         GameObject[] objs = SceneManager.GetActiveScene().GetRootGameObjects();
    //         print(objs.Count());
    //         foreach (GameObject obj in objs)
    //         {
    //             if (!obj.GetComponent<PlatformEffector2D>())
    //                 continue;
    //             else
    //             {
    //                 StartCoroutine(DisableForSeconds(1f, obj.GetComponent<TilemapCollider2D>()));
    //             }

    //         }
    //     }


    //     IEnumerator DisableForSeconds(float seconds, TilemapCollider2D obj)
    //     {
    //         obj.enabled = false;
    //         yield return new WaitForSeconds(seconds);
    //         obj.enabled = true;
    //     }
    // }
}
