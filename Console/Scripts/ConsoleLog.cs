using UnityEngine;
using System.Collections.Generic;

public class ConsoleLog
{
    /// <summary>
    /// The current instance
    /// </summary>
    private static ConsoleLog instance;

    /// <summary>
    /// The current list of lines
    /// </summary>
    private List<string> lines;

    /// <summary>
    /// The maximum number of lines to store
    /// </summary>
    private int MaxLines = 512;

    /// <summary>
    /// Whether we have new lines
    /// </summary>
    private bool hasNewLines;

    /// <summary>
    /// Default constructor
    /// </summary>
    public ConsoleLog()
    {
        hasNewLines = false;
        lines = new List<string>();
    }

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
    /// Returns whether the log has new lines since the last time this was checked
    /// Will set HasNewLines to false
    /// </summary>
    public bool HasNewLines
    {
        get
        {
            bool current = hasNewLines;
            hasNewLines = false;
            return current;
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
        lines.Add(message);
        //We're exceeding the maximum number of lines we want to store
        if (lines.Count > MaxLines)
        {
            int diff = lines.Count - MaxLines;
            //Removes the oldest messages until we have the amount of lines we wanted to store left
            lines.RemoveRange(0, diff);
        }
        log = string.Join("\n", lines.ToArray());
        hasNewLines = true;
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
