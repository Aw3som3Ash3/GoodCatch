using System.Collections;
using UnityEngine;

public class ShipObject : MonoBehaviour
{
    [SerializeField]
    float exitTime;

    Transform parent;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<ShipSimulator>())
        {
            parent = other.transform.GetComponentInParent<ShipSimulator>().transform;
            other.transform.GetComponentInParent<ShipSimulator>()?.AddObject(this.transform);
            //this.transform.localEulerAngles= new Vector3(0, 0, 0);
            StopAllCoroutines();

        }



    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ShipArea"))
        {
            StartCoroutine(ExitTimer());
        }
    }
    //private void OnCollisionExit(Collision collision)
    //{
    //    StopCoroutine(ExitTimer());
    //    StartCoroutine(ExitTimer());
    //}


    IEnumerator ExitTimer()
    {

        yield return new WaitForSeconds(exitTime);
        this.transform.parent = null;
        this.transform.localEulerAngles = new Vector3(0, this.transform.localEulerAngles.y, 0);
        this.transform.localScale = Vector3.one;
    }
}
