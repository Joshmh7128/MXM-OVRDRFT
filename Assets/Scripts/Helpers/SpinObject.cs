using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    [SerializeField] float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + speed * Time.deltaTime, 0);
    }
}
