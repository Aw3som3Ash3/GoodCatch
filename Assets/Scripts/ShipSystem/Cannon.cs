using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Station
{
    float xInput,xRot;
    float yInput,yRot;
    [SerializeField]
    
    float maxPitch, minPitch;
    [SerializeField]
    float maxYaw, minYaw;

    [SerializeField]
    GameObject cannonBallPrefab;
    
    [SerializeField]
    Transform barrel,barrelExit,cannonBase;
    [SerializeField]
    float launchVelocity;
    ShipSimulator shipSimulator;
    public override void MoveInput(Vector2 input)
    {
        xInput=input.x;
        yInput = input.y;
    }

    public override void Use()
    {
        GameObject cannonBall=Instantiate(cannonBallPrefab,barrelExit.position, cannonBallPrefab.transform.rotation);
        cannonBall.GetComponent<Rigidbody>()?.AddForce(barrelExit.forward * launchVelocity+ shipSimulator.Velocity, ForceMode.VelocityChange);

    }

    // Start is called before the first frame update
    void Start()
    {
        shipSimulator = GetComponentInParent<ShipSimulator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.Rotate(0, xInput*Time.deltaTime*10, 0);
        //barrel.transform.Rotate(-yInput * Time.deltaTime * 10, 0, 0);
        cannonBase.transform.localEulerAngles = new Vector3(0, ClampRotation(cannonBase.transform.localEulerAngles.y + xInput * Time.deltaTime*10,minYaw,maxYaw), 0);
        barrel.transform.localEulerAngles = new Vector3(ClampRotation(barrel.transform.localEulerAngles.x - yInput * Time.deltaTime * 10,maxPitch,minPitch), 0, 0);
       
    }

    float ClampRotation(float angle,float minAngle,float maxAngle)
    {
        print(angle);
        angle %= 360;
        print(angle);
        //angle=angle - 180f * Mathf.Floor((angle + 180f) / 180f);
        if (angle > 180 && angle < minAngle%360)
        {
            angle = minAngle % 360;
        }
        else if (angle <180 &&angle > maxAngle)
        {
            angle= maxAngle;
        }
        return angle;
    }
}
