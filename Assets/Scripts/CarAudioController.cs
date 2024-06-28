using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAudioController : MonoBehaviour
{
    // our engine
    [SerializeField] AudioSource engineSource;
    [SerializeField] float currentPitch; // our current pitch is equal to the current speed of the car
    CarController carController;

    private void Start()
    {
        // get our car controller
        carController = GetComponent<CarController>();
    }

    private void Update()
    {
        // process our engine
        ProcessEngine();
    }

    void ProcessEngine()
    {
        currentPitch = Mathf.MoveTowards(currentPitch, carController.moveInput * 3, 1f * Time.deltaTime);

        engineSource.pitch = currentPitch;

        // if we reach our max pitch, shift up
        if (currentPitch >= carController.moveInput * 3)
            currentPitch -= Random.Range(0.1f, 0.2f);
    }
}
