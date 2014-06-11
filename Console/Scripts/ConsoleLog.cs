using UnityEngine;
using System.Collections;

public class ConsoleLog
{
    /// <summary>
    /// The current instance
    /// </summary>
    private static ConsoleLog instance;

    /// <summary>
    /// Getter/setter to make sure we always have only one instance
    /// Automaticly creates new instance if one doesn't exist
    /// </summary>
    public static ConsoleLog Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ConsoleLog();
                Application.RegisterLogCallback(Instance.HandleLog);
            }
            return instance;
        }
    }

    /// <summary>
    /// The current log
    /// </summary>
    public string log = "";

    /// <summary>
    /// Log a new message
    /// </summary>
    /// <param name="message">The message to log</param>
    public void Log(string message)
    {
        log += message + "\n";
    }

    /// <summary>
    /// Handles the logging of unity debug messages
    /// </summary>
    /// <param name="logString">The logged string</param>
    /// <param name="stackTrace">The stacktrace</param>
    /// <param name="type">The logtype</param>
    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        Instance.Log(string.Format("[{0}] {1}", type.ToString(), logString));
    }
}
