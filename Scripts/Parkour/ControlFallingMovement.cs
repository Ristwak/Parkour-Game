using UnityEngine;

public class ControlFallingMovement : StateMachineBehaviour
{
    PlayerScript playerScript;

    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo animatorStateInfo,
        int layerIndex)
    {
        if (playerScript == null && animator != null)
            playerScript = animator.GetComponent<PlayerScript>();

        if (playerScript != null)
            playerScript.HasPlayerControl = false;
    }

    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo animatorStateInfo,
        int layerIndex)
    {
        if (playerScript != null)
            playerScript.HasPlayerControl = true;
    }
}