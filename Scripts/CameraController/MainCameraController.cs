using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public float rotationY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // get the current camera state
        var state = vcam.State;

        // extarct the rotation quaternion from the state
        var rotation = state.FinalOrientation;

        // convert rotation to euler angles
        var euler = rotation.eulerAngles;

        // get the y-axis values from the euler angle
        rotationY = euler.y;

        // round the rotation y value to the nearest integer
        var roundedRotationY = Mathf.RoundToInt(rotationY);
    }

    public Quaternion floatRotation => Quaternion.Euler(0f, rotationY, 0f);
}
