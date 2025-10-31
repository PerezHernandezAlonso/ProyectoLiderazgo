using UnityEngine;

[CreateAssetMenu(menuName = "PlayerMovement")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 100f)] public float MaxWalkSpeed = 12f;
    [Range(1f, 100f)] public float GroundAcceleration = 5f;
    [Range(1f, 100f)] public float GroundDeceleration = 20f;
    [Range(1f, 100f)] public float AirAcceleration = 5f;
    [Range(1f, 100f)] public float AirDeceleration = 5f;

    [Header("Grounded/Collision Checks")]
    public LayerMask GroundLayer;
    public float GroundDetectionRayLength = 0.02f;
    public float HeadDetectionRayLength = 0.02f;
    [Range(0f, 1f)] public float HeadWidth = 0.75f;

    [Header("Jump")]
    public float JumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float JumpHeightCompensationFactor = 1.054f;
    public float TimeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;
    public float MaxFallSpeed = 26f;
    [Range(1f, 5f)] public int NumberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float ApexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float JumpBufferTime = 0.125f;

    [Header("Coyote Time")]
    [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;

    [Header("Debug")]
    public bool DebugShowIsGroundedBox;
    public bool DebugShowHeadBumpBox;

    [Header("Jump Vizualization Tool")]
    public bool ShowWalkJumpArc;
    public bool ShowRunJumpArc;
    public bool StopOnCollision = true;
    public bool DrawRight = true;
    [Range(5f, 100f)] public int ArcResolution = 20; 
    [Range(0f, 500f)] public int VisualizationSteps= 90; 

    public float Gravity {  get; private set; }
    public float InitialJumpVelocity {  get; private set; }
    public float AdjustedJumpHeigth {  get; private set; }
    private void OnValidate()
    {
        CalculateValues();
    }
    private void OnEnable()
    {
        CalculateValues();
    }
    void CalculateValues()
    {
        AdjustedJumpHeigth = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * AdjustedJumpHeigth) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;
    }
}
