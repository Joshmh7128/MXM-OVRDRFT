using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Tracks
{
    public class TrackCheckpoint : MonoBehaviour
    {
        // our handler, set automatically by the TrackHandler we're part of
        [HideInInspector] public TrackHandler handler;
        [SerializeField] public int pos;

        // our game objects for simple visuals
        [SerializeField] GameObject markerParent; // lights to show we're active

        public void EnablePoint()
        {
            markerParent.SetActive(true);
        }

        public void DisablePoint()
        {
            markerParent.SetActive(false);
        }

        void PointFired()
        {
            handler.PointFired(pos);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("trigger hit");

            if (other.gameObject.transform.tag == "Player")
            {
                PointFired();
            }
        }
    }

}