using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // GetComponent<PlayerController>().TakeDamage();
            Destroy(this.gameObject);
        }
        else if (other.gameObject.CompareTag("Wall"))
            Destroy(this.gameObject);
    }
}
