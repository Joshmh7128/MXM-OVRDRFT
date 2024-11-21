using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // the object we intend to follow
    [SerializeField] Transform followPoint, rotationContainer;
    [SerializeField] float followSpeed, rotationSpeed, localRotSpeed;
    [SerializeField] Vector3 currentLocalRot;

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

        Vector3 dir = new Vector3(0, 0, 0);
        if (Mathf.Abs(Input.GetAxis("Right Stick X")) > 0.1f || Mathf.Abs(Input.GetAxis("Right Stick Y")) > 0.1f)
        // get the direction from the zero point to the sticks
            dir = new Vector3(Input.GetAxis("Right Stick X"), 0, Input.GetAxis("Right Stick Y")) - Vector3.zero;
        
        // lerp to the rotation
        if (dir != Vector3.zero)
            rotationContainer.localRotation = Quaternion.Slerp(rotationContainer.localRotation, Quaternion.LookRotation(dir,Vector3.up), 10 * Time.fixedDeltaTime);
        else
            rotationContainer.localRotation = Quaternion.Slerp(rotationContainer.localRotation, Quaternion.Euler(Vector3.zero), 10 * Time.fixedDeltaTime);

    }
}
