using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CosmeticCarController : MonoBehaviour
{
    // first two wheels are the front two wheels
    [SerializeField] Transform[] wheels;
    CarController carController;
    [SerializeField] float maxSteerAngle; // the maximum angle to rotate the wheels to on the y axis
    [SerializeField] ParticleSystem[] particleSystems;
    // get our car controller
    private void Start()
    {
        carController = GetComponent<CarController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyTurn();
        ApplyWheelSpin();
        HandleParticles();
    }

    /// <summary>
    /// Turns the front two wheels
    /// </summary>
    void ApplyTurn()
    {
        // turn the front two wheels
        for (int i = 0; i < 2; i++)
        {
            float angle = carController.steerInput / maxSteerAngle;
            Vector3 currentAngles = wheels[i].localEulerAngles;
            wheels[i].localEulerAngles = new Vector3(currentAngles.x, angle * maxSteerAngle * carController.steerStrength, currentAngles.z);
        }
    }

    /// <summary>
    /// Applies spin to the wheels when the car is moving
    /// </summary>
    void ApplyWheelSpin()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            float speed = carController.currentCarLocalVelocity.z;
            wheels[i].localEulerAngles += new Vector3(speed * 1000, 0, 0);
        }
    }

    void HandleParticles()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            float rOT = carController.drifting ? 200 : 0;
            var emission = particleSystems[i].emission;
            emission.rateOverTime = rOT;
        }
    }
}