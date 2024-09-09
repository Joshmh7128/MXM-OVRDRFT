using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobObject : MonoBehaviour
{
    Vector3 bobAdd, originalPos;
    [SerializeField] float x, y, z, speed;

    private void Start()
    {
        originalPos = transform.position;
    }

    private void FixedUpdate()
    {
        bobAdd.x = Mathf.Sin(Time.time * speed) * x;
        bobAdd.y = Mathf.Sin(Time.time * speed) * y;
        bobAdd.z = Mathf.Sin(Time.time * speed) * z;
        // set the position
        transform.position = originalPos + bobAdd;
    }
}
