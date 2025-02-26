using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GameManager Command", menuName = "Dev console/Gamemanager", order = 1)]
public class GameManagerCommands : DevCommand
{
    public override void RunCommand(string[] args)
    {
        CommandInvoker<GameManager>()(GameManager.Instance, args);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
