using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmGameVersion
{
    public class BeatManager : MonoBehaviour
    {
        public float BPM = 140; // the beats per minute of the game
        [SerializeField] float timeBetweenBeats; // the amount of time in seconds between the beats

        [SerializeField] float lastBeatTime = 0; // 120 bpm at 4 beats is 0.5 seconds. so (120 / 60) / 4 = 0.5
        [SerializeField] Image beatThumper;
        public float beatShrinkTime = 0.1f;
        public Vector2 thumperMaxSize = Vector2.one;

        private void Start()
        {
            GetTimeBetweenBeats();
        }

        void GetTimeBetweenBeats()
        {
            // we're always in 4/4 time
            timeBetweenBeats = (60f/BPM);
        }

        // runs super fast
        private void Update()
        {
            ProcessBPM();
        }

        public void ProcessBPM()
        {
            if (Time.time > lastBeatTime + timeBetweenBeats)
            {
                // run a beat trigger
                BeatTrigger();
                // set time
                lastBeatTime = Time.time;
            }
        }

        void BeatTrigger()
        {
            // we do all our localized beat stuff in here
            beatThumper.transform.localScale = thumperMaxSize;
        }

        public void FixedUpdate()
        {
            ProcessUI();
        }

        void ProcessUI()
        {
            // shrink our beat icon
            beatThumper.transform.localScale = Vector3.MoveTowards(beatThumper.transform.localScale, Vector3.zero, beatShrinkTime * Time.fixedDeltaTime);
        }
    }

}