using UnityEngine;

public class ShipObjectManager : MonoBehaviour
{
    [SerializeField]
    Transform childrenObject;
    private void OnTriggerEnter(Collider other)
    {
        print("object in trigger");
        other.transform.SetParent(childrenObject);
    }
    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
    }
}
