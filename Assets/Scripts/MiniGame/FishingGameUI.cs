using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingGameUI : MonoBehaviour
{
    public RectTransform fishTransform;
    public RectTransform catcherTransform;

    public bool isFishOn;

    public Slider slider;
    float successIncrement = 15;
    float failDecrement = 12;
    float successThreshold = 100;
    float failThreshold = -100;
    float successCounter = 0;

    private void Update()
    {
        if (CheckOverlapping(fishTransform, catcherTransform))
        {
            isFishOn = true;
        }
        else
        {
            isFishOn = false;
        }

        OverlappingCalculation();
    }

    private void OverlappingCalculation()
    {
        if (isFishOn)
        {
            successCounter += successIncrement * Time.deltaTime;
        }
        else
        {
            successCounter -= failDecrement * Time.deltaTime;
        }

        // Clamp counter
        successCounter = Mathf.Clamp(successCounter, failThreshold, successThreshold);

        // Update slider value
        slider.value = successCounter;

        // Checker
        if (successCounter >= successThreshold)
        {
            Debug.Log("Success");

            successCounter = 0;
            slider.value = 0;
        }
        else if (successCounter <= failThreshold)
        {
            Debug.Log("Failed");

            successCounter = 0;
            slider.value = 0;
        }
    }

    private bool CheckOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Rect r1 = new Rect(rect1.position.x, rect1.position.y, rect1.rect.width, rect1.rect.height);
        Rect r2 = new Rect(rect2.position.x, rect2.position.y, rect2.rect.width, rect2.rect.height);
        return r1.Overlaps(r2);
    }

    // Place these code bits when ready to fully implement to the current (10/27) minigame system
    /*
     * public GameObject minigame
     * 
     * 
     * private void StartMinigame()
     * {
     *      minigame.SetActive(true);
     * }
     */
}


