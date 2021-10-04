using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //all the background music
    [SerializeField]
    Sound[] bgMusic;

    //all the sound effects
    [SerializeField]
    Sound[] soundEffects;

    //different audio sources for bg and fx
    [SerializeField]
    AudioSource bgSource;

    [SerializeField]
    AudioSource[] fxSources;

    [SerializeField] float fixMusicTransition = 0.5f;

    public static AudioManager Instance;//simpleton
    public static bool bgON = true;//volume adjustable from settings
    public static bool sfxON = true;
    public float SFXVolume { get; private set; }
    public float BGVolume { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        var clip1 = GetBGMusic("BGIntro");
        var clip2 = GetBGMusic("BGLoop");
        float secs = clip1.length;
        StartCoroutine(PlayBGAfterSeconds(secs, clip1, clip2));
    }

    IEnumerator PlayBGAfterSeconds(float seconds,AudioClip clip1, AudioClip clip2)
    {
        bgSource.clip = clip1;
        bgSource.Play();
        yield return new WaitForSeconds(seconds-fixMusicTransition);
        bgSource.clip = clip2;
        bgSource.Play();
    }

    public void MuteSound()
    {
        MuteBGMusic();
        MuteSoundEffects();
    }

    public void UnMuteSound()
    {
        UnMuteBGMusic();
        UnMuteSoundEffects();
    }

    public void MuteBGMusic()
    {
        BGVolume = 0;
        bgSource.volume = BGVolume;
        bgON = false;
    }

    public void UnMuteBGMusic()
    {
        BGVolume = 1;
        bgSource.volume = BGVolume;
        bgON = true;
    }

    public void MuteSoundEffects()
    {       
        sfxON = false;
        SFXVolume = 0;

    }

    public void UnMuteSoundEffects()
    {
        sfxON = true;
        SFXVolume = 1;
    }

    //find and play bg music using a string 
    public void PlayBGMusic(string name)
    {
        Sound s = new Sound();
        foreach (Sound sound in bgMusic)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if (bgSource.clip != s.clip)
        {
            bgSource.clip = s.clip;
            bgSource.Play();
        }
    }

    public void StopBGMusic()
    {
        bgSource.Stop();
    }

    //this finds and plays a sound effect using the audio manager audio source
    public void PlaySoundEffect(string name,Vector3 position)
    {
        Sound s = new Sound();
        foreach (Sound sound in soundEffects)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if(s.clip==null)
        {
            Debug.LogError("Couldnt find sound");
            return;
        }
        foreach(AudioSource source in fxSources)
        {
            if(!source.isPlaying)
            {
                source.transform.position = position;
                source.PlayOneShot(s.clip,SFXVolume);
                return;
            }
        }
       
    }

    public void PlaySoundEffectInSpecificSource(string name, int sourceIndex)
    {
        Sound s = new Sound();
        foreach (Sound sound in soundEffects)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if(sourceIndex<fxSources.Length && !fxSources[sourceIndex].isPlaying)
        {
            fxSources[sourceIndex].PlayOneShot(s.clip, SFXVolume);
        }
    }


    public void ChangeSFXVolume(float volume)
    {
        SFXVolume = volume;
    }

    public void ChangeBGVolume(float volume)
    {
        BGVolume = volume;
        bgSource.volume = BGVolume;
    }

    //this returns the audio clip of a sound effect for other objects to play
    public AudioClip GetSoundEffect(string name)
    {
        Sound s = new Sound();
        foreach (Sound sound in soundEffects)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if (s != null)
        {
            return s.clip;
        }
        else
        {
            return null;
        }
    }

    public AudioClip GetBGMusic(string name)
    {
        Sound s = new Sound();
        foreach (Sound sound in bgMusic)
        {
            if (sound.name == name)
            {
                s = sound;
                break;
            }
        }
        if (s != null)
        {
            return s.clip;
        }
        else
        {
            return null;
        }
    }
}
