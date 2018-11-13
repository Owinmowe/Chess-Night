using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayMenu : MonoBehaviour {

    [SerializeField] TextMeshProUGUI whiteControlButtonText;
    [SerializeField] TextMeshProUGUI blackControlButtonText;

    const string WHITE_CONTROL = "WhiteControl";
    const string BLACK_CONTROL = "BlackControl";
    const string LANGUAGE = "Language";
    const int HUMAN = 0;
    const int AI = 1;

    // Use this for initialization
    void OnEnable ()
    {
        StartingSettings();	
	}

    public void StartGame()
    {
        PlayerPrefs.Save();
        StartCoroutine(CoroutineWaitToPlay());
    }

    public void SetWhiteControl()
    {
        if (PlayerPrefs.GetInt(WHITE_CONTROL) == HUMAN)
        {
            PlayerPrefs.SetInt(WHITE_CONTROL, AI);
            if (PlayerPrefs.GetInt(LANGUAGE) == 0)
            {
                whiteControlButtonText.text = "White Player AI";
            }
            else if (PlayerPrefs.GetInt(LANGUAGE) == 1)
            {
                whiteControlButtonText.text = "Jugador Blanco IA";
            }
        }
        else if (PlayerPrefs.GetInt(WHITE_CONTROL) == AI)
        {
            PlayerPrefs.SetInt(WHITE_CONTROL, HUMAN);
            if (PlayerPrefs.GetInt(LANGUAGE) == 0)
            {
                whiteControlButtonText.text = "White Player Human";
            }
            else if (PlayerPrefs.GetInt(LANGUAGE) == 1)
            {
                whiteControlButtonText.text = "Jugador Blanco Humano";
            }
        }
    }

    public void SetBlackControl()
    {
        if (PlayerPrefs.GetInt(BLACK_CONTROL) == HUMAN)
        {
            PlayerPrefs.SetInt(BLACK_CONTROL, AI);
            if (PlayerPrefs.GetInt(LANGUAGE) == 0)
            {
                blackControlButtonText.text = "Black Player AI";
            }
            else if (PlayerPrefs.GetInt(LANGUAGE) == 1)
            {
                blackControlButtonText.text = "Jugador Negro IA";
            }
        }
        else if (PlayerPrefs.GetInt(BLACK_CONTROL) == AI)
        {
            PlayerPrefs.SetInt(BLACK_CONTROL, HUMAN);
            if (PlayerPrefs.GetInt(LANGUAGE) == 0)
            {
                blackControlButtonText.text = "Black Player Human";
            }
            else if (PlayerPrefs.GetInt(LANGUAGE) == 1)
            {
                blackControlButtonText.text = "Jugador Negro Humano";
            }
        }
    }

    IEnumerator CoroutineWaitToPlay()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);
        operation.allowSceneActivation = false;
        while (operation.progress >= 0.9f) // 0.9f is the set parameter to stop
        {
            Debug.Log(operation.progress);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(.5f);
        operation.allowSceneActivation = true;
    }

    void StartingSettings()
    {
        if (!PlayerPrefs.HasKey(BLACK_CONTROL))
        {
            PlayerPrefs.SetInt(BLACK_CONTROL, AI);
            PlayerPrefs.SetInt(BLACK_CONTROL, HUMAN);
        }
        if (PlayerPrefs.GetInt(BLACK_CONTROL) == HUMAN && PlayerPrefs.GetInt(LANGUAGE) == 0)
        {
            blackControlButtonText.text = "Black Player Human";
        }
        else if (PlayerPrefs.GetInt(BLACK_CONTROL) == HUMAN && PlayerPrefs.GetInt(LANGUAGE) == 1)
        {
            blackControlButtonText.text = "Jugador Negro Humano";
        }
        else if (PlayerPrefs.GetInt(BLACK_CONTROL) == AI && PlayerPrefs.GetInt(LANGUAGE) == 0)
        {
            blackControlButtonText.text = "Black Player AI";
        }
        else if (PlayerPrefs.GetInt(BLACK_CONTROL) == AI && PlayerPrefs.GetInt(LANGUAGE) == 1)
        {
            blackControlButtonText.text = "Jugador Negro IA";
        }

        if (!PlayerPrefs.HasKey(WHITE_CONTROL))
        {
            PlayerPrefs.SetInt(WHITE_CONTROL, AI);
            PlayerPrefs.SetInt(WHITE_CONTROL, HUMAN);
        }
        if (PlayerPrefs.GetInt(WHITE_CONTROL) == HUMAN && PlayerPrefs.GetInt(LANGUAGE) == 0)
        {
            whiteControlButtonText.text = "White Player Human";
        }
        else if (PlayerPrefs.GetInt(WHITE_CONTROL) == HUMAN && PlayerPrefs.GetInt(LANGUAGE) == 1)
        {
            whiteControlButtonText.text = "Jugador Blanco Humano";
        }
        else if (PlayerPrefs.GetInt(WHITE_CONTROL) == AI && PlayerPrefs.GetInt(LANGUAGE) == 0)
        {
            whiteControlButtonText.text = "White Player AI";
        }
        else if (PlayerPrefs.GetInt(WHITE_CONTROL) == AI && PlayerPrefs.GetInt(LANGUAGE) == 1)
        {
            whiteControlButtonText.text = "Jugador Blanco IA";
        }
    }
}
