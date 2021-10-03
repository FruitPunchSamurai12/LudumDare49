using UnityEngine.Audio;
using UnityEngine;

///used in audio manager to store audio clips paired with a name

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
}
