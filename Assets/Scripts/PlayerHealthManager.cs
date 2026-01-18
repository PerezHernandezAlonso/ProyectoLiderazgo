using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthManager : MonoBehaviour
{
    //This script only exists in order to avoid possible merging issues if another script is modified.
    //This script is supposed to go into the main player controller, and will be merged once I'm sure it works correctly
    public int maxLifes = 5;
    public int maxHealth = 5;
    public int currentHealth;
    public int currentLife;

    private InputManagerForPlayer inputManagerForPlayer;
    private PlayerMovement playerMovement;
    public ScoreBoard scoreBoard;
    public GameManager gameManager;


    private void Start()
    {
        scoreBoard.GenerateHealthIcons(5);
    }
    private void Awake()
    {
        FindFirstObjectByType<ScoreBoard>();
        gameManager = FindFirstObjectByType<GameManager>();

        scoreBoard = ScoreBoard.Instance;
        currentHealth = maxHealth;
        currentLife = maxLifes;
        inputManagerForPlayer = GetComponent<InputManagerForPlayer>();
        playerMovement = GetComponent<PlayerMovement>();
        inputManagerForPlayer.canMove = true;
    }

    public void LoseHealth(int damage)
    {
        if (currentHealth <= damage)
        {
            Debug.Log("Salud 0");
            if (!gameManager.IsTraining)
                Die();
            else StartCoroutine(gameManager.Respawn(gameObject));
        }
        else
        {
            currentHealth -= damage;
            if(scoreBoard != null)
            scoreBoard.UpdatePlayerHealth(playerMovement.is2P, damage);
        }
    }
    public void Die()
    {
        inputManagerForPlayer.canMove = false;
        if (playerMovement.is2P)
        {
            scoreBoard.OnPlayerScored(false);
        }
        else
        {
            scoreBoard.OnPlayerScored(true);
        }
        StartCoroutine(gameManager.Respawn(gameObject));
    }
    public void LoseLife()
    {
        if (currentLife <= 0)
        {
            Debug.Log("Vidas 0");
        }
        else
        {
            currentLife -= 1;
            //if (scoreBoard != null)
            //    scoreBoard.UpdatePlayerLifes(playerMovement.is2P, 1);
        }
    }
}
