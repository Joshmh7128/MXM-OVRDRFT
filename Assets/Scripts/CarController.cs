using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    // the start of all four of our car wheel suspensions
    [SerializeField] List<Transform> suspensionPoints = new List<Transform>();
    [SerializeField] float restLength, springTravel, wheelRadius, damperStiffness, springStiffness;
    [SerializeField] Rigidbody carBody;

    // whether or not the controller has been initialized
    [SerializeField] bool controllerInitialized;

    // checks if the wheels are grounded
    private int[] wheelIsGrounded = new int[4];
    private bool isGrounded = false;

    public float moveInput, steerInput;

    [SerializeField] private float acceleration = 25f, maxSpeed = 100f, deceleration = 10f;
    [SerializeField] public Vector3 currentCarLocalVelocity = Vector3.zero;
    // our car's current speed from 0 to 1
    [SerializeField] float carVelocityRatio = 0;
    [SerializeField] Transform accelerationPoint;
    [SerializeField] public float steerStrength;
    [SerializeField] AnimationCurve turningCurve;
    [SerializeField] float dragCoefficient, normalDrag, driftDrag, dragChangeDelta;
    [SerializeField] public bool drifting;


    private void Start()
    {
        // setup our controller
        InitializeController();
    }

    private void InitializeController()
    {
        // get our car body
        carBody = GetComponent<Rigidbody>();

        // then initialize
        controllerInitialized = true;
    }

    // run our FixedUpdate Stack
    private void FixedUpdate()
    {
        // don't run if we're not initialized!
        if (!controllerInitialized) return;
        // get our input once per tick (120fps)
        GetInput();
        // then perform our stack
        ProcessSuspension();
        // check our grounded state
        GroundCheck();
        // set our drift state
        ManageDriftState();
        // then calculate our car's velocity
        CalculateCarVelocity();
        // apply our acceleration and deceleration
        ApplyMovement();
    }

    /// <summary>
    /// Handles the suspension of our car
    /// </summary>
    private void ProcessSuspension()
    {
        // apply force to all four of our suspension points
        for (int i = 0; i < suspensionPoints.Count; i++)
        {
            // store the hit
            RaycastHit hit;
            // get the maximum length of the raycast
            float maxLength = restLength + springTravel;

            // perform a raycast downwards
            if (Physics.Raycast(suspensionPoints[i].position, -suspensionPoints[i].up, out hit, maxLength + wheelRadius))
            {
                wheelIsGrounded[i] = 1;

                // set the distance of this current spring to be the hit's distance minus the wheel's radius, so that it is as the center of the wheel
                float currentSpringDistance = hit.distance - wheelRadius;
                // get the current amount that the spring is compressed by taking the rest length, subtracting the distance, then out of the spring's travel
                float springCompression = (restLength - currentSpringDistance) / springTravel;
                // build our spring velocity by getting the dot
                float springVelocity = Vector3.Dot(carBody.GetPointVelocity(suspensionPoints[i].position), suspensionPoints[i].up);
                // then add our dampening force
                float dampForce = damperStiffness * springVelocity;
                // build our spring's force
                float springForce = springStiffness * springCompression;
                // finally build our netForce using our spring force and dampener
                float netForce = springForce - dampForce;

                // now, perform the add force to the suspension
                carBody.AddForceAtPosition(netForce * suspensionPoints[i].up, suspensionPoints[i].position);

                Debug.DrawLine(suspensionPoints[i].position, suspensionPoints[i].position - suspensionPoints[i].up, Color.red);
            }
            else
            {
                wheelIsGrounded[i] = 0;
                Debug.DrawLine(suspensionPoints[i].position, suspensionPoints[i].position - suspensionPoints[i].up, Color.green);
            }
        }
    }

    /// <summary>
    /// Checks how many wheels are on the ground and sets our grounded state accordingly
    /// </summary>
    private void GroundCheck()
    {
        int tempGroundedWheels = 0;

        // check how many wheels are on the ground
        for (int i = 0; i < wheelIsGrounded.Length; i++)
            tempGroundedWheels += wheelIsGrounded[i];

        // if more than one wheel is on the ground, then we are grounded.
        if (tempGroundedWheels > 1)
            isGrounded = true;
        else
            isGrounded = false;
    }

    /// <summary>
    /// Calculates the velocity of the car each frame
    /// </summary>
    private void CalculateCarVelocity()
    {
        // returns the car's velocity in it's local axis
        currentCarLocalVelocity = transform.InverseTransformDirection(carBody.velocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;

    }

    /// <summary>
    /// Get the player's input and translate it into our move axes
    /// </summary>
    private void GetInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// Add acceleration to the rigidbody relative to the movement and forward direction of the car
    /// </summary>
    private void Acceleration()
    {
        carBody.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    /// <summary>
    /// Add deceleration so that we can more quickly slow down the car, we use a different deceleration variable to slow down the car
    /// </summary>
    private void Deceleration()
    {
        carBody.AddForceAtPosition(deceleration * moveInput * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    /// <summary>
    /// Applies the current accelerate and decelerate to the car
    /// </summary>
    private void ApplyMovement()
    {
        // only run when the car is grounded
        if (!isGrounded) return;

        Acceleration();
        Deceleration();
        ApplyTurn();
        SidewaysDrag();
    }

    /// <summary>
    /// Applies torque to our car's rigidbody
    /// </summary>
    private void ApplyTurn()
    {
        carBody.AddTorque(steerStrength * steerInput * turningCurve.Evaluate(carVelocityRatio) * Mathf.Sign(carVelocityRatio) * transform.up, ForceMode.Acceleration);
    }

    /// <summary>
    /// Properly set our sideways movement so that we don't slide everywhere
    /// </summary>
    private void SidewaysDrag()
    {
        float currentSidewaysSpeed = currentCarLocalVelocity.x;
        float dragMagnitude = -currentSidewaysSpeed * dragCoefficient;
        Vector3 dragForce = transform.right * dragMagnitude;
        carBody.AddForceAtPosition(dragForce, carBody.worldCenterOfMass, ForceMode.Acceleration);
    }

    /// <summary>
    /// Manages whether or not we're in a drift state by modifying the dragCoefficient
    /// </summary>
    private void ManageDriftState()
    {
        // get our A button input to determine our drift state
        drifting = Input.GetButton("Drift");
        // then set our drag coefficient accordingly
        // slowly adjust our drag coefficient so that we do not instantly snap to position
        if (drifting)
            dragCoefficient = Mathf.Lerp(dragCoefficient, driftDrag, dragChangeDelta * Time.deltaTime);
        else
            dragCoefficient = Mathf.Lerp(dragCoefficient, normalDrag, dragChangeDelta * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        foreach (Transform sus in suspensionPoints)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(sus.transform.position, sus.transform.position - sus.transform.up * restLength);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(sus.transform.position - sus.transform.up * restLength,(sus.transform.position - sus.transform.up * restLength) - sus.transform.up * wheelRadius);
        }
    }
}
