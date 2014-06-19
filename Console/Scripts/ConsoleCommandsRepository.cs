using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void ConsoleCommandCallback(params string[] args);

public class ConsoleCommandsRepository
{ 
    /// <summary>
    /// The ConsoleCommandsRespository instance
    /// </summary>
    private static ConsoleCommandsRepository instance;

    /// <summary>
    /// The dictionary of commands and their callbacks
    /// </summary>
    private Dictionary<string, ConsoleCommandCallback> repository;

    /// <summary>
    /// The ConsoleCommandRepo instance
    /// Getter/setter to make sure there is exactly one instance
    /// </summary>
    public static ConsoleCommandsRepository Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ConsoleCommandsRepository();
            }
            return instance;
        }
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ConsoleCommandsRepository()
    {
        repository = new Dictionary<string, ConsoleCommandCallback>();
        this.RegisterCommand("list", ListCommands);
    }

    /// <summary>
    /// Adds a new command to the commands list
    /// </summary>
    /// <param name="command">The command name to add</param>
    /// <param name="callback">The callback to call when this command is added</param>
    public void RegisterCommand(string command, ConsoleCommandCallback callback)
    {
        if (HasCommand(command))
        {
            ConsoleLog.Instance.Log(string.Format("Command already exists: {0}, new definition ignored", command));
            return;
        }
        repository[command] = new ConsoleCommandCallback(callback);
    }

    /// <summary>
    /// Checks whether a command already exists
    /// </summary>
    /// <param name="command">The command to check</param>
    /// <returns>True if it exists, false otherwise</returns>
    public bool HasCommand(string command)
    {
        return repository.ContainsKey(command);
    }

    /// <summary>
    /// Handles the excution of a command
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="args">The args to pass to the command</param>
    public void ExecuteCommand(string command, string[] args)
    {
        if (HasCommand(command))
        {
            repository[command](args);
        }
        else
        {
            ConsoleLog.Instance.Log("Command not found");
        }
    }

    /// <summary>
    /// Lists all command registered
    /// </summary>
    /// <param name="args">Unused, but defined to match the delegate</param>
    public void ListCommands(params string[] args)
    {
        ConsoleLog.Instance.Log("Commands:");
        foreach (KeyValuePair<string, ConsoleCommandCallback> pair in repository)
        {
            ConsoleLog.Instance.Log(pair.Key);
        }
    }

    /// <summary>
    /// Returns a list of commands that math the given string
    /// </summary>
    /// <param name="str">The string to match</param>
    /// <returns>The list of matching commands, or an empty list if none match</returns>
    public List<string> SearchCommands(string str)
    {
        string[] keys = new string[repository.Count];
        repository.Keys.CopyTo(keys, 0);
        List<string> output = new List<string>();
        foreach (string key in keys)
        {
            if (key.StartsWith(str))
                output.Add(key);
        }
        return output;
    }
}
