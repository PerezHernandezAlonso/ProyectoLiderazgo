using UnityEngine;

public class CargaDeBloques : MonoBehaviour
{
    public GameObject Spawner;
    public GameObject[] pisos;

    [SerializeField] bool spawnearPisos = true;
    [SerializeField] float intervaloEntreSpawnDePisos = 1f;
    [SerializeField] GameObject suelo;
    [SerializeField] int MaximosPisos = 10;

    private float timer;

    private void Start()
    {
        Destroy(suelo);
    }

    private void Update()
    {
        if (!spawnearPisos) return;

        timer += Time.deltaTime;

        if (timer >= intervaloEntreSpawnDePisos)
        {
            timer = 0f;
            ComprobarYSpawnear();
        }
    }

    void ComprobarYSpawnear()
    {
        int pisosEnEscena = GameObject.FindGameObjectsWithTag("Piso").Length;

        if (pisosEnEscena < MaximosPisos)
        {
            DefinirPisoSpawn();
        }
    }

    public void DefinirPisoSpawn()
    {
        int r = Random.Range(0, pisos.Length);
        SpawnearPiso(pisos[r]);
    }

    private void SpawnearPiso(GameObject piso)
    {
        Instantiate(piso, Spawner.transform.position, Spawner.transform.rotation);
    }
}
