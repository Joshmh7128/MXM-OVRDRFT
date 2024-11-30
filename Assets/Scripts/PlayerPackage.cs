using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPackage : MonoBehaviour
{
    private void Update()
    {
        ProcessReturnToMenu();
    }

    void ProcessReturnToMenu()
    {
        if (Input.GetButtonDown("Start"))
        {
            // SceneManager.LoadScene("CarSelect");
            // Destroy(gameObject);
        }
    }
}
