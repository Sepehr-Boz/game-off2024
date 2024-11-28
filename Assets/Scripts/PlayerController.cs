using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float MoveVelocity = 7.5f;
    [SerializeField] private float JumpVelocity = 10f;
    [SerializeField] private float maxRayDist = 1f;
    [SerializeField] private bool DebugRays = true;

    private Vector2 dir;
    private Vector2 rayDir = Vector2.down;
    private bool isGrounded = true;
    private bool canFall = false;
    private RaycastHit2D[] hits;
    private string[] rayLayerNames = {"Wall", "Enemy"};
    private int[] rayLayers;

    private new Rigidbody2D rigidbody2D;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        rayLayers = new int[rayLayerNames.Length];
        for (int i = 0; i < rayLayerNames.Length; i++)
        {
            rayLayers[i] = LayerMask.NameToLayer(rayLayerNames[i]);
        }
    }

    private Vector2 getInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    // Update is called once per frame
    void Update()
    {
        dir = getInput();
    }

    // fixed update (kinda for physics)
    void FixedUpdate()
    {
        // move
        rigidbody2D.linearVelocityX = dir.x * MoveVelocity;
        // raycast down to check if grounded
        hits = Physics2D.RaycastAll(transform.position, rayDir, maxRayDist);
        if (DebugRays)
        {
            Debug.DrawLine(transform.position, transform.position + (Vector3) rayDir * maxRayDist, Color.red);
        }
        foreach (RaycastHit2D hit in hits)
        {
            foreach (int layer in rayLayers)
            {
                if (hit.collider.gameObject.layer == layer)
                {
                    isGrounded = true;
                    canFall = true;
                }
            }
        }

        // jump
        if (isGrounded && dir.y > 0)
        if (isGrounded && dir.y > 0)
        {
            rigidbody2D.linearVelocityY = dir.y * JumpVelocity;
            isGrounded = false;
        }

        if (dir.y < 0 && canFall)
        {
            FallDownPlatform();
            canFall = false;
        }
    }

    void FallDownPlatform()
    {
        IEnumerator DisableForSeconds(float seconds, Collider2D col)
        {
            col.enabled = false;
            yield return new WaitForSeconds(seconds);
            col.enabled = true;
        }


        StartCoroutine(DisableForSeconds(0.5f, GetComponent<Collider2D>()));




        // IEnumerator DisableForSeconds(float seconds, TilemapCollider2D col)
        // {
        //     col.enabled = false;
        //     yield return new WaitForSeconds(seconds);
        //     col.enabled = true;
        // }


        // GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();

        // GameObject grid = null;
        // foreach (GameObject root in roots)
        // {
        //     if (root.name == "Grid")
        //     {
        //         grid = root;
        //         break;
        //     }
        // }

        // foreach (Transform template in grid.transform)
        // {
        //     foreach (Transform layer in template)
        //     {
        //         if (!layer.GetComponent<PlatformEffector2D>())
        //             continue;

        //         if (layer.name == "Platforms")
        //         {
        //             StartCoroutine(DisableForSeconds(1f, layer.GetComponent<TilemapCollider2D>()));
        //         }
        //     }
        // }
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
    // void OnCollisionEnter2D(Collision2D other)
    // {
    //     isGrounded = true;
    // void OnCollisionEnter2D(Collision2D other)
    // {
    //     isGrounded = true;
    // }
}
