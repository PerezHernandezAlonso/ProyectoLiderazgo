using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public abstract class Gun : MonoBehaviour
{
    //Which player is currently holding the gun. If no players are holding it, it should be null
    public GameObject assignedPlayer;
    //The offset for the gun's position, so that the bullet comes out of the barrel. ASSUMING THE PLAYER FACES RIGHT
    public Vector2 holdingOffset;
    //The object that will act as a bullet when th egun calls the Shoot function
    public GameObject bullet;
    //The current ammunition the gun has
    public int ammo;
    //The total ammunition the gun has
    public int maxAmmo;
    //Whether or not the gun should keep firing with the fire button held
    public bool automatic;
    //How long it will take to fire another shot. This is calculated in seconds
    public float baseCoolDown = 0f; 
    public float coolDown;
    //In order to know where the player is looking at
    public bool isFacingRight;
    //When the gun has no assigned player, the radius at which the gun will be automatically assigned to a player
    public float pickUpRadius;
    private CircleCollider2D pickupTrigger;


    private InputManagerForPlayer playerInputManager;

    protected virtual void Awake()
    {
        pickupTrigger = GetComponent<CircleCollider2D>();

        if (pickupTrigger == null)
            pickupTrigger = gameObject.AddComponent<CircleCollider2D>();

        pickupTrigger.isTrigger = true;

        UpdatePickupRadius();
    }
    private void UpdatePickupRadius()
    {
        if (pickUpRadius > 0f)
        {
            pickupTrigger.enabled = true;
            pickupTrigger.radius = pickUpRadius;
        }
        else
        {
            pickupTrigger.radius = 0f;
            pickupTrigger.enabled = false;
        }
    }
    public void Update()
    {
        //This won't work with multiple players, still trying to adjust this. 

        if (assignedPlayer == null) return;

        coolDown -= Time.deltaTime;
        transform.position = isFacingRight ? assignedPlayer.transform.position + (Vector3)holdingOffset :
            assignedPlayer.transform.position + new Vector3(-holdingOffset.x, holdingOffset.y, 0f);

        //Not working, update with new input system

        /*if (automatic && Input.GetButton("Shoot") && coolDown <= 0f)
        {
            Fire();
        }

        else if (!automatic && /*Input.GetButtonDown("Shoot") && coolDown <= 0f)
        {
            Fire();
        }*/

    }
    [ContextMenu("Force Fire Gun")]
    private void Fire()
    {
        if (ammo <= 0)
        {
            Destroy(gameObject);
            return;
        }

        Shoot();
        ammo--;
        coolDown = baseCoolDown;
    }

    /// <summary>
    /// Will shoot a bullet in the direction of the gun. 
    /// The default values are 1 for all bullet stats except for the gravity factor.
    /// </summary>
    public virtual void Shoot()
    {
        Vector3 offset = new Vector3(holdingOffset.x * (isFacingRight ? 1 : -1), holdingOffset.y, 0);
        GameObject currentBullet = Instantiate(bullet, transform.position + offset, transform.rotation);
        Bullet bulletScript = currentBullet.GetComponent<Bullet>();
        if (bulletScript == null) { Debug.LogError("No bullet script detected in bullet"); }
        bulletScript.damage = 1;
        bulletScript.direction = isFacingRight? new Vector2 (1, 0) : new Vector2 (-1, 0);
        bulletScript.gravityFactor = 0;
        bulletScript.speed = 1;
    }

    //Checks for picking up the weapon
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(assignedPlayer != null || collision.CompareTag("Player")) return;

        assignedPlayer = collision.gameObject;
    }

}
