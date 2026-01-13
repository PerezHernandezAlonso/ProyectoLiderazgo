using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private PlayerMovement.guns collectableGun;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision con: " + other.gameObject.name);

        PlayerMovement playerCollect = other.GetComponentInParent<PlayerMovement>();
        if (playerCollect != null)
        {
            Debug.Log("Colleccionado: "+collectableGun.ToString()+"");
            playerCollect.PickUpGun(collectableGun);
            gameObject.SetActive(false);
        }
    }
}