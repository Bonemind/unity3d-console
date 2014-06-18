using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void ConsoleCommandCallback(params string[] args);

public class ConsoleCommandsRepository
{
    private static ConsoleCommandsRepository instance;
    private Dictionary<string, ConsoleCommandCallback> repository;

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
    public ConsoleCommandsRepository()
    {
        repository = new Dictionary<string, ConsoleCommandCallback>();
        this.RegisterCommand("list", ListCommands);
    }

    public void RegisterCommand(string command, ConsoleCommandCallback callback)
    {
        if (HasCommand(command))
        {
            ConsoleLog.Instance.Log(string.Format("Command already exists: {0}, new definition ignored", command));
            return;
        }
        repository[command] = new ConsoleCommandCallback(callback);
    }

    public bool HasCommand(string command)
    {
        return repository.ContainsKey(command);
    }

    public void ExecuteCommand(string command, string[] args)
    {
        if (HasCommand(command))
        {
            repository[command](args);
        }
        else
        {
            Debug.LogError("Command not found");
        }
    }

    public void ListCommands(params string[] args)
    {
        Debug.Log("Commands:");
        foreach (KeyValuePair<string, ConsoleCommandCallback> pair in repository)
        {
            Debug.Log(pair.Key);
        }
    }

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
