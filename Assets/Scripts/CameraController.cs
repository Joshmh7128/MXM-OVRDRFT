using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    // the object we intend to follow
    [SerializeField] Transform followPoint, rotationContainer, photomodeContainer;
    [SerializeField] float followSpeed, rotationSpeed, localRotSpeed;
    [SerializeField] Vector3 currentLocalRot;
    [SerializeField] Camera cam;

    [SerializeField] bool photomode, initPhoto; // are we in photomode

    // check for photomode
    void Update()
    {
        if (Input.GetButtonDown("Start"))
        {
            photomode = !photomode;
            Debug.Log("photomode hit");

            if (!photomode)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("Track_001");
            }
        }

        if (photomode)
        {
            if (!initPhoto)
            {
                transform.localEulerAngles = Vector3.zero;
                rotationContainer.localEulerAngles = Vector3.zero;
            }
            initPhoto = true;
            Time.timeScale = 0.00001f;
            ProcessPhotomode();
        }
    }

    private void FixedUpdate()
    {
        // process our rig position and rotation
        if (!photomode)
        {
            ProcessPosition();
            ProcessRotation();
            Time.timeScale = 1f;
        }
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

    // process our photomode
    private void ProcessPhotomode()
    {
        Debug.Log(Input.GetAxis("Left Stick X") + "x");
        Debug.Log(Input.GetAxis("Left Stick Y") + "y");
        // modify the camera
        if (Mathf.Abs(Input.GetAxis("Right Stick X")) > 0.1f || Mathf.Abs(Input.GetAxis("Right Stick Y")) > 0.1f)
            rotationContainer.localEulerAngles += new Vector3(Input.GetAxis("Right Stick Y") * (cam.fieldOfView / 90), Input.GetAxis("Right Stick X") * (cam.fieldOfView / 90), 0);

        if (Mathf.Abs(Input.GetAxis("Left Stick Y")) > 0.1f || Mathf.Abs(Input.GetAxis("Left Stick X")) > 0.1f)
        {
            Vector3 mov = new Vector3(Input.GetAxis("Left Stick X") * 0.1f, 0, -Input.GetAxis("Left Stick Y") * 0.1f);
            Vector3 dirmov = (rotationContainer.right * mov.z) + (rotationContainer.forward * -mov.x);
            dirmov.y = 0;
            photomodeContainer.localPosition += dirmov;
        }

        // adjust the camera fov
        cam.fieldOfView += Input.GetAxis("DPAD Vertical");

        if (Input.GetButton("Right Bumper"))
            photomodeContainer.localPosition += new Vector3(0, 0.1f);
        if (Input.GetButton("Left Bumper"))
            photomodeContainer.localPosition += new Vector3(0, -0.1f);
    }
}
