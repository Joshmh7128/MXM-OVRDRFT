using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CarAudioController : MonoBehaviour
{
    // our engine
    [SerializeField] AudioSource engineSource;
    [SerializeField] AudioClip accel, idle;
    CarController carController;
    [SerializeField] float gearMax, gearCurrentProgress, speed, bounceAmount;
    [SerializeField] int currentGear; // our current gear and how much more we move up when we start a new gear
    private void Start()
    {
        // get our car controller
        carController = GetComponent<CarController>();
        engineSource.clip = accel;
        gearMax = accel.length;
    }

    private void FixedUpdate()
    {
        // process our engine
        ProcessEngine();
    }

    void ProcessEngine()
    {

        if (!engineSource.isPlaying)
        {
            engineSource.Play();
        }

        // if we ever hit the top of the gear, bounce on it
        if (gearCurrentProgress >= gearMax)
        {
            // if we're not drifting shift up
            if (!carController.drifting)
                ShiftUp();
            // if we are drifting...
            else
            {
                gearCurrentProgress -= bounceAmount;
                engineSource.time = gearCurrentProgress;
            }
        }

        // while accelerating, play the accel clip. when we decel, move backwards through the clip, then replay at the same point one accel
        if (carController.moveInput > 0)
        {
            gearCurrentProgress += speed * Time.deltaTime;
            engineSource.pitch = speed * 1;
            engineSource.clip = accel;
            // engineSource.time = gearCurrent;
        }

        // if we're decelerating
        if (carController.moveInput <= 0)
        {
            gearCurrentProgress -= Time.deltaTime;
            engineSource.pitch = -1;
        }

        if (gearCurrentProgress <= 0)
        {
            // if we are not in 1st gear, shift down
            if (currentGear != 0)
            {
                ShiftDown();
            }

            gearCurrentProgress = 0.1f;
        }

        // if we're low and idling
        if (gearCurrentProgress < 0.5f && carController.moveInput < 0.1)
        {
        }

        if (engineSource.pitch == 0)
        {
            engineSource.pitch = 0.1f;
        }
    }

    // shift up
    void ShiftUp()
    {

        // reduce our gearCurrent time by our current gear * the ratio increase
        gearCurrentProgress = gearMax * 0.5f + (currentGear * 0.1f);

        currentGear++;
        // set the time
        engineSource.time = gearCurrentProgress;
    }

    void ShiftDown()
    {
        // reduce our gearCurrent time by our current gear * the ratio increase
        gearCurrentProgress = gearMax;
        currentGear = 0;

        // set the time
        engineSource.time = gearCurrentProgress;
    }
}

