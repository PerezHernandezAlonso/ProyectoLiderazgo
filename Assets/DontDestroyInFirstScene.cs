using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyAfterNextScene : MonoBehaviour
{
    [SerializeField] private bool hasChangedScene = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (hasChangedScene)
        {
            Destroy(gameObject);
            return;
        }

        hasChangedScene = true;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}