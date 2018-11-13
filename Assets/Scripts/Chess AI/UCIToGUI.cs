using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Diagnostics;

public class UCIToGUI : MonoBehaviour
{

    Process process;
    StreamWriter messageStream;

    public static UCIToGUI instance;

    public string piecesPosition = "position startpos moves";
    string recievedMove;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartProcess();    
    }

    void StartProcess()
    {
        try
        {
            process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo.FileName = Application.streamingAssetsPath + "/stockfish-9-win/Windows/stockfish_9_x64.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.OutputDataReceived += new DataReceivedEventHandler(DataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(ErrorReceived);
            process.Start();
            process.BeginOutputReadLine();
            messageStream = process.StandardInput;
            messageStream.WriteLine("uci /n");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Unable to launch app: " + e.Message);
        }
    }


    void DataReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        // Handle it
        recievedMove = eventArgs.Data;
    }


    void ErrorReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        UnityEngine.Debug.LogError(eventArgs.Data);
    }

    private void OnDestroy()
    {
        if (process != null && !process.HasExited)
        {
            process.Kill();
        }
    }

    void OnApplicationQuit()
    {
        if (process != null && !process.HasExited)
        {
            process.Kill();
        }
    }

    public void SearchForMove()
    {
        messageStream.WriteLine(piecesPosition);
        messageStream.WriteLine("go movetime 6000 /n");
        StartCoroutine(CoroutineReadStockfish());
    }

    IEnumerator CoroutineReadStockfish()
    {
        yield return new WaitForSecondsRealtime(7f);
        var a = recievedMove[9].ToString();
        var b = recievedMove[10].ToString();
        var c = recievedMove[11].ToString();
        var d = recievedMove[12].ToString();
        var e = recievedMove[13].ToString();
        BoardManager.Instance.RecieveMoveFromUCI(a + b + c + d + e);
    }
}
