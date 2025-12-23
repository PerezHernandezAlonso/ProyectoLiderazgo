using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManagerForPlayer : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction move;
    private InputAction jump;

    private InputAction attack;


    private InputAction shoot;


    public Vector2 MoveInput { get; private set; }
    public bool JumpButton { get; private set; }
    public bool JumpButtonDown { get; private set; }
    public bool JumpButtonUp { get; private set; }

    public bool AttackButton { get; private set; }
    public bool AttackButtonDown { get; private set; }

    public bool ShootButton { get; private set; }


    private InputBuffer jumpBuffer;
    [SerializeField] float inputBufferTime = 0.2f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        jumpBuffer = new InputBuffer(inputBufferTime);
    }

    private void Start()
    {
        move = playerInput.actions["Move"];
        jump = playerInput.actions["Jump"];

        attack = playerInput.actions["Attack"];

        shoot = playerInput.actions["Attack"];

    }

    private void Update()
    {
        MoveInput = move.ReadValue<Vector2>();

        JumpButton = jump.IsPressed();
        JumpButtonDown = jump.WasPressedThisFrame();
        JumpButtonUp = jump.WasReleasedThisFrame();

        AttackButton = attack.IsPressed();
        AttackButtonDown = attack.WasPressedThisFrame();

        if (JumpButtonDown) jumpBuffer.RegisterInput();
        jumpBuffer.Update();

        ShootButton = shoot.IsPressed();    
    }
    public class InputBuffer
    {
        private float bufferDuration;
        private float bufferTimer;
        public bool IsBuffered { get; private set; }

        public InputBuffer(float duration)
        {
            bufferDuration = duration;
        }

        public void RegisterInput()
        {
            IsBuffered = true;
            bufferTimer = bufferDuration;
        }

        public void Update()
        {
            if (!IsBuffered) return;

            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0f)
            {
                IsBuffered = false;
            }
        }

        public void Consume() { IsBuffered = false; }
    }
}

