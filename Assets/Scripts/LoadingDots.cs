using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingDots : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text startText;
    [SerializeField] private bool isPlayer2;
    [SerializeField] private string baseText = "Loading";

    [Header("Dots Settings")]
    [SerializeField] private int maxDots = 3;
    [SerializeField] private float dotsPerSecond = 2f;
    GameManager gameManager;

    private Coroutine animationCoroutine;

    void OnEnable()
    {
        animationCoroutine = StartCoroutine(AnimateDots());
    }

    void Awake()
    {
        if(isPlayer2) startText.gameObject.SetActive(false);
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if (!isPlayer2)
        {
            if (gameManager.playerCount > 0) text.gameObject.SetActive(false);

        }
        else if (gameManager.playerCount > 1)
        {

            startText.gameObject.SetActive(true);
            text.gameObject.SetActive(false);

        }

    }

    void OnDisable()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
    }

    IEnumerator AnimateDots()
    {
        int dotCount = 0;
        float delay = 1f / dotsPerSecond;

        while (true)
        {
            dotCount = (dotCount + 1) % (maxDots + 1);
            text.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(delay);
        }
    }

    

}