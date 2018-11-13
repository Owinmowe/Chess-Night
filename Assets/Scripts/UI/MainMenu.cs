using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour {

    [SerializeField] AudioClip MenuMove;
    [SerializeField] AudioClip MenuSelect;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] AudioMixer audioMixer;

    const string VOLUME = "Volume";

    private void Start()
    {
        StartingSettings();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MoveMenuSound()
    {
        audioSource.PlayOneShot(MenuMove);
    }

    public void MoveSelectSound()
    {
        audioSource.PlayOneShot(MenuSelect);
    }

    private void StartingSettings()
    {
        if (PlayerPrefs.HasKey(VOLUME))
        {
            audioMixer.SetFloat(VOLUME, PlayerPrefs.GetFloat(VOLUME));
        }
    }
}
