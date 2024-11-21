using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CosmeticCarController : MonoBehaviour
{
    // first two wheels are the front two wheels
    [SerializeField] Transform[] wheels;
    [SerializeField] Transform[] frontWheels;
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
        HandleWheelHeat();
    }

    /// <summary>
    /// Turns the front two wheels
    /// </summary>
    void ApplyTurn()
    {
        // turn the front two wheels
        for (int i = 0; i < 2; i++)
        {
            float steerInput = Mathf.Abs(carController.steerInput) > 0.1 ? carController.steerInput : 0;
            float angle = steerInput / maxSteerAngle;
            Debug.Log(angle);
            Vector3 currentAngles = frontWheels[i].localEulerAngles;
            float targetAngle = angle * maxSteerAngle * carController.steerStrength;
            targetAngle = angle == 0 ? 0 : targetAngle; // so that our target angle cannot be multiplied by 0
            frontWheels[i].localRotation = Quaternion.Euler(new Vector3(currentAngles.x, targetAngle, currentAngles.z));
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
            // add the speed
            wheels[i].rotation *= Quaternion.Euler(speed, 0, 0);
        }
    }

    void HandleParticles()
    {
        if (carController.isGrounded)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                float rOT = carController.drifting ? 200 : 0;
                var system = particleSystems[i].emission;
                system.rateOverTime = rOT;

            }
        }
        else
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                float rOT = 0;
                var system = particleSystems[i].emission;
                system.rateOverTime = rOT;
            }
        }
    }

    [SerializeField] float minHeat, maxHeat, wheelHeat, heatRate, coolRate;
    [SerializeField] Renderer[] heatRenderers;
    // makes the wheels glow red after a long drift
    void HandleWheelHeat()
    {
        // while drifting, increase our heat
        if (carController.drifting)
            wheelHeat += Time.deltaTime * heatRate;
        else
            wheelHeat -= Time.deltaTime * coolRate;

        //wheelHeat = Mathf.Clamp(wheelHeat, minHeat, maxHeat);

        // set the materials
        if (heatRenderers.Length > 0)
        foreach (Renderer r in heatRenderers)
        {
            r.sharedMaterial.SetColor("_EmissiveColor", Color.red * wheelHeat);
        }
    }
}
