using UnityEngine;

public class SetNewSpawn : MonoBehaviour
{
    public CargaDeBloques cargaDeBloques;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cargaDeBloques = FindAnyObjectByType<CargaDeBloques>();
        cargaDeBloques.Spawner = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
