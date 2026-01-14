using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthManager : MonoBehaviour
{
    //This script only exists in order to avoid possible merging issues if another script is modified.
    //This script is supposed to go into the main player controller, and will be merged once I'm sure it works correctly
    public int maxHealth = 5;
    public int currentHealth;

    private InputManagerForPlayer inputManagerForPlayer;
    private PlayerMovement playerMovement;
    public ScoreBoard scoreBoard;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreBoard = ScoreBoard.Instance;
        currentHealth = maxHealth;
        inputManagerForPlayer = GetComponent<InputManagerForPlayer>();
        playerMovement = GetComponent<PlayerMovement>();
        inputManagerForPlayer.canMove = true;
        scoreBoard.GenerateHealthIcons(maxHealth);
    }

    public void LoseLife(int damage)
    {
        if (currentHealth <= damage)
        {
            Debug.Log("Current life is 0");
            Die();
        }
        else
        {
            currentHealth -= damage;
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
        StartCoroutine(ResetSceneAfterWaitTime());
    }

    IEnumerator ResetSceneAfterWaitTime()
    {
        yield return new WaitForSeconds(4);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(currentSceneName);
    }
}
