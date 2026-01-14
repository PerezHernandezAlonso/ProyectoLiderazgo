using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard Instance;
    public Transform healthContainer1;
    public Transform lifeContainer1;
    public Transform healthContainer2;
    public Transform lifeContainer2;
    public GameObject healthIconPrefab;
    public GameObject lifesIconPrefab;

    public TMP_Text player2Points;
    public TMP_Text player1Points;


    public uint player1Score;
    public uint player2Score;

    public uint maxScore = 5;


    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        //ResetScoreBoard();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Will add a point to the assigned player 
    /// </summary>
    /// <param name="isPlayer2"></param>
    public void OnPlayerScored(bool isPlayer2)
    {
        if (isPlayer2)
        {
            player2Score++;
            if (player2Score >= maxScore) 
            {
                Win(true);
            }
        }
        else
        {
            player1Score++;
            if (player1Score >= maxScore)
            {
                Win(false);
            }

        }

    }
    void Win(bool isPlayer2)
    {
        Debug.Log($"Player {(isPlayer2 ? 2 : 1)} wins!");
        if(isPlayer2)
        SceneManager.LoadScene("Player2Wins");
        else
        SceneManager.LoadScene("Player1Wins");
        //Rest of the logic should go here. Something about maybe resetting the game, we need to make an actual scene for the game though
        //Robin - Hecho :D
    }

    public void ResetScoreBoard()
    {
        player1Score = 0;
        player2Score = 0;
    }

    public void GenerateHealthIcons(int amount)
    {
        
        foreach (Transform child in healthContainer1)
            Destroy(child.gameObject);

        for (int i = 0; i < amount; i++)
        {
            Instantiate(healthIconPrefab, healthContainer1);
        }
        foreach (Transform child in healthContainer2)
            Destroy(child.gameObject);

        for (int i = 0; i < amount; i++)
        {
            Instantiate(healthIconPrefab, healthContainer2);
        }
    }
    public void UpdatePlayerHealth(bool isPlayer2, int amount)
    {
        foreach (Transform child in !isPlayer2? healthContainer1: healthContainer2)
            Destroy(child.gameObject);

        for (int i = 0; i < amount; i++)
        {
            Instantiate(healthIconPrefab, !isPlayer2 ? healthContainer1 : healthContainer2);
        }
    }
    public void UpdatePlayerLifes(bool isPlayer2, int amount)
    {
        foreach (Transform child in !isPlayer2 ? lifeContainer1 : lifeContainer2)
            Destroy(child.gameObject);

        for (int i = 0; i < amount; i++)
        {
            Instantiate(healthIconPrefab, !isPlayer2 ? lifeContainer1 : lifeContainer2);
        }
    }
}
