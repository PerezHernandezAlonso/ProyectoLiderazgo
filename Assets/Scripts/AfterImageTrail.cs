using UnityEngine;

public class AfterimageTrail : MonoBehaviour
{
    public GameObject afterimagePrefab;
    public float spawnRate = 0.05f;
    public float lifetime = 0.3f;
    public Color trailColor = Color.cyan;

    Rigidbody2D rb;
    float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        
        if (rb.linearVelocity.sqrMagnitude < 0.1f)
            return;

        timer += Time.fixedDeltaTime;
        if (timer >= spawnRate)
        {
            timer = 0f;
            SpawnAfterimage();
        }
    }

    void SpawnAfterimage()
    {
        GameObject ghost = Instantiate(
            afterimagePrefab,
            transform.position,
            transform.rotation
        );

        var mat = ghost.GetComponent<Renderer>().material;
        mat.SetColor("_TrailColor", trailColor);

        Destroy(ghost, lifetime);
    }
}
