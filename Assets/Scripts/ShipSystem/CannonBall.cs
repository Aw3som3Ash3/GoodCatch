using UnityEngine;

public class CannonBall : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
