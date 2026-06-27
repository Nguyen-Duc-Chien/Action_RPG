using UnityEngine;
using System;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public Sound[] bgmSounds;
    public Sound[] sfxSounds;

    private void Awake()
    {
        // Singleton Pattern & DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Load saved volume, default to 1 if not found
            AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(string name)
    {
        Sound s = Array.Find(bgmSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("BGM Sound: " + name + " not found!");
            return;
        }

        if (bgmSource.clip == s.clip && bgmSource.isPlaying) return;

        bgmSource.clip = s.clip;
        bgmSource.volume = s.volume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("SFX Sound: " + name + " not found!");
            return;
        }

        // PlayOneShot helps prevent sound interruptions when multiple SFX play at once
        sfxSource.PlayOneShot(s.clip, s.volume);
    }
}
