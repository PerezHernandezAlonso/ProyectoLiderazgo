using ChristinaCreatesGames.Animations;
using System.Collections;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats stats;
    [SerializeField] Collider2D feetCollider;
    [SerializeField] Collider2D bodyCollider;
    Rigidbody2D rb;
    [SerializeField] GameManager gameManager;
    [SerializeField] ChangeScene changeScene;
    [SerializeField] InputManagerForPlayer inputManagerForPlayer;

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
    Animator animatorOutline;

    InputManagerForPlayer playerInput;
    public bool is2P;

    float knockback;
    [SerializeField] float shootingKnockbackForce = 5f;
    float knockbackDamping = 10f;

    SquashAndStretch squashAndStretch;
    [SerializeField] ParticleSystem muscleflash;
    [SerializeField] ParticleSystem smoke;

    public GameObject SoundManager;
    public GameObject MainSprite;
    public GameObject PlayerOutline;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;


        gameManager = FindFirstObjectByType<GameManager>();
        if (FindFirstObjectByType<ChangeScene>() != null)
        {
            changeScene = FindFirstObjectByType<ChangeScene>();
        }
        inputManagerForPlayer = GetComponentInParent<InputManagerForPlayer>();
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        animator = MainSprite.GetComponent<Animator>();
        animatorOutline = PlayerOutline.GetComponent<Animator>();
        squashAndStretch = GetComponentInChildren<SquashAndStretch>();

        playerInput = GetComponent<InputManagerForPlayer>();
        SoundManager = GameObject.Find("EffectSoundManager");

        if (is2P)
        {
            PlayerOutline.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            PlayerOutline.GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    private void Update()
    {
        if (gameManager.playerCount >= 2)
        {
            if (inputManagerForPlayer.StartButtonDown == true)
            {
                changeScene.LoadScene();
            }
        }

        JumpChecks();
        CountTimers();
        CheckShoot();

        HandleAnimations();

        // borrar cuando se implemente la recogida de armas
        if (pickUpGun) { PickUpGun(guns.pistol); pickUpGun = false; }
    }

    // borrar cuando se implemente la recogida de armas
    public bool pickUpGun;

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        if (isGrounded) { Move(stats.GroundAcceleration, stats.GroundDeceleration, playerInput.MoveInput); }
        else { Move(stats.AirAcceleration, stats.AirDeceleration, playerInput.MoveInput); }

        knockback = Mathf.Lerp(knockback, 0f, knockbackDamping * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(moveVelocity.x + knockback, VerticalVelocity);
    }

    // MOVEMENT
    void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        Vector2 targetVelocity = Vector2.zero;
        if (moveInput.sqrMagnitude > 0.1f)
        {
            TurnCheck(moveInput);
            targetVelocity = new Vector2(moveInput.x, 0) * stats.MaxWalkSpeed;

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            //rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
        }
        else
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            //rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);
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

    // JUMP
    void JumpChecks()
    {
        if (playerInput.JumpButtonDown)
        {
            jumpBufferTimer = stats.JumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }

        if (playerInput.JumpButtonUp)
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

        if (isJumping == false && isFastFalling == false && !isGrounded)
        {
        }

        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -stats.MaxFallSpeed, 50f);

        //rb.linearVelocity = new Vector2(rb.linearVelocity.x, VerticalVelocity);
    }
    void CountTimers()
    {
        jumpBufferTimer -= Time.deltaTime;
        if (jumpBufferTimer < 0f) jumpBufferTimer = 0f;
        if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
            if (coyoteTimer < 0f) coyoteTimer = 0f;
        }
        else
        {
            coyoteTimer = stats.JumpCoyoteTime;
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
        //BumpedHead();
    }
    #region SHOOT
    // PLAYERS SHOOT
    public enum guns { none, pistol, shotgun, tommy };
    [SerializeField] guns currentGun;

    int maxCurrentAmmo;
    [SerializeField] int currentAmmo;

    bool shotCooldown;

    [SerializeField] GameObject bullet;

    [Header("Pistol")]
    int pistolMaxAmmo = 5;
    float pistolSpeed = 100f;
    float pistolRange = .4f;
    int pistolSpread = 0;
    float pistolRate = .3f;
    string pistolShootSound = "pistolSound";
    [SerializeField] ParticleSystem pistolDrop;

    [Header("Shotgun")]
    int shotgunMaxAmmo = 3;
    float shotgunSpeed = 100f;
    float shotgunRange = .1f;
    int shotgunSpread = 5;
    float shotgunRate = 1f;
    string shotgunShootSound = "shotgunSound";
    [SerializeField] ParticleSystem shotgunDrop;

    [Header("Tommy")]
    int tommyMaxAmmo = 7;
    float tommySpeed = 100f;
    float tommyRange = .4f;
    int tommySpread = 1;
    float tommyRate = .1f;
    string tommyShootSound = "tommySound";
    [SerializeField] ParticleSystem tommyDrop;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResolveSceneReferences();
        PickUpGun(guns.pistol);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PickUpGun(guns gunPickUp)
    {
        currentGun = gunPickUp;
        switch (currentGun)
        {
            case guns.pistol:
                maxCurrentAmmo = pistolMaxAmmo;
                currentAmmo = maxCurrentAmmo;
                break;
            case guns.shotgun:
                maxCurrentAmmo = shotgunMaxAmmo;
                currentAmmo = maxCurrentAmmo;
                break;
            case guns.tommy:
                maxCurrentAmmo = tommyMaxAmmo;
                currentAmmo = maxCurrentAmmo;
                break;
        }
        HandleAnimations();
        shotCooldown = false; smoke.Stop();
    }
    void CheckShoot()
    {
        if (shotCooldown) return;
        if (currentGun != guns.none && playerInput.ShootButton)
        {
            switch (currentGun)
            {
                case guns.pistol:
                    SoundManager.GetComponent<SoundManager>().PlaySound(pistolShootSound);
                    Shoot(pistolSpeed, pistolSpread, pistolRate, pistolRange);
                    break;
                case guns.shotgun:
                    SoundManager.GetComponent<SoundManager>().PlaySound(shotgunShootSound);
                    Shoot(shotgunSpeed, shotgunSpread, shotgunRate, shotgunRange);
                    break;
                case guns.tommy:
                    SoundManager.GetComponent<SoundManager>().PlaySound(tommyShootSound);
                    Shoot(tommySpeed, tommySpread, tommyRate, tommyRange);
                    break;
            }
        }
    }
    void Shoot(float speed, int spread, float rate, float range)
    {
        if (shotCooldown) return;

        if (spread > 0)
        {
            for (int i = 0; i < spread; i++)
            {
                SpawnBulletShoot(speed, spread, range);
            }
        }
        else { SpawnBulletShoot(speed, spread, range); }

        shotCooldown = true;
        smoke.Play();
        Invoke(nameof(EnableShooting), rate);

        squashAndStretch.PlaySquashAndStretch();
        muscleflash.Play();
        if (currentAmmo > 0) { currentAmmo--; }
        else
        {

            switch (currentGun)
            {
                case guns.pistol:
                    pistolDrop.Play();
                    break;
                case guns.shotgun:
                    shotgunDrop.Play();
                    break;
                case guns.tommy:
                    tommyDrop.Play();
                    break;
            }
            PickUpGun(guns.none);
        }
    }
    void ResolveSceneReferences()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (changeScene == null)
            changeScene = FindFirstObjectByType<ChangeScene>();

        if (SoundManager == null)
            SoundManager = GameObject.Find("EffectSoundManager");
    }

    #endregion
    void SpawnBulletShoot(float speed, int spread, float range)
    {
        float offset = 1.5f;
        Vector3 spawnPos = transform.position + new Vector3(isFacingRight ? offset : -offset, 0.2f, 0);

        float baseAngle = isFacingRight ? 0f : 180f;
        float randomAngle = randomAngle = (spread > 3) ? Random.Range(-30f, 30f) : (spread > 0) ? Random.Range(-5f, 5f) : 0f;
        float finalAngle = baseAngle + randomAngle;
        Quaternion rot = Quaternion.Euler(0, 0, finalAngle);

        GameObject newBullet = Instantiate(bullet, spawnPos, rot);
        Rigidbody2D bulletRB = newBullet.GetComponent<Rigidbody2D>();
        Vector2 direction = rot * Vector2.right;
        bulletRB.AddForce(direction * speed, ForceMode2D.Impulse);

        knockback += -direction.x * shootingKnockbackForce;

        Destroy(newBullet, range);
    }
    void EnableShooting() { shotCooldown = false; smoke.Stop(); }

    public void AddKnockback(float direction, float strength)
    {
        knockback += direction * strength;

        squashAndStretch.PlaySquashAndStretch();
    }

    // ANIMATIONS

    string currentAnim = "";
    bool wasGroundedLastFrame;
    bool wasRunningLastFrame;
    void HandleAnimations()
    {
        float horizontalSpeed = Mathf.Abs(rb.linearVelocityX);
        bool isRunning = horizontalSpeed > 0.1f && isGrounded && (playerInput.MoveInput.x > 0.1f || playerInput.MoveInput.x < -0.1f);
        bool isStopping = wasRunningLastFrame && !isRunning && isGrounded;
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

        if (currentGun != guns.none)
        {
            switch (currentGun)
            {
                case guns.pistol:
                    animName += "_Pistol";
                    break;
                case guns.shotgun:
                    animName += "_Shotgun";
                    break;
                case guns.tommy:
                    animName += "_Tommy";
                    break;
            }
        }

        if (is2P)
        {
            animator.Play(animName + "_2");
            animatorOutline.Play(animName + "_2");
        }
        else
        {
            animator.Play(animName);
            animatorOutline.Play(animName);
        }

        currentAnim = animName;
    }
    bool IsBlockingAnimationPlaying()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isBlocking = currentAnim == "Jump" || currentAnim == "Land" || currentAnim == "Stop";
        return isBlocking && stateInfo.normalizedTime < .9f;
    }
}

