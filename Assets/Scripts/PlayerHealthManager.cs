using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthManager : MonoBehaviour
{
    //This script only exists in order to avoid possible merging issues if another script is modified.
    //This script is supposed to go into the main player controller, and will be merged once I'm sure it works correctly
    public float maxHealth = 100f;
    public float currentHealth;

    private InputManagerForPlayer inputManagerForPlayer;
    private PlayerMovement playerMovement;
    public ScoreBoard scoreBoard;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        inputManagerForPlayer = GetComponent<InputManagerForPlayer>();
        playerMovement = GetComponent<PlayerMovement>();
        inputManagerForPlayer.canMove = true;
    }

    public void LoseLife(float damage)
    {
        if (currentHealth < damage)
        {
            Die();
        }
        else
        {
            currentHealth -= damage;
        }
    }
    public void Die()
    {
        inputManagerForPlayer.canMove = false;
        if (playerMovement.is2P)
        {
            scoreBoard.OnPlayerScored(true);
        }
        else
        {
            scoreBoard.OnPlayerScored(false);
        }
    }

    IEnumerator ResetSceneAfterWaitTime()
    {
        yield return new WaitForSeconds(4);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
