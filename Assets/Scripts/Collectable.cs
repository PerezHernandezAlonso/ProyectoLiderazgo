using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private PlayerMovement.guns collectableGun;
    [SerializeField] private bool aleatorio;
    [SerializeField] private bool destruirAlFinal;

    [SerializeField] private SpriteRenderer spriteRenderer; // Asignar en el inspector
    [SerializeField] private Sprite[] gunSprites; // Array de sprites en el mismo orden que el enum
    [SerializeField, Range(0f, 1f)] private float firstGunDropRate = 0.8f; // Editable en el editor

    void Awake()
    {
        // Si aleatorio está activado, selecciona un arma al azar
        if (aleatorio && gunSprites.Length > 0)
        {
            float rand = Random.Range(0f, 1f);

            // % para la primera arma (la pistola)
            if (rand <= firstGunDropRate)
            {
                collectableGun = (PlayerMovement.guns)1;
            }
            else
            {
                // Elige una de las otras armas al azar
                int otherIndex = Random.Range(1, gunSprites.Length);
                collectableGun = (PlayerMovement.guns)otherIndex;
            }
        }

        // Asignar el sprite correspondiente
        if (spriteRenderer != null && gunSprites.Length > 0)
        {
            spriteRenderer.sprite = gunSprites[(int)collectableGun];
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Collision con: " + other.gameObject.name);

        PlayerMovement playerCollect = other.GetComponentInParent<PlayerMovement>();
        if (playerCollect != null)
        {
            //Debug.Log("Colleccionado: "+collectableGun.ToString()+"");
            playerCollect.PickUpGun(collectableGun);
            if(destruirAlFinal)
            gameObject.SetActive(false);
        }
    }
}