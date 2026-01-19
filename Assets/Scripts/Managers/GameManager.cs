using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private bool isTraining;
    public bool IsTraining => isTraining;
    [SerializeField] private Transform puntoDeMuerte;
    [SerializeField] private Transform puntoDeRespawn1;
    [SerializeField] private Transform puntoDeRespawn2;
    public int playerCount = 0;
    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }
    private void OnEnable()
    {
        if (playerCount != 0)
        {
            PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        }

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
    public IEnumerator Respawn(GameObject gameObject_)
    {
        if(!IsTraining)
        {
            gameObject_.transform.position = puntoDeMuerte.transform.position;
            yield return new WaitForSeconds(4f);
        }
        if (gameObject_.GetComponentInParent<InputManagerForPlayer>() == FindFirstObjectByType<InputManagerForPlayer>())
            gameObject_.transform.position = puntoDeRespawn1.transform.position;
        else gameObject_.transform.position = puntoDeRespawn2.transform.position;
        gameObject_.GetComponentInParent<InputManagerForPlayer>().canMove = true;

    }
}
