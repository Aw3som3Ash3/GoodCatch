using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed at which the circle moves
    public float changeDirectionInterval = 2f;  // Time interval before changing direction
    private Vector3 movementDirection;
    private float timeSinceLastChange = 0f;  // Track time since the last direction change

    public float xLimit = 10f;
    public float zLimit = 10f;

    void Start()
    {
        SetRandomDirection();
    }

    void Update()
    {
        transform.Translate(movementDirection * moveSpeed * Time.deltaTime);

        // Check if it's time to change direction
        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= changeDirectionInterval)
        {
            SetRandomDirection();  // Change to a new random direction
            timeSinceLastChange = 0f;  // Reset timer
        }

        CheckBounds();
    }

    void SetRandomDirection()
    {
        float xOffset = (transform.localPosition.x / xLimit);
        float zOffset = (transform.localPosition.z / zLimit);


        // Generate a random direction in 3D space
        float randomX = Random.Range(-1f + xOffset, 1f - xOffset);
        float randomZ = Random.Range(-1f + zOffset, 1f - zOffset);

        // Create a normalized vector for direction
        movementDirection = new Vector3(randomX, 0, randomZ).normalized;
    }

    void CheckBounds()
    {
        Vector3 pos = transform.localPosition;

        pos.x = Mathf.Clamp(pos.x,-xLimit, xLimit);
        pos.z = Mathf.Clamp(pos.z, -zLimit, zLimit);
        
        transform.localPosition = pos;  // Ensure the circle stays within the bounds
    }
}
