using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class RadioController : MonoBehaviour
{
    public List<AudioClip> songs;
    AudioSource audioSource;
    int currentSong; // the song we're on as an int
    public Text songTitle;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // repeat
        StartCoroutine(PlayNextSong());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            audioSource.volume += 0.1f;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            audioSource.volume -= 0.1f;
        }
    }

    // play our songs
    IEnumerator PlayNextSong()
    {
        // stop
        audioSource.Stop();

        // check our int
        if (currentSong > songs.Count)
        {
            currentSong = 0;
        }

        if (currentSong == 0)
        {
            // then shuffle the list
            var count = songs.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = songs[i];
                songs[i] = songs[r];
                songs[r] = tmp;
            }
        }

        // load a song
        audioSource.clip = songs[currentSong];

        // set our text
        songTitle.text = songs[currentSong].name;

        // play
        audioSource.Play();

        // now wait for the song to end
        yield return new WaitForSecondsRealtime(songs[currentSong].length);

        // iterate
        currentSong++;

        // repeat
        StartCoroutine(PlayNextSong());
    }
}