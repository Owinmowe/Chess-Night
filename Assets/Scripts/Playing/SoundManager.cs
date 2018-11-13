using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pointerSound;
    [SerializeField] AudioClip selectSound;
    [SerializeField] AudioClip[] shatterSounds;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioClip flickerSound;

    const string VOLUME = "Volume";

    public static SoundManager Instance { set; get; }

    void Awake ()
    {
        Instance = this;	
	}

    public void PointSound()
    {
        audioSource.PlayOneShot(pointerSound);
    }

    public void ClickSound()
    {
        audioSource.PlayOneShot(pointerSound);
    }

    public void ShatterSound()
    {
        audioSource.PlayOneShot(shatterSounds[Random.Range(0, 3)]);
    }

    public void FlickerSound()
    {
        audioSource.PlayOneShot(flickerSound);
    }

    private void StartingSettings()
    {
        if (PlayerPrefs.HasKey(VOLUME))
        {
            audioMixer.SetFloat(VOLUME, PlayerPrefs.GetFloat(VOLUME));
        }
    }
}
