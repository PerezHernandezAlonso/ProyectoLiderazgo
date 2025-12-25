using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class ScreenEffects : MonoBehaviour
{
    private static ScreenEffects singleton;
    public static ScreenEffects Singleton
    {
        get { return singleton; }
    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ScreenShake(float ShakeDuration, float ShakeIntensity)
    {
        impulseSource.ImpulseDefinition.ImpulseDuration = ShakeDuration;

        impulseSource.GenerateImpulseWithForce(ShakeIntensity);
    }

    public void HitStop(float duration)
    {
        StartCoroutine(HitStopCoroutine(duration));
    }

    IEnumerator HitStopCoroutine(float duration)
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }
}
