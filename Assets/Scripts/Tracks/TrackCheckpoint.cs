using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tracks
{
    public class TrackCheckpoint : MonoBehaviour
    {
        // our handler, set automatically by the TrackHandler we're part of
        public TrackHandler handler;
        public int pos; 
    }

}