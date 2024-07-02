using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // script manages the main menu
    public enum menuStates { start, carSelect }

    public menuStates menuState; // the current menu state

    // our camera's variables
    [SerializeField] Transform cameraTransform; // the actual camera
    [SerializeField] Transform fauxTransform; // the one that moves quickly that we lerp to
    [SerializeField] float cameraLerpSpeed;
    [SerializeField] Vector3 cameraTargetPositon, cameraLookatPosition;
    [SerializeField] Transform startCameraPosition, startCameraLookat;
    [SerializeField] Transform selectCameraLookat;

    // now for our carousel
    [SerializeField] Transform[] carPresentationPositions, carPresentationLookats;
    [SerializeField] int currentCar;
    [SerializeField] GameObject[] carPrefabs;
    float lastMove;
    [SerializeField] float cooldown;

    private void Update()
    {
        // process our camera
        ProcessCamera();

        switch (menuState)
        {
            case menuStates.start:
                ProcessStartState();
                break;  

            case menuStates.carSelect:
                ProcessCarSelect();
                break;
        }
    }

    // processess our start state
    void ProcessStartState()
    {
        // while in this state, set our target camera position and lookat positions
        cameraTargetPositon = startCameraPosition.position;
        cameraLookatPosition = startCameraLookat.position;  

        if (Input.GetButtonDown("Drift"))
        {
            menuState = menuStates.carSelect;
            // then reset our car state to the first car
            currentCar = 0;
        }
    }

    // process our car carousel
    void ProcessCarSelect()
    {
        // while in this state, set our target camera position and lookat positions
        cameraTargetPositon = carPresentationPositions[currentCar].position;
        // then set the lookat
        cameraLookatPosition = carPresentationLookats[currentCar].position;

        // if we press b we go back to the start
        if (Input.GetButtonDown("B"))
        {
            menuState = menuStates.start;
        }

        Debug.Log(Input.GetAxis("Horizontal"));

        // navigate our carousel to the right
        if (Input.GetAxis("Horizontal") > 0.5)
            Move(true);
        if (Input.GetAxis("Horizontal") < -0.5) 
            Move(false);

        // if we select, instantiate a car and deactivate it, to be activated when we enter the play space
        if (Input.GetButtonDown("Drift"))
        {
            var car = Instantiate(carPrefabs[currentCar], Vector3.zero, Quaternion.identity, null);
            DontDestroyOnLoad(car);
            // load
            SceneManager.LoadScene("PlaySpace");
        }
    }

    // move right
    void Move(bool right)
    {
        Debug.Log("attempting move...");

        // can we move?
        if (Time.time > lastMove + cooldown)
        {
            Debug.Log("setting move");
            if (right)
            {
                currentCar++;
                // but catch it otherwise
                if (currentCar >= carPresentationPositions.Length)
                    currentCar = 0;
            }
            else
            {
                currentCar--;
                // but catch it otherwise
                if (currentCar < 0)
                    currentCar = carPresentationPositions.Length;
            }

            lastMove = Time.time;
        }
    }

    void ProcessCamera()
    {
        // lerp to our intended position
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraTargetPositon, cameraLerpSpeed * Time.deltaTime);

        // loop at our intended position
        fauxTransform.position = cameraTransform.position;
        fauxTransform.LookAt(cameraLookatPosition);

        // lerp our look
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, fauxTransform.rotation, cameraLerpSpeed * Time.deltaTime);
    }
}
