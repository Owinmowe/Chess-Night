using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class OptionMenu : MonoBehaviour {

    [SerializeField] TMP_Dropdown cameraDropdown;
    [SerializeField] TMP_Dropdown modelDetailsDropdown;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Slider volumeSlider;
    [SerializeField] AudioClip MenuMove;
    [SerializeField] AudioClip MenuSelect;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] TextMeshProUGUI fullscreenButtonText;
    Resolution[] resolutions;

    const string VOLUME = "Volume";
    const string MODEL_DETAILS = "ModelDetails";
    const string CAMERA_MODE = "Camera";
    const string LANGUAGE = "Language";

    private void OnEnable()
    {
        StartingSettings();
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
        if (PlayerPrefs.HasKey(MODEL_DETAILS))
        {
            modelDetailsDropdown.value = PlayerPrefs.GetInt(MODEL_DETAILS);
        }

        if (PlayerPrefs.HasKey(CAMERA_MODE))
        {
            cameraDropdown.value = PlayerPrefs.GetInt(CAMERA_MODE);
        }
        if (PlayerPrefs.HasKey(VOLUME))
        {
            volumeSlider.value = PlayerPrefs.GetFloat(VOLUME);
        }
        if (Screen.fullScreen == true)
        {
            if(PlayerPrefs.GetInt(LANGUAGE) == 0)
            {
                fullscreenButtonText.text = "Window mode";
            }
            else if (PlayerPrefs.GetInt(LANGUAGE) == 1)
            {
                fullscreenButtonText.text = "Modo Ventana";
            }
        }
        else
        {
            if (PlayerPrefs.GetInt(LANGUAGE) == 0)
            {
                fullscreenButtonText.text = "FullScreen Mode";
            }
            else if (PlayerPrefs.GetInt(LANGUAGE) == 1)
            {
                fullscreenButtonText.text = "Pantalla Completa";
            }
        }

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options); //dropdowns can't use arrays, so i make a list and iterate it.
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetCameraView()
    {
        if (cameraDropdown.value == 0)
        {
            PlayerPrefs.SetInt(CAMERA_MODE, 0);
            PlayerPrefs.Save();
        }
        else if (cameraDropdown.value == 1)
        {
            PlayerPrefs.SetInt(CAMERA_MODE, 1);
            PlayerPrefs.Save();
        }
        else if (cameraDropdown.value == 2)
        {
            PlayerPrefs.SetInt(CAMERA_MODE, 2);
            PlayerPrefs.Save();
        }
        else if (cameraDropdown.value == 3)
        {
            PlayerPrefs.SetInt(CAMERA_MODE, 3);
            PlayerPrefs.Save();
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat(VOLUME, volume);
        PlayerPrefs.SetFloat(VOLUME, volume);
        PlayerPrefs.Save();
    }

    public void ToogleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        if (Screen.fullScreen == true)
        {
            if (PlayerPrefs.GetInt(LANGUAGE) == 0)
            {
                fullscreenButtonText.text = "Window mode";
            }
            else if (PlayerPrefs.GetInt(LANGUAGE) == 1)
            {
                fullscreenButtonText.text = "Modo Ventana";
            }
        }
        else
        {
            if (PlayerPrefs.GetInt(LANGUAGE) == 0)
            {
                fullscreenButtonText.text = "FullScreen Mode";
            }
            else if (PlayerPrefs.GetInt(LANGUAGE) == 1)
            {
                fullscreenButtonText.text = "Pantalla Completa";
            }
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetModelDetails()
    {
        if(modelDetailsDropdown.value == 0)
        {
            PlayerPrefs.SetInt(MODEL_DETAILS, 0);
        }
        else if(modelDetailsDropdown.value == 1)
        {
            PlayerPrefs.SetInt(MODEL_DETAILS, 1);
        }
    }
}
