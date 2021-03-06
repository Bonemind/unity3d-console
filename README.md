In-game Console
=======

Synopsis
--------

Quake-style console plugin for Unity3d.  Toggle the console by pressing tilde (~).

Forked from  [mikelovesrobots/unity3d-console] (https://github.com/mikelovesrobots/unity3d-console)

#### Includes:

* Original functionality
* [KovaaK's] (https://github.com/KovaaK) Tab completion and command history
* Scrollbar fix since it wouldn't work for me
* Autoscroll fix when a new message is logged
* Command listing using list
* Basic handling for double command registration
* Unity Debug.Log message display in console window

Screenshot
-----------
![Screenshot](https://dl.dropboxusercontent.com/s/z0gw0h267h0fzz4/Screen%20Shot%202014-01-06%20at%2011.26.19%20AM.png)

Installation
------------

1. Copy the Console directory to your Assets/Plugins folder.  (Make the plugins folder if it doesn't exist.)
2. Drag the console prefab into your scene.
3. Run your scene and press ~ to launch the console.
4. Now register some custom commands

Registering custom commands
---------------
```
using UnityEngine;
using System.Collections;

public class ConsoleCommandRouter : MonoBehaviour {
    void Start () {
        var repo = ConsoleCommandsRepository.Instance;
        repo.RegisterCommand("save", Save);
        repo.RegisterCommand("load", Load);
    }

    public void Save(params string[] args) {
        var filename = args[0];
        new LevelSaver().Save(filename);
        Debug.Log("Saved to " + filename);
    }

    public void Load(params string[] args) {
        var filename = args[0];
        new LevelLoader().Load(filename);
        Debug.Log("Loaded " + filename);
    }
}
```

The string returned from the console command will be written to the in-game 
console log.  Insert newlines in your response to have multiple lines be written
to the log.

Logging
-------

```
Debug.Log("Player died")
```

Logs to the in-game console.

Note from the author
--------------------

mikelovesrobots is the original author:

Feel free to contribute your changes back!  I love pull requests.

Cheers,

Mike

mikelovesrobots@gmail.com

@mikelovesrobots on Twitter
