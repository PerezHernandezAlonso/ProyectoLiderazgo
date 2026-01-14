using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CinematicManager : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image blackOverlay;        // Assign your full-screen black UI Image
    public float fadeDuration = 2f;   // Time to fade in/out
    public string nextScene = "MenuScene"; // Scene to load after fade

    private void Awake()
    {
        // Start the cinematic fade automatically
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        if (blackOverlay == null)
        {
            Debug.LogError("Black overlay Image not assigned!");
            yield break;
        }

        // Ensure the overlay starts fully black
        Color color = blackOverlay.color;
        color.a = 1f;
        blackOverlay.color = color;

        // Fade from black to transparent
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            blackOverlay.color = color;
            yield return null;
        }
        color.a = 0f;
        blackOverlay.color = color;

        // Optional pause to show the scene
        yield return new WaitForSeconds(1f);

        // Fade back to black
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            blackOverlay.color = color;
            yield return null;
        }

        // Load the next scene
        SceneManager.LoadScene(nextScene);
    }
}
