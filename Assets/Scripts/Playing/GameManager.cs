using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] CinemachineVirtualCamera whiteCamera;
    [SerializeField] CinemachineVirtualCamera blackCamera;
    [SerializeField] CinemachineVirtualCamera orthographicCamera;
    [SerializeField] float turnChangeTimer = .5f;
    [SerializeField] TextMeshProUGUI blackTurnText;
    [SerializeField] TextMeshProUGUI whiteTurnText;
    [SerializeField] TextMeshProUGUI whiteTimeNumber;
    [SerializeField] TextMeshProUGUI blackTimeNumber;
    [SerializeField] TextMeshProUGUI turnNumberText;
    [SerializeField] TextMeshProUGUI whiteCheckText;
    [SerializeField] TextMeshProUGUI blackCheckText;
    [SerializeField] TextMeshProUGUI whiteWinsText;
    [SerializeField] TextMeshProUGUI blackWinsText;
    [SerializeField] TextMeshProUGUI DrawText;
    [SerializeField] GameObject promotionScreen;
    [SerializeField] GameObject playingScreen;
    [SerializeField] GameObject endGameScreen;
    [SerializeField] GameObject boardLight;
    [SerializeField] GameObject piecesLight;

    enum CameraState { rotating, orthographic, blackSide, whiteSide }
    CameraState camState = CameraState.whiteSide;

    enum TurnState { whiteTurn, blackTurn }
    TurnState turnState = TurnState.whiteTurn;

    public int turnNumber = 1;
    int whiteMinutes = 120;
    int whiteSeconds = 0;
    int whiteMinutesExtra = 60;
    int blackMinutes = 120;
    int blackSeconds = 0;
    int blackMinutesExtra = 60;

    const string CAMERA_MODE = "Camera";

    private void Start()
    {
        SetCameraView();
        turnNumber++;
        turnNumberText.text = (turnNumber / 2).ToString();
        blackTurnText.canvasRenderer.SetAlpha(0f);
        whiteTurnText.canvasRenderer.SetAlpha(0f);
        whiteCheckText.canvasRenderer.SetAlpha(0f);
        blackCheckText.canvasRenderer.SetAlpha(0f);
        blackTimeNumber.text = blackMinutes.ToString("00") + ":" + blackSeconds.ToString("00");
        StartCoroutine(CoroutineStartingLights());
    }

    IEnumerator CoroutineStartingLights()
    {
        SoundManager.Instance.FlickerSound();
        yield return new WaitForSeconds(.5f);
        int flickerings = 3;
        while (flickerings > 0)
        {
            boardLight.SetActive(false);
            piecesLight.SetActive(false);
            yield return new WaitForSeconds(.1f);
            boardLight.SetActive(true);
            piecesLight.SetActive(true);
            yield return new WaitForSeconds(.1f);
            flickerings--;
        }
        StartCoroutine(CoroutineWhiteTimer());
    }

    private void SetCameraView()
    {
        if (PlayerPrefs.HasKey(CAMERA_MODE) == true)
        {
            if (PlayerPrefs.GetInt(CAMERA_MODE) == 3)
            {
                camState = CameraState.rotating;
                if (turnState == TurnState.whiteTurn)
                {
                    whiteCamera.enabled = true;
                    blackCamera.enabled = false;
                    orthographicCamera.enabled = false;
                }
                else if (turnState == TurnState.blackTurn)
                {
                    blackCamera.enabled = true;
                    whiteCamera.enabled = false;
                    orthographicCamera.enabled = false;
                }
            }
            else if (PlayerPrefs.GetInt(CAMERA_MODE) == 2)
            {
                camState = CameraState.orthographic;
                orthographicCamera.enabled = true;
                whiteCamera.enabled = false;
                blackCamera.enabled = false;
            }
            else if (PlayerPrefs.GetInt(CAMERA_MODE) == 0)
            {
                camState = CameraState.whiteSide;
                orthographicCamera.enabled = false;
                whiteCamera.enabled = true;
                blackCamera.enabled = false;
            }
            else if (PlayerPrefs.GetInt(CAMERA_MODE) == 1)
            {
                camState = CameraState.blackSide;
                orthographicCamera.enabled = false;
                whiteCamera.enabled = false;
                blackCamera.enabled = true;
            }
        }
        else
        {
            camState = CameraState.whiteSide;
            orthographicCamera.enabled = false;
            whiteCamera.enabled = true;
            blackCamera.enabled = false;
        }
    }

    public void Check(bool isInCheck, bool isItWhite)
    {
        if (isItWhite)
        {
            if (isInCheck)
            {
                whiteCheckText.CrossFadeAlpha(1, 1, true);
            }
            else
            {
                whiteCheckText.CrossFadeAlpha(0, 1, true);
            }
        }
        else
        {
            if (isInCheck)
            {
                blackCheckText.CrossFadeAlpha(1, 1, true);
            }
            else
            {
                blackCheckText.CrossFadeAlpha(0, 1, true);
            }
        }
    }

    public void BlackWin()
    {
        playingScreen.SetActive(false);
        endGameScreen.SetActive(true);
        blackWinsText.alpha = 255f;
        whiteWinsText.alpha = 0f;
        DrawText.alpha = 0f;
    }

    public void WhiteWin()
    {
        playingScreen.SetActive(false);
        endGameScreen.SetActive(true);
        blackWinsText.alpha = 0f;
        whiteWinsText.alpha = 255f;
        DrawText.alpha = 0f;
    }

    public void Draw()
    {
        playingScreen.SetActive(false);
        endGameScreen.SetActive(true);
        blackWinsText.alpha = 0f;
        whiteWinsText.alpha = 0f;
        DrawText.alpha = 255f;
    }

    public void ChangeTurn()
    {
        turnNumber++;
        turnNumberText.text = (turnNumber / 2).ToString();
        if (turnNumber == 80)
        {
            whiteMinutes = whiteMinutes + whiteMinutesExtra;
            blackMinutes = blackMinutes + blackMinutesExtra;
        }
        whiteTimeNumber.outlineColor = Color.black;
        blackTimeNumber.outlineColor = Color.black;
        if (camState == CameraState.rotating)
        {
            if (turnState == TurnState.whiteTurn)
            {
                whiteCamera.enabled = false;
                blackCamera.enabled = true;
                blackTurnText.CrossFadeAlpha(1, .5f, true);
                StartCoroutine(CoroutineBlackTurn());
            }
            else if (turnState == TurnState.blackTurn)
            {
                whiteCamera.enabled = true;
                blackCamera.enabled = false;
                whiteTurnText.CrossFadeAlpha(1, .5f, true);
                StartCoroutine(CoroutineWhiteTurn());
            }
        }
        else if (camState == CameraState.orthographic || camState == CameraState.whiteSide || camState == CameraState.blackSide)
        {
            if (turnState == TurnState.whiteTurn)
            {
                blackTurnText.CrossFadeAlpha(1, .5f, true);
                StartCoroutine(CoroutineBlackTurn());
            }
            else if (turnState == TurnState.blackTurn)
            {
                whiteTurnText.CrossFadeAlpha(1, .5f, true);
                StartCoroutine(CoroutineWhiteTurn());
            }
        }
    }

    IEnumerator CoroutineBlackTurn()
    {
        turnState = TurnState.blackTurn;
        StartCoroutine(CoroutineBlackTimer());
        yield return new WaitForSecondsRealtime(turnChangeTimer);
        blackTurnText.CrossFadeAlpha(0, .5f, true);
    }

    IEnumerator CoroutineBlackTimer()
    {
        whiteTimeNumber.outlineColor = new Color32(200, 0, 0, 255);
        blackTimeNumber.outlineColor = new Color32(0, 128, 0, 255);
        while (turnState == TurnState.blackTurn)
        {
            if (blackSeconds == 0)
            {
                if (blackMinutes == 0 && blackSeconds == 0)
                {
                    BoardManager.Instance.WhiteTeamWin();
                }
                blackSeconds = 59;
                blackMinutes--;
                blackTimeNumber.text = blackMinutes.ToString("00") + ":" + blackSeconds.ToString("00");
                yield return new WaitForSecondsRealtime(1f);
            }
            else if (blackSeconds != 0)
            {
                blackSeconds--;
                blackTimeNumber.text = blackMinutes.ToString("00") + ":" + blackSeconds.ToString("00");
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }

    IEnumerator CoroutineWhiteTurn()
    {
        turnState = TurnState.whiteTurn;
        StartCoroutine(CoroutineWhiteTimer());
        yield return new WaitForSecondsRealtime(turnChangeTimer);
        whiteTurnText.CrossFadeAlpha(0, .5f, true);
    }

    IEnumerator CoroutineWhiteTimer()
    {
        whiteTimeNumber.outlineColor = new Color32(0, 128, 0, 255);
        blackTimeNumber.outlineColor = new Color32(200, 0, 0, 255);
        while (turnState == TurnState.whiteTurn)
        {
            if (whiteSeconds == 0)
            {
                if (blackMinutes == 0 && blackSeconds == 0)
                {
                    BoardManager.Instance.BlackTeamWin();
                }
                whiteSeconds = 59;
                whiteMinutes--;
                whiteTimeNumber.text = whiteMinutes.ToString("00") + ":" + whiteSeconds.ToString("00");
                yield return new WaitForSecondsRealtime(1f);
            }
            else if (whiteSeconds != 0)
            {
                whiteSeconds--;
                whiteTimeNumber.text = whiteMinutes.ToString("00") + ":" + whiteSeconds.ToString("00");
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }

    public void BackToTheMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenOrClosePromotionScreen()
    {
        if (!promotionScreen.activeSelf)
        {
            promotionScreen.SetActive(true);
        }
        else
        {
            promotionScreen.SetActive(false);
        }
    }
}