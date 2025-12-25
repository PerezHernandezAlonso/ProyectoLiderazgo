using UnityEngine;

public class BenjaBullet : MonoBehaviour
{
    [SerializeField] float damage = 1f;
    [SerializeField] float knockback = 5f;

    [SerializeField] GameObject playerHitEffect;

    Vector3 startingPosition;

    private void Start()
    {
        startingPosition = transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.transform.parent.TryGetComponent(out PlayerMovement player))
        {
            Vector3 direction = (player.transform.position - startingPosition).normalized;
            player.AddKnockback(direction.x, knockback);

            Instantiate(playerHitEffect, collision.transform);
            

            Destroy(gameObject);
        }
        else
        {

        }
    }
}
