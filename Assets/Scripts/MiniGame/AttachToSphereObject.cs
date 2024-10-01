using UnityEngine;
using UnityEngine.UI;
using System.Collections;  // Ensure this is included for IEnumerator

public class MouseHoverLoading3D : MonoBehaviour
{
    public Image loadingBar;  // Reference to the UI loading bar
    public Text percentageText;  // Reference to the UI percentage text
    public float loadingTime = 2.0f;  // Time it takes to fill the loading bar
    public float decreaseSpeed = 1.0f;  // Speed at which the loading bar decreases when the mouse is off
    public float moveSpeed = 5f;  // Base movement speed
    public float speedMultiplier = 2f;  // Speed multiplier when mouse is inside the circle
    public float changeDirectionInterval = 2f;  // Time interval before changing direction
    public Vector3 shrinkScale = new Vector3(0.5f, 0.5f, 0.5f);  // Scale when the sphere shrinks
    public float scaleSpeed = 1f;  // Speed at which the sphere scales
    private Vector3 originalScale;  // Original size of the sphere

    private Vector3 movementDirection;  // Current movement direction
    private float currentTime = 0.0f;  // The current progress time
    private bool isInsideSphere = false;  // Boolean to check if the mouse is over the circle
    private float timeSinceLastChange = 0f;  // Timer to track direction changes
    private Coroutine scaleCoroutine;  // Reference to the running scaling coroutine

    void Start()
    {
        // Set an initial random direction
        SetRandomDirection();
        originalScale = transform.localScale;  // Store the original scale of the sphere
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  // Create a ray from the camera to the mouse position
        RaycastHit hit;

        // Check if the ray hits the object with the SphereCollider
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == GetComponent<Collider>())
            {
                if (!isInsideSphere)
                {
                    isInsideSphere = true;
                    // Start scaling down and stop any previous scaling coroutine
                    if (scaleCoroutine != null)
                    {
                        StopCoroutine(scaleCoroutine);
                    }
                    scaleCoroutine = StartCoroutine(SmoothScale(shrinkScale)); // Start scaling down
                }
            }
            else
            {
                if (isInsideSphere)
                {
                    isInsideSphere = false;
                    // Start scaling back up and stop any previous scaling coroutine
                    if (scaleCoroutine != null)
                    {
                        StopCoroutine(scaleCoroutine);
                    }
                    scaleCoroutine = StartCoroutine(SmoothScale(originalScale)); // Start scaling back up
                }
            }
        }
        else
        {
            if (isInsideSphere)
            {
                isInsideSphere = false;
                // Start scaling back up and stop any previous scaling coroutine
                if (scaleCoroutine != null)
                {
                    StopCoroutine(scaleCoroutine);
                }
                scaleCoroutine = StartCoroutine(SmoothScale(originalScale)); // Start scaling back up
            }
        }

        // Adjust speed based on whether the mouse is inside the circle or not
        float currentMoveSpeed = isInsideSphere ? moveSpeed * speedMultiplier : moveSpeed;

        // Move the circle in the current direction
        transform.Translate(movementDirection * currentMoveSpeed * Time.deltaTime);

        // Update loading bar and percentage
        if (isInsideSphere)
        {
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp(currentTime / loadingTime, 0, 1);
            loadingBar.fillAmount = progress;
            percentageText.text = (progress * 100).ToString("F0") + "%";
        }
        else
        {
            // Decrease the progress when the mouse is not inside the circle
            currentTime -= Time.deltaTime * decreaseSpeed;
            currentTime = Mathf.Clamp(currentTime, 0, loadingTime);  // Clamp the value so it doesn't go below 0
            float progress = Mathf.Clamp(currentTime / loadingTime, 0, 1);
            loadingBar.fillAmount = progress;
            percentageText.text = (progress * 100).ToString("F0") + "%";
        }

        // Change direction at intervals
        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= changeDirectionInterval)
        {
            SetRandomDirection();
            timeSinceLastChange = 0f;
        }

        // (Optional) Check bounds to keep the circle within certain limits
        CheckBounds();
    }

    private IEnumerator SmoothScale(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < scaleSpeed)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / scaleSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure the final scale is set
    }

    // Generate a random direction
    void SetRandomDirection()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);

        movementDirection = new Vector3(randomX, randomY, randomZ).normalized;
    }

    // Optional: Keep the circle within bounds
    void CheckBounds()
    {
        float xLimit = 10f;
        float yLimit = 10f;
        float zLimit = 10f;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -xLimit, xLimit);
        pos.y = Mathf.Clamp(pos.y, -yLimit, yLimit);
        pos.z = Mathf.Clamp(pos.z, -zLimit, zLimit);

        transform.position = pos;
    }
}
