using System.Collections;
using UnityEngine;


public class CargaDeBloques : MonoBehaviour
{
    public GameObject Spawner;
    public GameObject[] pisos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] bool spawnearPisos = true;

    [SerializeField] float intervaloEntreSpawnDePisos = 3f;

    [SerializeField] GameObject suelo;

    public void DefinirPisoSpawn()
    {
        int r = Random.Range(0, pisos.Length);
        SpawnearPiso(pisos[r]);
    }

    private void SpawnearPiso(GameObject piso)
    {
        Instantiate(piso, Spawner.transform.position, Spawner.transform.rotation);
    }

    private void Start()
    {
        EmpezarASpawnearPisos();
    }

    public void EmpezarASpawnearPisos()
    {
        if (spawnearPisos) 
        {
            StartCoroutine(SpawnearPisosEnBucle()); // esto se llamará al empezar la partida
            Destroy(suelo);
        }
    }

    IEnumerator SpawnearPisosEnBucle()
    {
        yield return new WaitForSeconds(intervaloEntreSpawnDePisos);
        DefinirPisoSpawn();
        if (!spawnearPisos) { yield return null; }
        else { StartCoroutine(SpawnearPisosEnBucle()); }
    }

}
