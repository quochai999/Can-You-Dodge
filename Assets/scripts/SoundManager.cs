using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField]
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        if (audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    public void Stop()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }
}
