using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour
{
    //The bullet's rigidbody
    Rigidbody2D rb2D;
    //The direction IN TWO DIMENSION in which the bullet will start travelling
    public Vector2 direction;
    //The linear speed of the bullet
    public float speed;
    //The factor by which gravity will affect the bullet
    //(ex: 1 = normal gravity. 2 = double the gravity. 0 = no gravity)
    public float gravityFactor = 0f;
    //The damage of the weapon
    public int damage = 1;
    public float knockback = 5f;

    [SerializeField] GameObject playerHitEffect;
    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.gravityScale = gravityFactor;

    }

    private void Start()
    {
        direction.Normalize();
        rb2D.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Object collided with something");
        if  (collision.CompareTag("Player"))
        {
            Debug.Log("Collision worked");
            //Here's where the character's method to take damage will play
            PlayerHealthManager healthManager = collision.gameObject.GetComponent<PlayerHealthManager>();
            collision.transform.TryGetComponent(out PlayerMovement player);
            player.AddKnockback(direction.x, knockback);

            Instantiate(playerHitEffect, collision.transform);

            //Destroy(gameObject);
            healthManager.LoseHealth(damage);
            //Now, it will be destroyed
            Destroy(gameObject, 0f);
        }
    }
}
