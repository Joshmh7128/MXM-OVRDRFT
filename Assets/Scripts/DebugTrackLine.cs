using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DebugTrackLine : MonoBehaviour
{
    [SerializeField] LineRenderer lr;
    [SerializeField] List<Transform> points = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }
}
