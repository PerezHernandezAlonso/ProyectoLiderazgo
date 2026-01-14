using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadScene()
    {
        Debug.Log("Carga escena: Gamescene");
        SceneManager.LoadScene("GameScene");
    }
}
