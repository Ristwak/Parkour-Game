using System.Collections;
using UnityEngine;

public class ClimbingController : MonoBehaviour
{
    EnvironmentChecker environmentChecker;

    public PlayerScript playerScript;

    ClimbingPoint currentClimbPoint;

    public float InOutValue;
    public float UpDownValue;
    public float LeftRightValue;

    void Awake()
    {
        environmentChecker = GetComponent<EnvironmentChecker>();

        if (playerScript == null)
            playerScript = GetComponent<PlayerScript>();
    }

    void Update()
    {
        if (playerScript == null || environmentChecker == null)
            return;

        if (!playerScript.playerhanging)
        {
            if (Input.GetButton("Jump") && !playerScript.playerInAction)
            {
                if (environmentChecker.CheckClimbing(transform.forward, out RaycastHit climbInfo))
                {
                    currentClimbPoint = climbInfo.transform.GetComponent<ClimbingPoint>();

                    if (currentClimbPoint == null)
                        return;

                    playerScript.SetControl(false);

                    InOutValue = -0.23f;
                    UpDownValue = -0.09f;
                    LeftRightValue = 0.15f;

                    StartCoroutine(
                        ClimbToLedge(
                            "IdleToClimb",
                            climbInfo.transform,
                            0.40f,
                            54f,
                            playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)
                        )
                    );
                }
            }

            if (Input.GetButton("Leave") && !playerScript.playerInAction)
            {
                if (environmentChecker.CheckDropClimbPoint(out RaycastHit dropHit))
                {
                    currentClimbPoint = GetNearestClimbingPoint(dropHit.transform, dropHit.point);

                    if (currentClimbPoint == null)
                        return;

                    playerScript.SetControl(false);

                    InOutValue = 0.1f;
                    UpDownValue = -0.44f;
                    LeftRightValue = 0.25f;

                    StartCoroutine(
                        ClimbToLedge(
                            "DropToFreehang",
                            currentClimbPoint.transform,
                            0.41f,
                            0.54f,
                            playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)
                        )
                    );
                }
            }
        }
        else
        {
            if (Input.GetButton("Leave") && !playerScript.playerInAction)
            {
                StartCoroutine(JumpFromWall());
                return;
            }

            float horizontal = Mathf.Round(Input.GetAxisRaw("Horizontal"));
            float vertical = Mathf.Round(Input.GetAxisRaw("Vertical"));

            var inputDirection = new Vector2(horizontal, vertical);

            if (playerScript.playerInAction || inputDirection == Vector2.zero)
                return;

            if (currentClimbPoint == null)
                return;

            // Climb to top.
            if (currentClimbPoint.MountPoint && inputDirection.y == 1)
            {
                StartCoroutine(ClimbToTop());
                return;
            }

            // Ledge-to-ledge parkour actions.
            var neighbour = currentClimbPoint.GetNeighbour(inputDirection);

            if (neighbour == null || neighbour.climbingPoint == null)
                return;

            if (neighbour.connectionType == ConnectionType.Jump && Input.GetButton("Jump"))
            {
                currentClimbPoint = neighbour.climbingPoint;

                if (neighbour.pointDirection.y == 1)
                {
                    InOutValue = 0.1f;
                    UpDownValue = 0.05f;
                    LeftRightValue = 0.25f;

                    StartCoroutine(
                        ClimbToLedge(
                            "ClimbUp",
                            currentClimbPoint.transform,
                            0.34f,
                            0.64f,
                            playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)
                        )
                    );
                }
                else if (neighbour.pointDirection.y == -1)
                {
                    InOutValue = 0.2f;
                    UpDownValue = 0.05f;
                    LeftRightValue = 0.25f;

                    StartCoroutine(
                        ClimbToLedge(
                            "ClimbDown",
                            currentClimbPoint.transform,
                            0.31f,
                            0.68f,
                            playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)
                        )
                    );
                }
                else if (neighbour.pointDirection.x == 1)
                {
                    StartCoroutine(
                        ClimbToLedge(
                            "ClimbRight",
                            currentClimbPoint.transform,
                            0.20f,
                            0.51f
                        )
                    );
                }
                else if (neighbour.pointDirection.x == -1)
                {
                    InOutValue = 0.1f;
                    UpDownValue = 0.04f;
                    LeftRightValue = 0.25f;

                    StartCoroutine(
                        ClimbToLedge(
                            "ClimbLeft",
                            currentClimbPoint.transform,
                            0.20f,
                            0.51f,
                            playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)
                        )
                    );
                }
            }
            else if (neighbour.connectionType == ConnectionType.Move)
            {
                currentClimbPoint = neighbour.climbingPoint;

                if (neighbour.pointDirection.x == 1)
                {
                    InOutValue = 0.2f;
                    UpDownValue = 0.03f;
                    LeftRightValue = 0.25f;

                    StartCoroutine(
                        ClimbToLedge(
                            "ShimmyRight",
                            currentClimbPoint.transform,
                            0f,
                            0.30f,
                            playerHandOffset: new Vector3(InOutValue, UpDownValue, LeftRightValue)
                        )
                    );
                }
                else if (neighbour.pointDirection.x == -1)
                {
                    InOutValue = 0.2f;
                    UpDownValue = 0.03f;
                    LeftRightValue = 0.25f;

                    StartCoroutine(
                        ClimbToLedge(
                            "ShimmyLeft",
                            currentClimbPoint.transform,
                            0f,
                            0.30f,
                            AvatarTarget.LeftHand,
                            new Vector3(InOutValue, UpDownValue, LeftRightValue)
                        )
                    );
                }
            }
        }
    }

    IEnumerator ClimbToLedge(
        string animationName,
        Transform ledgePoint,
        float compareStartTime,
        float compareEndTime,
        AvatarTarget hand = AvatarTarget.RightHand,
        Vector3? playerHandOffset = null)
    {
        if (ledgePoint == null || playerScript == null)
            yield break;

        var compareParams = new CompareTargetParameter
        {
            position = SetHandPosition(ledgePoint, hand, playerHandOffset),
            bodyPart = hand,
            positionWeight = Vector3.one,
            startTime = compareStartTime,
            endTime = compareEndTime
        };

        var requiredRot = Quaternion.LookRotation(-ledgePoint.forward);

        yield return playerScript.PerformAction(
            animationName,
            compareParams,
            requiredRot,
            true
        );

        playerScript.playerhanging = true;
    }

    Vector3 SetHandPosition(
        Transform ledge,
        AvatarTarget hand,
        Vector3? playerHandOffset)
    {
        Vector3 offsetValue = playerHandOffset ??
            new Vector3(InOutValue, UpDownValue, LeftRightValue);

        var handDirection = (hand == AvatarTarget.RightHand)
            ? ledge.right
            : -ledge.right;

        return ledge.position
            + ledge.forward * offsetValue.x
            + Vector3.up * offsetValue.y
            - handDirection * offsetValue.z;
    }

    IEnumerator JumpFromWall()
    {
        playerScript.playerhanging = false;

        yield return playerScript.PerformAction("JumpFromWall");

        playerScript.ResetRequiredRotation();
        playerScript.SetControl(true);
    }

    IEnumerator ClimbToTop()
    {
        playerScript.playerhanging = false;

        yield return playerScript.PerformAction("ClimbToTop");

        playerScript.EnableCC(true);

        yield return new WaitForSeconds(0.5f);

        playerScript.ResetRequiredRotation();
        playerScript.SetControl(true);
    }

    ClimbingPoint GetNearestClimbingPoint(Transform dropClimbPoint, Vector3 hitPoint)
    {
        if (dropClimbPoint == null)
            return null;

        var points = dropClimbPoint.GetComponentsInChildren<ClimbingPoint>();

        ClimbingPoint nearestPoint = null;
        float nearestPointDistance = Mathf.Infinity;

        foreach (var point in points)
        {
            if (point == null)
                continue;

            float distance = (point.transform.position - hitPoint).sqrMagnitude;

            if (distance < nearestPointDistance)
            {
                nearestPoint = point;
                nearestPointDistance = distance;
            }
        }

        return nearestPoint;
    }
}