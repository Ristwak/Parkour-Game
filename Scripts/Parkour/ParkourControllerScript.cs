using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourControllerScript : MonoBehaviour
{
    public EnvironmentChecker environmentChecker;
    public Animator animator;
    public PlayerScript playerScript;

    [SerializeField] NewParkourAction jumpDownParkourActions;

    [Header("Parkor Action Area")]
    public List<NewParkourAction> newParkourActions;

    void Awake()
    {
        if (environmentChecker == null)
            environmentChecker = GetComponent<EnvironmentChecker>();

        if (playerScript == null)
            playerScript = GetComponent<PlayerScript>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerScript == null || environmentChecker == null)
            return;

        if (Input.GetButton("Jump") &&
            !playerScript.playerInAction &&
            !playerScript.playerhanging)
        {
            var hitData = environmentChecker.CheckObstacle();

            if (hitData.hitFound && hitData.heightHitFound)
            {
                if (newParkourActions != null)
                {
                    foreach (var action in newParkourActions)
                    {
                        if (action == null)
                            continue;

                        if (action.CheckIfAvailabe(hitData, transform))
                        {
                            StartCoroutine(PerformParkourAction(action));
                            break;
                        }
                    }
                }
            }
        }

        if (playerScript.playerOnLedge &&
            !playerScript.playerInAction &&
            Input.GetButton("Jump"))
        {
            if (jumpDownParkourActions != null &&
                playerScript.LedgeInfo.angle <= 50)
            {
                playerScript.playerOnLedge = false;
                StartCoroutine(PerformParkourAction(jumpDownParkourActions));
            }
        }
    }

    IEnumerator PerformParkourAction(NewParkourAction action)
    {
        if (action == null || playerScript == null)
            yield break;

        playerScript.SetControl(false);

        CompareTargetParameter compareTargetParameter = null;

        if (action.AllowTargetMatching)
        {
            compareTargetParameter = new CompareTargetParameter
            {
                position = action.ComparePosition,
                bodyPart = action.CompareBodyPart,
                positionWeight = action.ComparePositionWeight,
                startTime = action.CompareStartTime,
                endTime = action.CompareEndTime
            };
        }

        yield return playerScript.PerformAction(
            action.AnimationName,
            compareTargetParameter,
            action.RequiredRotation,
            action.LookAtObstacle,
            action.ParkourActionDelay
        );

        playerScript.SetControl(true);
    }

    void CompareTarget(NewParkourAction action)
    {
        if (action == null || animator == null)
            return;

        animator.MatchTarget(
            action.ComparePosition,
            transform.rotation,
            action.CompareBodyPart,
            new MatchTargetWeightMask(action.ComparePositionWeight, 0),
            action.CompareStartTime,
            action.CompareEndTime
        );
    }
}