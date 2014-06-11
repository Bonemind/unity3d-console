using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsoleGUI : MonoBehaviour
{
    public ConsoleAction escapeAction;
    public ConsoleAction submitAction;
    [HideInInspector]
    public string input = "";
    /// <summary>
    /// The consolelog instance, stored for convenience
    /// </summary>
    private ConsoleLog consoleLog;

    /// <summary>
    /// The console rectangle area
    /// </summary>
    private Rect consoleRect;

    /// <summary>
    /// Whether the console is focused
    /// </summary>
    private bool focus = false;

    /// <summary>
    /// The window-id for the console
    /// </summary>
    private const int WINDOW_ID = 50;

    /// <summary>
    /// The current scrollposition
    /// </summary>
    private int scrollPosition;

    /// <summary>
    /// The current linecount
    /// Used to determine if a line was added so we know when to scroll to the bottom
    /// </summary>
    private int lineCount = 0;

    /// <summary>
    /// An arbitrary large number used to scroll to the bottom of the console window
    /// </summary>
    public const int SCROLLDOWN_LARGE_VALUE = 100000;

    /// <summary>
    /// The consolecommandsrepo, stored for convenience
    /// </summary>
    private ConsoleCommandsRepository consoleCommandsRepository;

    /// <summary>
    /// The maximum number of commands to store
    /// </summary>
    public int maxConsoleHistorySize = 100;

    /// <summary>
    /// The current position in the console history
    /// Used for up and down scrolling in the history
    /// </summary>
    private int consoleHistoryPosition = 0;

    /// <summary>
    /// List containing the console commands issued thusfar
    /// </summary>
    private List<string> consoleHistoryCommands = new List<string>();

    /// <summary>
    /// Hack to work around up arrow behaviour (moving cursor to start of line)
    /// </summary>
    private bool fixPositionNextFrame = false;

    /// <summary>
    /// Setup this instance
    /// </summary>
    private void Awake()
    {
        consoleRect = new Rect(0, 0, Screen.width, Mathf.Min(300, Screen.height));
        consoleLog = ConsoleLog.Instance;
        consoleCommandsRepository = ConsoleCommandsRepository.Instance;
    }

    /// <summary>
    /// Handles enabling of the console
    /// </summary>
    private void OnEnable()
    {
        focus = true;
    }

    /// <summary>
    /// Handles disabling of the console
    /// </summary>
    private void OnDisable()
    {
        focus = true;
    }

    /// <summary>
    /// Draw the console window
    /// Only drawn when this gameObject is active
    /// </summary>
    public void OnGUI()
    {
        GUILayout.Window(WINDOW_ID, consoleRect, RenderWindow, "Console");
    }

    /// <summary>
    /// Renders the actual window
    /// </summary>
    /// <param name="id">The windowID</param>
    private void RenderWindow(int id)
    {
        //Implements the hack to work around moving the cursor to the start of a line when pressing up
        if (fixPositionNextFrame)
        {
            MoveCursorToPos(input.Length);
            fixPositionNextFrame = false;
        }

        //This should never be the case but sometimes is
        if (consoleLog == null)
        {
            return;
        }
        //Handle input
        HandleSubmit();
        HandleEscape();
        HandleTab();
        HandleUp();
        HandleDown();

        //Draw window, handle scrollbar position changes
        Vector2 scrollPositionVector = GUILayout.BeginScrollView(new Vector2(0, scrollPosition), false, true);
        scrollPosition = (int)scrollPositionVector.y;
        GUILayout.Label(consoleLog.log);
        GUILayout.EndScrollView();
        GUI.SetNextControlName("input");
        input = GUILayout.TextField(input);
        if (focus)
        {
            GUI.FocusControl("input");
            focus = false;
        }

    }

    /// <summary>
    /// Issued every frame, removes backtic that sometimes spawnes when starting the console
    /// Handles scrolling down when new text is added
    /// </summary>
    public void Update()
    {
        if (input == "`")
        {
            input = "";
        }
        int numLines = consoleLog.log.Split('\n').Length;
        if (numLines != lineCount)
        {
            scrollPosition = SCROLLDOWN_LARGE_VALUE;
        }
        lineCount = numLines;
    }

    /// <summary>
    /// Handles a command submit
    /// </summary>
    private void HandleSubmit()
    {
        if (KeyDown("[enter]") || KeyDown("return"))
        {
            consoleHistoryPosition = -1;
            if (submitAction != null)
            {
                submitAction.Activate();
                consoleHistoryCommands.Insert(0, input);
                if (consoleHistoryCommands.Count > maxConsoleHistorySize)
                {
                    consoleHistoryCommands.RemoveAt(consoleHistoryCommands.Count - 1);
                }
            }
            input = "";
        }
    }

    /// <summary>
    /// Handles the closing of the console
    /// </summary>
    private void HandleEscape()
    {
        if (KeyDown("escape") || KeyDown("`"))
        {
            escapeAction.Activate();
            input = "";
        }
    }

    /// <summary>
    /// Wraps a keyboard event
    /// </summary>
    /// <param name="key">The key to check for</param>
    /// <returns>True if the key is pressed, false otherwise</returns>
    private bool KeyDown(string key)
    {
        return Event.current.Equals(Event.KeyboardEvent(key));
    }

    /// <summary>
    /// Takes two strings and returns the largest matching substring
    /// </summary>
    /// <param name="in1">First input string</param>
    /// <param name="in2">Second input string</param>
    /// <returns>"" if no match was found, the largest substring otherwise</returns>
    private string LargestSubString(string in1, string in2)
    {
        string output = "";
        int smallestLen = Mathf.Min(in1.Length, in2.Length);
        for (int i = 0; i < smallestLen; i++)
        {
            if (in1[i] == in2[i]) output += in1[i];
            else return output;
        }
        return output;
    }

    /// <summary>
    /// Moves the text cursor to a certain position
    /// </summary>
    /// <param name="position">The position to move the cursor to</param>
    private void MoveCursorToPos(int position)
    {
        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
        editor.selectPos = position;
        editor.pos = position;
        return;
    }

    /// <summary>
    /// Handles pressing of the tab key (autocompletion)
    /// </summary>
    private void HandleTab()
    {
        //Tab is not pressed, we have no business being here
        if (!KeyDown("tab"))
        {
            return;
        }

        //Nothing was typed yet, assume the user wanted to list all commands and then return
        if (input == "")
        {
            consoleCommandsRepository.ListCommands();
            return;
        }

        //Find commands that match the current input
        List<string> search = consoleCommandsRepository.SearchCommands(input);
        if (search.Count == 0)
        {
            consoleLog.Log(string.Format("No command start with \"{0}\".", input));
            input = ""; // clear input
            return;
        }
        else if (search.Count == 1)
        {
            input = search[0] + " "; // only found one command - type it in for the guy
            MoveCursorToPos(input.Length);
        }
        else
        {
            consoleLog.Log("Commands starting with \"" + input + "\":");
            string largestMatch = search[0]; // keep track of the largest substring that matches all searches
            foreach (string command in search)
            {
                consoleLog.Log(command);
                largestMatch = LargestSubString(largestMatch, command);
            }
            input = largestMatch;
            MoveCursorToPos(input.Length);
        }
    }

    /// <summary>
    /// Scroll back up the command history
    /// </summary>
    private void HandleUp()
    {
        if (KeyDown("up"))
        {
            consoleHistoryPosition += 1;
            if (consoleHistoryPosition > consoleHistoryCommands.Count - 1) consoleHistoryPosition = consoleHistoryCommands.Count - 1;
            input = consoleHistoryCommands[consoleHistoryPosition];
            fixPositionNextFrame = true;
        }
    }

    /// <summary>
    /// Scroll down the command history
    /// </summary>
    private void HandleDown()
    {
        if (KeyDown("down"))
        {
            consoleHistoryPosition -= 1;
            if (consoleHistoryPosition < 0)
            {
                consoleHistoryPosition = -1;
                input = "";
            }
            else
                input = consoleHistoryCommands[consoleHistoryPosition];
            MoveCursorToPos(input.Length);
        }
    }
}
