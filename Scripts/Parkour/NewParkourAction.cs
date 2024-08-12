using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour Menu/Create New Parkour Action")]
public class NewParkourAction : ScriptableObject
{
    [Header("Checking Obstacle Height")]
    [SerializeField] string animationName;
    [SerializeField] string barrierTag;
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;

    [Header("Rotating Player Toward Obstacle")]
    [SerializeField] bool lookAtObstacle;
    [SerializeField] float parkourActionDelay;
    public Quaternion RequiredRotation { get; set; }

    [Header("Target Matching")]
    [SerializeField] bool allowTargetMatching = true;
    [SerializeField] AvatarTarget compareBodyPart;
    [SerializeField] float compareStartTime;
    [SerializeField] float compareEndTime;
    [SerializeField] Vector3 comparePositionWeight = new Vector3(0, 1, 0);

    public Vector3 ComparePosition { get; set; }

    public bool CheckIfAvailabe(ObstacleInfo hitData, Transform player)
    {
        if(!string.IsNullOrEmpty(barrierTag)  && hitData.hitInfo.transform.tag != barrierTag)
        {
            return false;
        }
        float checkHeight = hitData.heightInfo.point.y - player.position.y;
        Debug.Log("CheckIfAvailable called for action: " + animationName);
        Debug.Log("Obstacle height: " + hitData.heightInfo.point.y);
        Debug.Log("Player height: " + player.position.y);
        Debug.Log("Check height: " + checkHeight);
        Debug.Log("Min height: " + minHeight + " Max height: " + maxHeight);

        if (checkHeight <= minHeight || checkHeight >= maxHeight)
        {
            Debug.Log("Height condition not met for action: " + animationName);
            return false;
        }

        if (lookAtObstacle)
        {
            RequiredRotation = Quaternion.LookRotation(-hitData.hitInfo.normal);
        }

        if (allowTargetMatching)
        {
            ComparePosition = hitData.heightInfo.point;
        }

        Debug.Log("Height condition met for action: " + animationName);
        return true;
    }

    public string AnimationName => animationName;
    public bool LookAtObstacle => lookAtObstacle;

    public bool AllowTargetMatching => allowTargetMatching;
    public AvatarTarget CompareBodyPart => compareBodyPart;
    public float CompareStartTime => compareStartTime;
    public float CompareEndTime => compareEndTime;
    public Vector3 ComparePositionWeight => comparePositionWeight;
    public float ParkourActionDelay => parkourActionDelay;
}
