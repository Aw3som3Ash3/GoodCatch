using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CreditsController : MonoBehaviour
{
    public UIDocument creditsUIDocument;
    public float scrollDuration = 10.0f; // Duration of scroll in seconds

    private ScrollView credits;
    private VisualElement fallenSouls;

    public void StartScroll()
    {
        var root = creditsUIDocument.rootVisualElement;
        credits = root.Q<ScrollView>("Credits");
        fallenSouls = root.Q<VisualElement>("GameTitle");

        if (credits != null && fallenSouls != null)
        {
            StartCoroutine(SmoothScrollToElement(fallenSouls, scrollDuration));
        }
    }

    private IEnumerator SmoothScrollToElement(VisualElement target, float duration)
    {
        Vector2 startOffset = credits.scrollOffset;

        // ScrollTo helps force layout so we can get the correct final offset
        credits.ScrollTo(target);
        Vector2 endOffset = credits.scrollOffset;
        credits.scrollOffset = startOffset; // Reset so we can interpolate

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            credits.scrollOffset = Vector2.Lerp(startOffset, endOffset, t);
            yield return null;
        }

        credits.scrollOffset = endOffset; // Ensure final position is exact
    }
}