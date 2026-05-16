using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoDrawer : MonoBehaviour
{
    GizmoDrawer[] giz;

    private void Start()
    {
        giz = FindObjectsByType<GizmoDrawer>(FindObjectsSortMode.None);
    }

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < giz.Length; i++)
    //    {
    //        if (Vector3.Distance(transform.position, giz[i].transform.position) < 20f)
    //            Gizmos.DrawLine(transform.position, giz[i].transform.position);
    //    }
    //}
}
