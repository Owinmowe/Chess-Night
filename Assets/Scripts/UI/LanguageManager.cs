using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageManager : MonoBehaviour {

    [SerializeField] TMP_Dropdown LanguageDropdown;

    public static LanguageManager Instance { set; get; }

    public List<TextType> UITexts;

    public List<DropdownTextType> DropdownsWithText;

    const string LANGUAGE = "Language";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetLanguage();
        if(LanguageDropdown != null)
        {
            LanguageDropdown.value = PlayerPrefs.GetInt(LANGUAGE);
        }
    }

    public void ChooseLanguage()
    {
        if (LanguageDropdown != null)
        {
            if (LanguageDropdown.value == 0)
            {
                PlayerPrefs.SetInt(LANGUAGE, 0);
            }
            else if (LanguageDropdown.value == 1)
            {
                PlayerPrefs.SetInt(LANGUAGE, 1);
            }
        }
        SetLanguage();
    }

    public void SetLanguage()
    {
        foreach (TextType c in UITexts)
        {
            if (PlayerPrefs.GetInt(LANGUAGE) == 0)
            {
                c.TextMeshPro.text = c.textInEnglish;
            }
            else if (PlayerPrefs.GetInt(LANGUAGE) == 1)
            {
                c.TextMeshPro.text = c.textoEnEspañol;
            }
        }

        foreach (DropdownTextType d in DropdownsWithText)
        {
            if (PlayerPrefs.GetInt(LANGUAGE) == 0)
            {
                d.DropdownTextMeshPro.ClearOptions();
                d.DropdownTextMeshPro.AddOptions(d.textsInEnglish);
            }
            else if (PlayerPrefs.GetInt(LANGUAGE) == 1)
            {
                d.DropdownTextMeshPro.ClearOptions();
                d.DropdownTextMeshPro.AddOptions(d.textosEnEspañol);
            }
            d.DropdownTextMeshPro.RefreshShownValue();
        }
    }


    [System.Serializable]
    public class TextType
    {
        public TextMeshProUGUI TextMeshPro;
        public string textInEnglish;
        public string textoEnEspañol;
    }

    [System.Serializable]
    public class DropdownTextType
    {
        public TMP_Dropdown DropdownTextMeshPro;
        public List<string> textsInEnglish;
        public List<string> textosEnEspañol;
    }
}
