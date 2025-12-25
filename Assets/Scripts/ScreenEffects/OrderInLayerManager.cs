using UnityEngine;

public class OrderInLayerManager : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRenderer.sortingOrder = -(int)(gameObject.transform.position.y * 8f);
    }
}
