using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    public int playerCount = 0;
    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }
    private void OnEnable()
    {
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        playerCount++;

        PlayerMovement pm = playerInput.GetComponent<PlayerMovement>();

        if (playerCount == 2 && pm != null)
        {
            pm.is2P = true;
        }
    }
}
