using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tracks
{
    public class TrackHandler : MonoBehaviour
    {
        // the checkpoints in this track, defined in-editor
        [SerializeField] TrackCheckpoint[] trackCheckpoints;
        int currentCheckpoint; // set automatically based on which checkpoint we enter first

        public enum TrackState { idle, running } // is this track idle or running?
        TrackState state;

        private void Start()
        {
            // setup
            SetupCheckpoints();
        }

        // sets up our checkpoints
        void SetupCheckpoints()
        {
            for (int i = 0; i < trackCheckpoints.Length; i++)
            {
                // set who interacts, and where we are
                trackCheckpoints[i].handler = this;
                trackCheckpoints[i].pos = i;
            }
        }

        // called automatically by the 
        public void RunTrack(int startPos)
        {
            // are we going forwards or backwards?
            if (startPos == 0)
            {

            }
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}