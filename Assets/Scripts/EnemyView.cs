using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnemyView : MonoBehaviour
{
    public GameObject player = null;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            player = other.gameObject;
    }
}
