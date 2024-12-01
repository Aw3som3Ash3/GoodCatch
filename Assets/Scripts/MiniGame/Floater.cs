using System;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField]
    LayerMask layerMask;
    float depth;

    public event Action HitWater;
    public event Action completed;
    [SerializeField]
    Transform lineEnd;

    public Vector3 LineEndPos { get { return lineEnd.position; } }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitiateFishingMiniGame()
    {
        HitWater?.Invoke();

    }
    void FailedCast()
    {
        completed?.Invoke();
        Destroy(gameObject);
        InputManager.EnablePlayer();
    }
   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            FailedCast();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WaterSimulator>() != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, Vector3.down, out hit, layerMask))
            {
                this.GetComponentInChildren<Collider>().enabled = false;
                depth = hit.distance;
                print(depth);
                if (depth > 1)
                {
                    InitiateFishingMiniGame();
                }
                else
                {
                    FailedCast();
                }

            }

        }
    }
}
