using UnityEngine;


public class CargaDeBloques : MonoBehaviour
{
    public GameObject Spawner;
    public GameObject[] pisos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
