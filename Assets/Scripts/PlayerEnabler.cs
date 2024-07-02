using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnabler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<PlayerPackage>().gameObject.SetActive(true);
    }
}
