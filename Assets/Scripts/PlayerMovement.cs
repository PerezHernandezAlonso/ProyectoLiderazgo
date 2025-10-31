using System.Security;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats stats;
    [SerializeField] Collider2D feetCollider;
    [SerializeField] Collider2D bodyCollider;
    Rigidbody2D rb;

    Vector2 moveVelocity;
    bool isFacingRight;

    RaycastHit2D groundHit;
    RaycastHit2D headHit;
    [SerializeField] bool isGrounded;
    bool bumpedHead;

    public float VerticalVelocity { get; private set; }
    [SerializeField] bool isJumping;
    [SerializeField] bool isFastFalling;
    [SerializeField] bool isFalling;
    [SerializeField] float fastFallTime;
    float fastFallReleaseSpeed;
    int numberOfJumpsUsed;

    float apexPoint;
    float timePastApexThreshold;
    bool isPastApexThreshold;

    [SerializeField] float jumpBufferTimer;
    bool jumpReleasedDuringBuffer;

    [SerializeField] float coyoteTimer;

    Animator animator;

    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        JumpChecks();
        CountTimers();

        HandleAnimations();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        if (isGrounded) { Move(stats.GroundAcceleration, stats.GroundDeceleration, InputManager.Singleton.MoveInput); }
        else { Move(stats.AirAcceleration, stats.AirDeceleration, InputManager.Singleton.MoveInput); }
    }

    void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        Vector2 targetVelocity = Vector2.zero;
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);
            targetVelocity = new Vector2(moveInput.x, 0) * stats.MaxWalkSpeed;

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
        }
        else
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
        }
    }
    void TurnCheck(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }
    void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0);
        }
    }

    void JumpChecks()
    {
        if (InputManager.Singleton.JumpButtonDown)
        {
            jumpBufferTimer = stats.JumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }

        if (InputManager.Singleton.JumpButtonUp)
        {
            if (jumpBufferTimer > 0)
            {
                jumpReleasedDuringBuffer = true;
            }

            if (isJumping && VerticalVelocity > 0)
            {
                if (isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = stats.TimeForUpwardsCancel;
                    VerticalVelocity = 0;
                }
                else
                {
                    isFastFalling = true;
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        if (jumpBufferTimer > 0 && !isJumping && (isGrounded || coyoteTimer > 0))
        {
            InitiateJump(1);

            if (jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }
        else if (jumpBufferTimer > 0 && isJumping && numberOfJumpsUsed < stats.NumberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);
        }
        else if (jumpBufferTimer > 0 && isFalling && numberOfJumpsUsed < stats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            isFastFalling = false;
        }

        // LANDED
        if ((isJumping || isFalling) && isGrounded && VerticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }
    void InitiateJump(int _numberOfJumpsUsed)
    {
        if (!isJumping)
        {
            isJumping = true;
        }

        jumpBufferTimer = 0;
        numberOfJumpsUsed += _numberOfJumpsUsed;
        VerticalVelocity = stats.InitialJumpVelocity;
    }
    void Jump()
    {
        // Si no estamos saltando, solo aplicamos gravedad normal en la rama de caída
        if (isJumping)
        {
            if (bumpedHead)
            {
                isFastFalling = true;
            }

            if (VerticalVelocity >= 0)
            {
                apexPoint = Mathf.InverseLerp(stats.InitialJumpVelocity, 0, VerticalVelocity);

                if (apexPoint > stats.ApexThreshold)
                {
                    if (!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0;
                    }

                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < stats.ApexHangTime)
                        {
                            VerticalVelocity = 0;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                else
                {
                    // Subiendo, aplicar gravedad hacia abajo (reduce la velocidad de subida)
                    VerticalVelocity += stats.Gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold) { isPastApexThreshold = false; }
                }
            }
            else
            {
                // Ya estamos por debajo de 0: caída normal o fast-fall
                if (!isFastFalling)
                {
                    VerticalVelocity += stats.Gravity * stats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }
                else if (VerticalVelocity < 0)
                {
                    if (!isFalling) { isFalling = true; }
                    // en fast falling, la lógica de fastFall se encarga de la velocidad
                }
            }
        }

        if (isFastFalling)
        {
            if (fastFallTime >= stats.TimeForUpwardsCancel)
            {
                VerticalVelocity += stats.Gravity * stats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else
            {
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0, (fastFallTime / stats.TimeForUpwardsCancel));
            }
            fastFallTime += Time.fixedDeltaTime;
        }

        // Si no estamos en salto y no estamos en ground, marcar caída
        if (!isGrounded && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            VerticalVelocity += stats.Gravity * stats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
        }

        // Normalizar/aplicar gravedad si estamos en caída fuera del bloque de isJumping
        if (isJumping == false && isFastFalling == false && !isGrounded)
        {
            // ya aplicado arriba donde corresponde
        }

        // Clamp final
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -stats.MaxFallSpeed, 50f);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, VerticalVelocity);
    }

void CountTimers()
{
    jumpBufferTimer -= Time.deltaTime;
    if (jumpBufferTimer < 0f) jumpBufferTimer = 0f; // evita negativos persistentes

    if (!isGrounded)
    {
        coyoteTimer -= Time.deltaTime;
        if (coyoteTimer < 0f) coyoteTimer = 0f;
    }
    else
    {
        coyoteTimer = stats.JumpCoyoteTime; // reseteo inmediato cuando estás en tierra
    }
}


    void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x, stats.GroundDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, stats.GroundDetectionRayLength, stats.GroundLayer);

        if (groundHit.collider != null) { isGrounded = true; }
        else { isGrounded = false; }

        if (stats.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (isGrounded)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * stats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * stats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - stats.GroundDetectionRayLength), Vector2.right * boxCastSize, rayColor);
        }
    }
    void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, bodyCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x * stats.HeadWidth, stats.HeadDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, stats.HeadDetectionRayLength, stats.GroundLayer);

        if (headHit.collider != null) { bumpedHead = true; }
        else { bumpedHead = false; }

        if (stats.DebugShowHeadBumpBox)
        {
            float headWidth = stats.HeadWidth;
            Color rayColor;
            if (bumpedHead)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.up * stats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.up * stats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - stats.HeadDetectionRayLength), Vector2.right * boxCastSize * headWidth, rayColor);
        }
    }
    void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }

    // ANIMATIONS

    string currentAnim = "";
    bool wasGroundedLastFrame;
    bool wasRunningLastFrame;
    void Flip()
    {
        float x = InputManager.Singleton.MoveInput.x;
        if (x > 0) transform.localScale = Vector3.one;
        else if (x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void HandleAnimations()
    {
        float horizontalSpeed = Mathf.Abs(rb.linearVelocityX);
        bool isRunning = horizontalSpeed > 0.1f && isGrounded;
        //bool isStopping = wasRunningLastFrame && !isRunning && isGrounded;
        bool isFalling = rb.linearVelocity.y < -0.1f && !isGrounded;
        bool isRising = rb.linearVelocity.y > 0.1f && !isGrounded;
        bool justLanded = !wasGroundedLastFrame && isGrounded;

        if (isRising) PlayAnim("Jump");
        else if (isFalling) PlayAnim("Fall");
        else if (justLanded) PlayAnim("Land");
        else if (isRunning) PlayAnim("Run");
        //else if (isStopping) PlayAnim("Stop");
        else if (isGrounded) PlayAnim("Idle");

        wasGroundedLastFrame = isGrounded;
        wasRunningLastFrame = isRunning;
    }

    void PlayAnim(string animName)
    {
        if (currentAnim == animName || IsBlockingAnimationPlaying()) return;
        animator.Play(animName);
        currentAnim = animName;
    }

    bool IsBlockingAnimationPlaying()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isBlocking = currentAnim == "Jump" || currentAnim == "Land" || currentAnim == "Stop";
        return isBlocking && stateInfo.normalizedTime < 1f;
    }

}
