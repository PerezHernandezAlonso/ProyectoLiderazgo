using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Metadata;

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard Instance;
    public Transform lifeContainer1;
    public Transform lifeContainer2;
    public GameObject healthIconPrefab;
    public GameObject lifesIconPrefab;

    public GameObject healthContainer1;
    public GameObject healthContainer2;

    public Sprite[] healthicons1;
    public Sprite[] healthicons2;
    public Sprite lostlifeIcon;


    public GameObject[] lifePlayer1;
    public GameObject[] lifePlayer2;



    public TMP_Text player2Points;
    public TMP_Text player1Points;


    public uint player1Score;
    public uint player2Score;

    public uint maxScore = 5;
    public int player1health = 5;
    public int player2health = 5;


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
    void Start()
    {
        /*getLifePlayer1();
        getLifePlayer2();*/
    }

    /*public void getLifePlayer1()
    {
        lifePlayer1 = new GameObject[lifeContainer1.transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            lifePlayer1[i] = transform.GetChild(i).gameObject;
        }
    }

    public void getLifePlayer2()
    {
        lifePlayer2 = new GameObject[lifeContainer2.transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            lifePlayer2[i] = transform.GetChild(i).gameObject;
        }
    }*/


    public void OnPlayerScored(bool isPlayer2)
    {
        if (isPlayer2)
        {
            player2Score++;
            player2Points.text = player2Score.ToString();

            lifePlayer2[lifePlayer2.Length - player2Score].GetComponent<Image>().sprite = lostlifeIcon;
            if (player2Score >= maxScore)
            {
                Win(true);
            }
        }
        else
        {
            player1Score++;
            player1Points.text = player1Score.ToString();
            lifePlayer1[lifePlayer1.Length - player1Score].GetComponent<Image>().sprite = lostlifeIcon;
            if (player1Score >= maxScore)
            {
                Win(false);
            }

        }

    }
    void Win(bool isPlayer2)
    {
        Debug.Log($"Player {(isPlayer2 ? 2 : 1)} wins!");
        if (isPlayer2)
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

    public void UpdatePlayerHealth(bool isPlayer2, int amount)
    {
        Debug.Log(amount);

        if (isPlayer2)
        {
            if (amount == 0) amount = 1;
            player2health = player2health - amount;

            if (isPlayer2 && player2health <= 0)
            {
                Debug.Log("heal");
                player2health = 5;
                healthContainer2.GetComponent<Image>().sprite = healthicons2[player2health - 1];
            }
            else
            {
                if (player2health < 0) player2health = 0;
                healthContainer2.GetComponent<Image>().sprite = healthicons2[player2health - 1];
            }
        }
        else
        {
            if (amount == 0) amount = 1;
            player1health = player1health - amount;

            if (!isPlayer2 && player1health <= 0)
            {
                Debug.Log("heal");
                player1health = 5;
                healthContainer1.GetComponent<Image>().sprite = healthicons1[player1health - 1];
            }
            else
            {
                if (player1health < 0) player1health = 0;
                healthContainer1.GetComponent<Image>().sprite = healthicons1[player1health - 1];
            }
        }
    }
}
