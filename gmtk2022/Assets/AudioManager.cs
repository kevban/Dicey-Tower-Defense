using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    [HideInInspector]
    public Sound curTrack;
    [HideInInspector]
    public Sound nextTrack;
    // Start is called before the first frame update
    private void Awake()
    {
        foreach (Sound s in sounds) { 
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    // Update is called once per frame

    public void Play(string name) {
        foreach (Sound s in sounds) {
            if (s.name == name) {
                s.source.Play();
            }
        }
    }

    public void PlayPitch(string name, float newPitch)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.pitch = newPitch;
                s.source.Play();
            }
        }
    }

    public void Stop(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                s.source.Stop();
            }
        }
    }

    public void StopAll() {
        foreach (Sound s in sounds)
        {
                s.source.Stop();
        }
    }

    public void  ChangeTrackAfterFinish(string name) {
        foreach (Sound s in sounds)
        {
            if (s.source.isPlaying) {
                curTrack = s;
                curTrack.source.loop = false;
            }
            if (s.name == name)
            {
                nextTrack = s;
            }
        }
        Invoke("ChangeTrack", 3f);
    }

    void ChangeTrack() {
        if (curTrack.source.isPlaying)
        {
            Invoke("ChangeTrack", 3f);
        }
        else {
            curTrack = nextTrack;
            nextTrack.source.Play();
        }
        
    }


}
