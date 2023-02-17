using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource _audioSource;
    private GameObject[] other;
    private bool notFirst = false;
    private void Awake()
    {
        other = GameObject.FindGameObjectsWithTag("Music");

        foreach (GameObject oneOther in other)
        {
            if (oneOther.scene.buildIndex == -1)
            {
                notFirst = true;
            }
        }

        if (notFirst == true)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }

    public void ChangeSong(AudioClip newSong)
    {
        _audioSource.clip = newSong;
        PlayMusic();
    }

    public void SetVolume(float level)
    {
        level = Mathf.Clamp(level, 0, 1);
        _audioSource.volume = level;
    }
}
