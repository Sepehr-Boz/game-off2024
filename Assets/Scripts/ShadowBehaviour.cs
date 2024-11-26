using UnityEngine;

public class ShadowBehaviour : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        // copy the parent sprite, if none then dont change as it means that this is a shadow enemy
        SpriteRenderer parentRenderer = transform.parent.GetComponent<SpriteRenderer>();

        if (parentRenderer.sprite == null)
            return;
        else
        {
            SpriteRenderer sp = GetComponent<SpriteRenderer>();

            sp.sprite = parentRenderer.sprite;
            sp.flipX = parentRenderer.flipX;
            sp.flipY = parentRenderer.flipY;
            sp.color = Color.black;
        }
    }
}
