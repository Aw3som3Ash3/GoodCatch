using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CreditsController : MonoBehaviour
{
    public UIDocument creditsUIDocument;
    public float scrollDuration = 10.0f; // Duration of scroll in seconds

    private ScrollView credits;
    private VisualElement fallenSouls;
    private void Start()
    {
        StartScroll();
    }
    public void StartScroll()
    {
        Debug.Log("start credit scroll");
        var root = creditsUIDocument.rootVisualElement;
        credits = root.Q<ScrollView>();

        if (credits != null)
        {
            StartCoroutine(SmoothScrollToElement(scrollDuration));
        }
    }
    private IEnumerator SmoothScrollToElement(float duration)
    {
        Vector2 startOffset = credits.scrollOffset;

        // ScrollTo helps force layout so we can get the correct final offset
        //credits.ScrollTo(target);
        //Vector2 endOffset = credits.scrollOffset;
        //credits.scrollOffset = startOffset; // Reset so we can interpolate

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            credits.verticalScroller.value = Mathf.Lerp(credits.verticalScroller.lowValue, credits.verticalScroller.highValue , t);
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }
}