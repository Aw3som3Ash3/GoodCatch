using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FishingGameUI : MonoBehaviour
{
    public RectTransform fishTransform;
    public RectTransform catcherTransform;

    public bool isFishOn;

    public Slider slider;
    float successIncrement = 15f;
    float failDecrement = 25f;
    float successThreshold = 100f;
    float failThreshold = -100f;
    float successCounter = 0f;

    Action<bool> onEnd;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

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
            onEnd?.Invoke(true);
            Destroy(gameObject);

            //successCounter = 0;
            //slider.value = 0;
        }
        else if (successCounter <= failThreshold)
        {
            Debug.Log("Failed");
            onEnd?.Invoke(false);
            Destroy(gameObject);

            //successCounter = 0;
            //slider.value = 0;
        }
    }

    private bool CheckOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Rect r1 = rect1.rect;
        Rect r2 = rect2.rect;
        return r1.Overlaps(r2);
    }

    public void StartMinigame(int difficulty, Action<bool> onEnd)
    {
        this.gameObject.SetActive(true);
        this.onEnd = onEnd;
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


