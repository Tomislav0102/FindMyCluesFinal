using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleInUI : MonoBehaviour
{
    UImanager _uiman;

    private void Awake()
    {
        _uiman = GameManager.Instance.uImanager;
    }


    void OnEnable()
    {
        Application.logMessageReceived += LogCallback;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogCallback;
    }

    void LogCallback(string logString, string stackTrace, LogType type)
    {
        _uiman.NewLineInConsole(logString);
    }
}
