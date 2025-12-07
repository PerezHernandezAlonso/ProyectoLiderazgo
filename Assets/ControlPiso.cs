using UnityEngine;

public class ControlPiso : MonoBehaviour
{
    [Header("Velocidad de descenso (unidades por segundo)")]
    public float velocidad = 1f;

    [Header("Altura mínima antes de destruir el bloque")]
    public float alturaLimite = -10f;

    [Header("Referencia al script CargaDeBloques (se busca automáticamente)")]
    public CargaDeBloques cargaDeBloques;

    void Start()
    {
        // Buscar automáticamente el script CargaDeBloques en la escena
        if (cargaDeBloques == null)
            cargaDeBloques = FindAnyObjectByType<CargaDeBloques>();
    }

    void Update()
    {
        // Movimiento descendente constante
        transform.position += Vector3.down * velocidad * Time.deltaTime;

        // Si el bloque baja más allá del límite, se destruye
        if (transform.position.y <= alturaLimite)
        {
            // Llamar a la función del otro script
            //if (cargaDeBloques != null)
            //{
            //    cargaDeBloques.DefinirPisoSpawn();
            //}
            //else
            //{
            //    Debug.LogWarning("No se ha asignado el script 'CargaDeBloques'.");
            //}

            Destroy(transform.parent.gameObject);
        }
    }
}
