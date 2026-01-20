using UnityEngine;

public class PlataformDrops : MonoBehaviour
{
    public Animator plataformController;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        plataformController.SetTrigger("onDrops");
    }
}
