using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // the object we intend to follow
    [SerializeField] Transform followPoint;
    [SerializeField] float followSpeed, rotationSpeed;

    private void FixedUpdate()
    {
        // process our rig position and rotation
        ProcessPosition();
        ProcessRotation();
    }

    /// <summary>
    /// Follow our follow point
    /// </summary>
    private void ProcessPosition()
    {
        if (Vector3.Distance(transform.position, followPoint.position) > 0.1f)
            transform.position = Vector3.Lerp(transform.position, followPoint.position, followSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Process the rotation of our camera rig
    /// </summary>
    private void ProcessRotation()
    {
        // get our rotation too and from the car's rotation and extrapolate
        Vector3 fromToRotation = Vector3.Lerp(transform.eulerAngles, followPoint.eulerAngles, rotationSpeed);
        // clear the other dimensions of this rotation as we only want the y axis to move
        fromToRotation.x = 0; fromToRotation.z = 0;
        transform.rotation = Quaternion.Euler(fromToRotation);
    }
}
