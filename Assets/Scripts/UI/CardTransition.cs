using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class CardTransition : MonoBehaviour
{
    private VisualElement outline;
    private Button flipflopButton;

    void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component is missing. Ensure it is attached to the GameObject.");
            return;
        }

        var root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root Visual Element is null. Ensure the UXML file is assigned to the UIDocument component.");
            return;
        }

        outline = root.Q<VisualElement>("Outline");
        if (outline == null)
        {
            Debug.LogError("Outline element not found. Ensure the name matches the UXML.");
            return;
        }

        flipflopButton = root.Q<Button>("flipflopbutton");
        if (flipflopButton == null)
        {
            Debug.LogError("FlipFlopButton element not found. Ensure the name matches the UXML.");
            return;
        }

        // Add click event listener to the button
        flipflopButton.clicked += () =>
        {
            Debug.Log("Button clicked");
            StartCoroutine(AnimateOutline());
        };
    }

    IEnumerator AnimateOutline()
    {
        Debug.Log("Starting animation: setting width to 0");
        // Set width to 0
        outline.style.width = 0;
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Animation complete: setting width back to 490");
        // Set width back to original
        outline.style.width = 490;
    }
}