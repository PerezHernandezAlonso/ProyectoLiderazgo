using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(CircleCollider2D))]
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
    public float damage;

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
        //Will collide with all the players unless the collider is well set
        if (collision.CompareTag("Player"))
        {
            //Here's where the character's method to take damage will play

            //Now, it will be destroyed
            Destroy(gameObject, 0f);
        }
        
    }
}
