using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{

    public static AudioHandler Instance;
    public AudioSource audioSource;
    public AudioClip CorrectAudio, WrongAudio, FlipAudio, OverAudio;

    private void Awake()
    {
        Instance = this;
    }
}
