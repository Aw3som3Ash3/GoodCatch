using System;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField]
    LayerMask layerMask;
    float depth;

    public Action HitWater;


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
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WaterSimulator>() != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, Vector3.down, out hit, layerMask))
            {
                depth = hit.distance;
                print(depth);
                if (depth > 1)
                {
                    InitiateFishingMiniGame();
                }

            }

        }
    }
}
