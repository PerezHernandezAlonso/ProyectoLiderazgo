using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] TextMeshProUGUI StatScreen;
    [SerializeField] Animator animator;

    [Header("Movement")]
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpTime;
    [SerializeField] float fallMultiplier;
    float jumpTimer;
    bool isJumping;

    [Header("AirMovement")]
    [SerializeField] float airAcceleration;
    [SerializeField] float airFriction;

    [Header("GroundState")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool isGrounded;
    bool isPhysicallyGrounded;

    [SerializeField] float coyoteTime;
    float coyoteTimer;

    string currentAnim = "";
    bool wasGroundedLastFrame;
    bool wasRunningLastFrame;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        StatScreen.text = rb.linearVelocityX.ToString("F2") + "\n" + currentAnim;
        HandleAnimations();
    }

    void FixedUpdate()
    {
        GroundCheck();
        Move();
        Flip();
        HandleJump();
    }

    void Move()
    {
        Vector2 moveVector = new Vector2(InputManager.Singleton.MoveInput.x, 0);
        float pickAcceleration = isGrounded ? acceleration : airAcceleration;
        float pickDeceleration = isGrounded ? deceleration : airFriction;

        if (InputManager.Singleton.MoveInput.magnitude > 0.1f)
        {
            rb.AddForce(moveVector * pickAcceleration);

            if (Mathf.Abs(rb.linearVelocityX) >= maxSpeed &&
                Mathf.Sign(rb.linearVelocityX) != Mathf.Sign(moveVector.x))
            {
                rb.AddForce(moveVector * deceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);
            }
        }
        else
        {
            rb.AddForce(new Vector2(-rb.linearVelocityX, 0) * pickDeceleration);
        }

        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxSpeed, maxSpeed);
    }

    void HandleJump()
    {
        if (InputManager.Singleton.IsJumpBuffered && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
            jumpTimer = 0f;
            InputManager.Singleton.ConsumeJumpBuffer();
        }

        if (InputManager.Singleton.JumpButton && isJumping)
        {
            jumpTimer += Time.fixedDeltaTime;
            if (jumpTimer < jumpTime)
            {
                rb.AddForce(Vector2.up * (jumpForce * 0.5f) * Time.fixedDeltaTime, ForceMode2D.Force);
            }
        }

        if (!isJumping && !isPhysicallyGrounded)
        {
            rb.AddForce(Vector2.down * fallMultiplier * Time.fixedDeltaTime);
        }

        if (InputManager.Singleton.JumpButtonUp || jumpTimer >= jumpTime)
        {
            isJumping = false;
            jumpTimer = 0f;
        }
    }

    void GroundCheck()
    {
        isPhysicallyGrounded = Physics2D.OverlapCircle(groundCheck.position + new Vector3(0, playerHeight), groundCheckRadius, groundLayer);
        if (isPhysicallyGrounded)
        {
            isGrounded = true;
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime;
            isGrounded = coyoteTimer > 0f;
        }
    }

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
        bool isStopping = wasRunningLastFrame && !isRunning && isGrounded;
        bool isFalling = rb.linearVelocity.y < -0.1f && !isGrounded;
        bool isRising = rb.linearVelocity.y > 0.1f && !isGrounded;
        bool justLanded = !wasGroundedLastFrame && isGrounded;

        if (isRising) PlayAnim("Jump");
        else if (isFalling) PlayAnim("fall");
        else if (justLanded) PlayAnim("Land");
        else if (isRunning) PlayAnim("Run");
        else if (isStopping) PlayAnim("Stop");
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position + new Vector3(0, playerHeight), groundCheckRadius);
    }
}
