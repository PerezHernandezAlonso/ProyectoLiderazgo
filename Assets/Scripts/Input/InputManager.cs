using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager singleton;
    public static InputManager Singleton => singleton;

    [HideInInspector] public Input input;
    [HideInInspector] public PlayerInput playerInput;

    InputAction Move;
    InputAction Jump;

    Vector2 moveInput;
    public Vector2 MoveInput => moveInput;

    public bool JumpButton { get; private set; }
    public bool JumpButtonDown { get; private set; }
    public bool JumpButtonUp { get; private set; }


    InputBuffer jumpBuffer;
    public bool IsJumpBuffered => jumpBuffer.IsBuffered;
    public void ConsumeJumpBuffer() => jumpBuffer.Consume();

    [SerializeField] float inputBufferTime = 0.2f;

    private void Awake()
    {
        if (Singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        input = new Input();
    }
    private void Start()
    {
        Move = input.PlayerInput.Move;
        Jump = input.PlayerInput.Jump;

        jumpBuffer = new InputBuffer(inputBufferTime);
    }
    private void Update()
    {
        moveInput = Move.ReadValue<Vector2>();

        JumpButton = Jump.IsPressed();
        JumpButtonDown = Jump.WasPressedThisFrame();
        JumpButtonUp = Jump.WasReleasedThisFrame();

        if (JumpButtonDown) { jumpBuffer.RegisterInput(); }
        jumpBuffer.Update();
    }

    #region // Enable & Disable Input
    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }
    #endregion 

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