using UnityEngine.Audio;
using System;
using UnityEngine;

[System.Serializable]
public class Music
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    public bool mute;
    public bool playOnAwake;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    public bool loop;
    public bool mute;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public Music[] music;
    public Sound[] sounds;
    public bool isMusicMute = false;
    public bool isSoundMute = false;

    private void Awake()
    {
        // Audio Management
        foreach (Music m in music)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.clip;
            m.source.volume = m.volume;
            m.source.playOnAwake = m.playOnAwake;
            m.source.mute = m.mute;
            m.source.loop = m.loop;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.mute = s.mute;
        }
    }

    private void Start()
    {
        PlayMusic("Menu", 0.6f);
    }

    public void Play(string name, float pitch = 1f)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("AudioManager: Clip [" + name + "] does not exist. Typo?");
            return;
        }

        s.pitch = pitch;
        s.source.Play();

        // if (!s.source.isPlaying)
        // {
        //     s.pitch = pitch;
        //     s.source.Play();
        // }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("AudioManager: Clip [" + name + "] does not exist. Typo?");
            return;
        }
        s.source.Stop();
    }

    public void StopAllSound()
    {
        foreach (Sound s in sounds)
        {
            s.source.Stop();
        }
    }

    public void ToggleAllSound()
    {
        isSoundMute = !isSoundMute;

        foreach (Sound s in sounds)
            s.source.mute = isSoundMute;
    }

    public void ToggleAllMusic()
    {
        isMusicMute = !isMusicMute;

        foreach (Music m in music)
            m.source.mute = isMusicMute;
    }

    public void StopAllMusic()
    {
        foreach (Music m in music)
        {
            m.source.Stop();
        }

        // Cancels all Invoke call on MixMusic;
        CancelInvoke();
    }

    public void PlayMusic(string name, float pitch = 1)
    {
        Music m = Array.Find(music, music => music.name == name);
        if (m == null)
        {
            Debug.LogWarning("AudioManager: Clip [" + name + "] does not exist. Typo?");
            return;
        }
        m.source.pitch = pitch;
        m.source.Play();
    }

    public void MixMusic()
    {
        // Range Skips the first Index [0] which is the Menu BG
        Music m = music[UnityEngine.Random.Range(1, music.Length)];
        m.source.Play();
        Invoke("MixMusic", m.clip.length);
    }
}
