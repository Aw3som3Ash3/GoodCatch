using UnityEngine;

public class WaterAligner : MonoBehaviour
{
    [SerializeField]
    Transform target;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(target.position.x, this.transform.position.y, target.position.z);
    }
}
