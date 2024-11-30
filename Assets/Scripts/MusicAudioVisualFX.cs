using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MusicAudioVisualFX : MonoBehaviour
{
    public AudioSource source;
    float updateStep = 0.1f;
    public int sampleDataLength = 1024;
    [SerializeField] float scaleMult, lerpSpeed;
    [SerializeField] Volume ourVolume;

    float currentUpdateTime = 0f;

    [SerializeField] float clipLoudness, loudnessMultiplier;
    float[] clipSampleData;
    [SerializeField] float maxBloom, minBloom;

    private void Awake()
    {
        clipSampleData = new float[sampleDataLength];
    }

    private void FixedUpdate()
    {
        // calculate the loudness
        CalculateLoudness();
        // process the VFX changes
        ProcessVFXChanges();
    }

    // calculate how loud we are
    void CalculateLoudness()
    {
        currentUpdateTime += Time.deltaTime;

        try
        {
            if (currentUpdateTime > updateStep)
                currentUpdateTime = 0f;


            if (source == null)
            {

                source = FindObjectOfType<RadioController>().audioSource;
            }

            if (source.timeSamples > 1030 && source.clip != null)
                try { source.clip.GetData(clipSampleData, source.timeSamples); }
                catch { }

            clipLoudness = 0f;

            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
        }
        catch { }


        clipLoudness /= sampleDataLength;

        clipLoudness *= loudnessMultiplier;
    }

    // now output that as the V on a material's emission intensity
    void ProcessVFXChanges()
    {
        ourVolume.weight = (clipLoudness * loudnessMultiplier) + minBloom;
    }
}