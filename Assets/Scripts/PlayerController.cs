using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float MoveVelocity = 7.5f;
    [SerializeField] private float JumpVelocity = 10f;
    [SerializeField] private float DropDisableTime = 0.3f;
    [SerializeField] private float MaxRayDistance = 0.6f;
    [SerializeField] private float DownRayOffset = 0.35f;
    [SerializeField] private bool DebugRays = true;

    private Vector2 dir;
    private Vector2 rayDir = Vector2.down;
    private bool isGrounded = true;
    private bool canFall = false;
    private RaycastHit2D[] hits;
    private string[] rayLayerNames = {"Wall", "Enemy", "Platform"};
    private int platformLayer;
    private int[] rayLayers;
    private int hitLayer;

    private new Rigidbody2D rigidbody2D;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        // convert layer names to layers
        platformLayer = LayerMask.NameToLayer("Platform");

        rayLayers = new int[rayLayerNames.Length];
        for (int i = 0; i < rayLayerNames.Length; i++) rayLayers[i] = LayerMask.NameToLayer(rayLayerNames[i]);
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

    // raycast all with debug line if enabled
    private RaycastHit2D[] debugRayCastAll(Vector3 pos, Vector2 dir, float maxDist)
    {
        if (DebugRays) Debug.DrawLine(pos, pos + (Vector3) dir * maxDist, Color.red);// debug line
        return Physics2D.RaycastAll((Vector2) pos, dir, maxDist);// get all hits from raycast
    }

    // compares hit collider layer to whitelist layer array (returns if platform below)
    private bool checkBelow(Vector2 offset)
    {
        foreach (RaycastHit2D hit in debugRayCastAll((Vector2) transform.position + offset, rayDir, MaxRayDistance))
        {
            // check hit layer
            hitLayer = hit.collider.gameObject.layer;
            foreach (int layer in rayLayers)
            {
                // valid layer player can jump off
                if (hitLayer == layer)
                {
                    isGrounded = true;
                    // if layer is platform allow falling through
                    if (hitLayer == platformLayer)
                    {
                        canFall = true;
                        return true;
                    }
                    break;
                }
            }
            if (isGrounded) break;// if already grounded no point checking other hits
        }
        return false;
    }

    // fixed update (kinda for physics)
    void FixedUpdate()
    {
        // move
        rigidbody2D.linearVelocityX = dir.x * MoveVelocity;
        
        // start with disabled to stop in-air jump
        isGrounded = false;
        canFall = false;

        // ray cast below to check for ground/platform
        bool platL = checkBelow(new Vector2(-DownRayOffset, 0));// down left
        bool platR = checkBelow(new Vector2(DownRayOffset, 0));// down right
        bool platM = checkBelow(Vector2.zero);// down middle

        // only let player drop if pretty sure big enough platform exists to fall through
        if (platL && platM && platR)
        {
            canFall = true;
        }

        // jump
        if (isGrounded && dir.y > 0)
        {
            rigidbody2D.linearVelocityY = dir.y * JumpVelocity;
        }

        // drop through platform
        if (canFall && dir.y < 0)
        {
            FallDownPlatform();
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
        
        StartCoroutine(DisableForSeconds(DropDisableTime, GetComponent<Collider2D>()));




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
