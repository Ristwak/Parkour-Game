using System;
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

    void Update()
    {
        if (Input.GetButton("Jump") && !playerScript.playerInAction && !playerScript.playerhanging)
        {
            var hitData = environmentChecker.CheckObstacle();
            Debug.Log("Jump enter");

            if (hitData.hitFound)
            {
                Debug.Log("Obstacle detected: " + hitData.hitInfo.transform.name);
                foreach (var action in newParkourActions)
                {
                    Debug.Log("Checking action: " + action.AnimationName);
                    if (action.CheckIfAvailabe(hitData, transform))
                    {
                        Debug.Log("Performing Parkour Action: " + action.AnimationName);
                        // perform parkour action
                        StartCoroutine(PerformParkourAction(action));
                        break;
                    }
                }
            }
        }

        if (playerScript.playerOnLedge && !playerScript.playerInAction && Input.GetButton("Jump"))
        {
            if (playerScript.LedgeInfo.angle <= 50)
            {
                playerScript.playerOnLedge = false;
                StartCoroutine(PerformParkourAction(jumpDownParkourActions));
            }
        }
    }

    IEnumerator PerformParkourAction(NewParkourAction action)
    {
        playerScript.SetControl(false);

        CompareTargetParameter compareTargetParameter = null;
        if(action.AllowTargetMatching)
        {
            compareTargetParameter = new CompareTargetParameter()
            {
                position = action.ComparePosition,
                bodyPart = action.CompareBodyPart,
                positionWeight = action.ComparePositionWeight,
                startTime = action.CompareStartTime,
                endTime = action.CompareEndTime
            };
        }

        yield return playerScript.PerformAction(action.AnimationName, compareTargetParameter, action.RequiredRotation, action.LookAtObstacle, action.ParkourActionDelay);
        playerScript.SetControl(true);
    }

    void CompareTarget(NewParkourAction action)
    {
        animator.MatchTarget(action.ComparePosition, transform.rotation, action.CompareBodyPart, new MatchTargetWeightMask(action.ComparePositionWeight, 0), action.CompareStartTime, action.CompareEndTime);
    }
}
