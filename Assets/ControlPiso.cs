using UnityEngine;

public class ControlPiso : MonoBehaviour
{
    [Header("Tiempo en segundos antes de destruir el piso")]
    public float DuracionDePiso = 11;

    void Start()
    {
        // Destruye este piso después de X segundos
        //Destroy(transform.parent.gameObject, DuracionDePiso);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Piso"))
        {
            Debug.Log("despawneandoMapa");
            Destroy(collision.gameObject);
        }
    }
    
}
