using UnityEngine;

public class BenjaBullet : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float knockback = 5f;
    [SerializeField] bool isEnd;

    [SerializeField] GameObject playerHitEffect;

    Vector3 startingPosition;

    private void Start()
    {
        startingPosition = transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        PlayerHealthManager healthManager =
            collision.GetComponentInParent<PlayerHealthManager>();
        if (healthManager == null) return;

        Vector3 direction = (player.transform.position - startingPosition).normalized;
        player.AddKnockback(direction.x, knockback);

        if (playerHitEffect != null)
            Instantiate(playerHitEffect, player.transform);

        healthManager.LoseHealth(damage);
        if(!isEnd)Destroy(gameObject);
    }
}
