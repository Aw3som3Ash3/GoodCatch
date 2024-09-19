using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class ShipControls : Station
{
    [SerializeField]
    ShipSimulator ship;
    float turnValue;
    float sailValue;
    [SerializeField]
    Transform wheel;
    [SerializeField]
    float turnSpeed;

    public override void MoveInput(Vector2 input)
    {
        turnValue = input.x*turnSpeed;
        sailValue = input.y;
    }

    public override void Use()
    {
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ship.AdjustTurn(turnValue*Time.deltaTime);
        
        ship.AdjustSails(sailValue * Time.deltaTime);
        wheel.eulerAngles = new Vector3(wheel.eulerAngles.x,wheel.eulerAngles.y, -ship.turnRatio * 360);

    }
}
