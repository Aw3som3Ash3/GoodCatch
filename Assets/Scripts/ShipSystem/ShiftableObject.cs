using UnityEngine;

public class ShiftableObject : MonoBehaviour
{
    Vector3 offsetPosition;
    public Vector3 RelativePosition { get { return offsetPosition + this.transform.position; } }
    // Start is called before the first frame update
    void Start()
    {
        FloatingOrigin.OriginShift += ShiftPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void ShiftPosition(Vector3 position)
    {
        this.transform.position -= position;
        offsetPosition += position;
    }
}
