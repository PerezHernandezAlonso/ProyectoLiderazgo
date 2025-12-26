using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    public uint player1Score;
    public uint player2Score;

    public uint maxScore = 5;

    private void Start()
    {
        ResetScoreBoard();
        DontDestroyOnLoad(this);
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

        }
        else
        {
            player1Score++;
        }

    }
    void Win(bool isPlayer2)
    {
        Debug.Log($"Player {(isPlayer2 ? 2 : 1)} wins!");
        //Rest of the logic should go here. Something about maybe resetting the game, we need to make an actual scene for the game though
    }

    public void ResetScoreBoard()
    {
        player1Score = 0;
        player2Score = 0;
    }
}
