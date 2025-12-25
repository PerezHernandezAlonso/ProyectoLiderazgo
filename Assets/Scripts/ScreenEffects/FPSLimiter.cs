using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    [SerializeField] int targetFPS = 60;
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
    }
}
