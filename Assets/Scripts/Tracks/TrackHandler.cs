using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tracks
{
    public class TrackHandler : MonoBehaviour
    {
        // the checkpoints in this track, defined in-editor
        [SerializeField] TrackCheckpoint[] trackCheckpoints;
        int expectedCheckpoint; // waht checkpoint are we looking for next?

        public enum TrackState { idle, runningForward, runningBackward } // is this track idle or running?
        [SerializeField] TrackState state;

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

        // called automatically by the checkpoints whenever they are interacted with
        public void PointFired(int currentPos)
        {
            Debug.Log("Point Fired");

            // if we are idle and we hit the first or last checkpoint, run the track forward or backward
            if (currentPos == 0 && state == TrackState.idle)
            {
                state = TrackState.runningForward;
                // we want to go forward
                expectedCheckpoint = 1;
            }

            if (currentPos == trackCheckpoints.Length - 1 && state == TrackState.idle)
            {
                state = TrackState.runningBackward;
                // we want to go backward
                expectedCheckpoint = trackCheckpoints.Length - 2;
            }

            // else, if we get any other number, check to see what the expected number is and if we skipped anything
            if (currentPos == expectedCheckpoint)
            {
                if (state == TrackState.runningForward) expectedCheckpoint++;
                if (state == TrackState.runningBackward) expectedCheckpoint--;

                // then ask the track to enable JUST the checkpoint we want
                foreach (TrackCheckpoint checkpoint in trackCheckpoints)
                {
                    checkpoint.DisablePoint();
                    // then if this is the point we want, enable it
                    if (checkpoint.pos == expectedCheckpoint)
                        checkpoint.EnablePoint();
                }
            } 

            // for ending the track
            if (state == TrackState.runningForward && currentPos == trackCheckpoints.Length - 1 || state == TrackState.runningBackward && currentPos == 0)
            {
                state = TrackState.idle;
                // enable the start and ends
                trackCheckpoints[0].EnablePoint();
                trackCheckpoints[trackCheckpoints.Length - 1].EnablePoint();
            }
        }

        // end the track
        void End(bool honorable)
        {
            if (!honorable) 
            { 
                // do something
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}