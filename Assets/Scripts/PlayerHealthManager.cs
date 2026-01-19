using System.Collections;
using TMPro;
using UnityEditor.Rendering.Canvas.ShaderGraph;
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
        if (!inputManagerForPlayer.canMove) return;

        if (currentHealth <= 1)
        {
            Debug.Log("Salud 0");
            if (!gameManager.IsTraining)
            {
                currentHealth = 5;
                Die();
                Debug.Log("muerto");
            }
            else
            {
                currentHealth = 5;
                Die();
                Debug.Log("muerto");
            }
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
        scoreBoard.UpdatePlayerHealth(playerMovement.is2P, 0);
    }
}
