using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Player Movement")]
    public float movementSpeed = 5f;
    public MainCameraController mainCameraController;
    public EnvironmentChecker environmentChecker;
    public float rotationSpeed = 600f;

    Quaternion requiredRotation;
    bool playerControl = true;

    public bool playerInAction { get; private set; }

    [Header("Player Animator")]
    public Animator animator;

    [Header("Player Collision and Gravity")]
    public CharacterController characterController;
    public float surfaceCheckRadius = 0.3f;
    public Vector3 surfaceCheckOffset;
    public LayerMask surfaceLayer;

    bool onSurface;

    public bool playerOnLedge { get; set; }
    public bool playerhanging { get; set; }
    public LedgeInfo LedgeInfo { get; set; }

    [SerializeField] float fallingSpeed;
    [SerializeField] Vector3 moveDir;
    [SerializeField] Vector3 requiredMoveDir;

    Vector3 velocity;

    void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (environmentChecker == null)
            environmentChecker = GetComponent<EnvironmentChecker>();
    }

    void Update()
    {
        if (!playerControl || playerhanging)
            return;

        if (characterController == null ||
            animator == null ||
            environmentChecker == null ||
            mainCameraController == null)
        {
            return;
        }

        velocity = Vector3.zero;

        if (onSurface)
        {
            fallingSpeed = 0f;
            velocity = moveDir * movementSpeed;

            playerOnLedge =
                environmentChecker.ChechLedge(moveDir, out LedgeInfo ledgeInfo);

            if (playerOnLedge)
            {
                LedgeInfo = ledgeInfo;
                PlayerLedgeMovement();
            }

            float movementValue = movementSpeed > 0f
                ? velocity.magnitude / movementSpeed
                : 0f;

            animator.SetFloat(
                "movementValue",
                movementValue,
                0.2f,
                Time.deltaTime
            );
        }
        else
        {
            fallingSpeed += Physics.gravity.y * Time.deltaTime;
            velocity = transform.forward * movementSpeed / 2f;
        }

        velocity.y = fallingSpeed;

        PlayerMovement();
        SurfaceCheck();

        animator.SetBool("onSurface", onSurface);
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float movementAmount = Mathf.Clamp01(
            Mathf.Abs(horizontal) + Mathf.Abs(vertical)
        );

        var movementInput =
            new Vector3(horizontal, 0f, vertical).normalized;

        requiredMoveDir =
            mainCameraController.floatRotation * movementInput;

        characterController.Move(velocity * Time.deltaTime);

        if (movementAmount > 0f && moveDir.magnitude > 0.2f)
        {
            requiredRotation = Quaternion.LookRotation(moveDir);
        }

        moveDir = requiredMoveDir;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            requiredRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void SurfaceCheck()
    {
        onSurface = Physics.CheckSphere(
            transform.TransformPoint(surfaceCheckOffset),
            surfaceCheckRadius,
            surfaceLayer
        );
    }

    void PlayerLedgeMovement()
    {
        float angle =
            Vector3.Angle(LedgeInfo.surfacehit.normal, requiredMoveDir);

        if (angle < 90f)
        {
            velocity = Vector3.zero;
            moveDir = Vector3.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(
            transform.TransformPoint(surfaceCheckOffset),
            surfaceCheckRadius
        );
    }

    public IEnumerator PerformAction(
        string AnimationName,
        CompareTargetParameter ctp = null,
        Quaternion RequiredRotation = new Quaternion(),
        bool LookAtObstacle = false,
        float ParkourActionDelay = 0f)
    {
        if (animator == null)
        {
            Debug.LogError("PlayerScript: Animator reference is missing.", this);
            yield break;
        }

        playerInAction = true;

        animator.CrossFadeInFixedTime(AnimationName, 0.2f);

        yield return null;

        var animationState = animator.GetNextAnimatorStateInfo(0);

        if (!animationState.IsName(AnimationName))
        {
            Debug.LogWarning(
                "PlayerScript: Animation state not found: " + AnimationName,
                this
            );
        }

        float animationLength = animationState.length;

        if (animationLength <= 0f)
        {
            playerInAction = false;
            yield break;
        }

        float rotateStartTime = ctp != null ? ctp.startTime : 0f;
        float timerCounter = 0f;

        while (timerCounter <= animationLength)
        {
            timerCounter += Time.deltaTime;

            float normalizedTimerCounter =
                timerCounter / animationLength;

            if (LookAtObstacle && normalizedTimerCounter > rotateStartTime)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    RequiredRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            if (ctp != null)
            {
                CompareTarget(ctp);
            }

            if (animator.IsInTransition(0) && timerCounter > 0.5f)
                break;

            yield return null;
        }

        if (ParkourActionDelay > 0f)
            yield return new WaitForSeconds(ParkourActionDelay);

        playerInAction = false;
    }

    void CompareTarget(CompareTargetParameter compareTargetParameter)
    {
        if (animator == null || compareTargetParameter == null)
            return;

        animator.MatchTarget(
            compareTargetParameter.position,
            transform.rotation,
            compareTargetParameter.bodyPart,
            new MatchTargetWeightMask(
                compareTargetParameter.positionWeight,
                0f
            ),
            compareTargetParameter.startTime,
            compareTargetParameter.endTime
        );
    }

    public void SetControl(bool hasControl)
    {
        playerControl = hasControl;

        if (characterController != null)
            characterController.enabled = hasControl;

        if (!hasControl)
        {
            if (animator != null)
                animator.SetFloat("movementValue", 0f);

            requiredRotation = transform.rotation;
        }
    }

    public void EnableCC(bool endbled)
    {
        if (characterController != null)
            characterController.enabled = endbled;
    }

    public void ResetRequiredRotation()
    {
        requiredRotation = transform.rotation;
    }

    public bool HasPlayerControl
    {
        get => playerControl;
        set => playerControl = value;
    }
}

public class CompareTargetParameter
{
    public Vector3 position;
    public AvatarTarget bodyPart;
    public Vector3 positionWeight;
    public float startTime;
    public float endTime;
}