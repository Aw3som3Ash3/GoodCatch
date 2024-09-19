using System.Collections;
using System.Collections.Generic;
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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<ShipSimulator>())
        {
            parent = collision.transform.GetComponentInParent<ShipSimulator>().transform;
            collision.transform.GetComponentInParent<ShipSimulator>()?.AddObject(this.transform);
            this.transform.localEulerAngles= new Vector3(0, 0, 0);
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
        this.transform.parent=null;
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
        this.transform.localScale = Vector3.one;
    }
}
