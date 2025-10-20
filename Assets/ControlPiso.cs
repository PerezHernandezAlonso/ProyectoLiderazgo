using UnityEngine;

public class ControlPiso : MonoBehaviour
{
    [Header("Velocidad de descenso (unidades por segundo)")]
    public float velocidad = 1f;

    [Header("Altura m�nima antes de destruir el bloque")]
    public float alturaLimite = -10f;

    [Header("Referencia al script CargaDeBloques (se busca autom�ticamente)")]
    public CargaDeBloques cargaDeBloques;

    void Start()
    {
        // Buscar autom�ticamente el script CargaDeBloques en la escena
        if (cargaDeBloques == null)
            cargaDeBloques = FindAnyObjectByType<CargaDeBloques>();
    }

    void Update()
    {
        // Movimiento descendente constante
        transform.position += Vector3.down * velocidad * Time.deltaTime;

        // Si el bloque baja m�s all� del l�mite, se destruye
        if (transform.position.y <= alturaLimite)
        {
            // Llamar a la funci�n del otro script
            if (cargaDeBloques != null)
            {
                cargaDeBloques.DefinirPisoSpawn();
            }
            else
            {
                Debug.LogWarning("No se ha asignado el script 'CargaDeBloques'.");
            }

            Destroy(gameObject);
        }
    }
}
