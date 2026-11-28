using UnityEngine;
using Cinemachine;

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
        if (vcam == null)
            return;

        var state = vcam.State;
        var rotation = state.FinalOrientation;
        var euler = rotation.eulerAngles;

        rotationY = euler.y;
    }

    public Quaternion floatRotation => Quaternion.Euler(0f, rotationY, 0f);
}