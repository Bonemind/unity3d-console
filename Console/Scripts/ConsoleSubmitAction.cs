using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class ConsoleSubmitAction : ConsoleAction
{
    public ConsoleGUI consoleGUI;
    private ConsoleCommandsRepository consoleCommandsRepository;
    private ConsoleLog consoleLog;

    private void Start()
    {
        consoleCommandsRepository = ConsoleCommandsRepository.Instance;
        consoleLog = ConsoleLog.Instance;
    }

    /// <summary>
    /// Actually submits a command
    /// Splits the arguments, checks if the command exists, calls the callback if it does and prints an error otherwise
    /// </summary>
    public override void Activate()
    {
        string[] parts = consoleGUI.input.Split(' ');
        string command = parts[0];
        string[] args = parts.Skip(1).ToArray();

        consoleLog.Log("> " + consoleGUI.input);
        if (consoleCommandsRepository.HasCommand(command))
        {
            consoleLog.Log(consoleCommandsRepository.ExecuteCommand(command, args));
        }
        else
        {
            consoleLog.Log("Command " + command + " not found");
        }
    }
}
