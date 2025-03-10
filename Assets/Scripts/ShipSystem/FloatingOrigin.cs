using System;
using UnityEngine;

public class FloatingOrigin : MonoBehaviour
{
    public static Action<Vector3> OriginShift;
    Vector3 offsetPosition;
    public Vector3 RelativePosition { get { return offsetPosition + this.transform.position; } }
    [SerializeField]
    float distanceToShift = 100;

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
       // OriginShift=new();
    }

    private void Awake()
    {
        offsetPosition = transform.position;
    }
    // Start is called before the first frame update 
    private void LateUpdate()
    {
        Vector3 pos = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        if (pos.magnitude >= distanceToShift)
        {
            Vector3 amountToShift = pos;
            this.transform.position = Vector3.zero;
            OriginShift(amountToShift);
        }
    }
}
