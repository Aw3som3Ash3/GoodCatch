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

    [Header("Line Distance Slider Parameters")]
    public Slider lineDistance;
    float successIncrement = 35f;
    float failDecrement = 30f;
    float successThreshold = -100f;
    float failThreshold = 100f;
    float successCounter = 0f;

    [Header("Good Catch Slider Parameters")]
    public Slider goodCatchSlider;
    float successIncrement2 = 25f;
    float failDecrement2 = 20f;
    float successThreshold2 = 100;
    float failThreshold2 = -100f;
    float successCounter2 = 0f;

    Action<bool> onEnd;

    private void Awake()
    {
        //this.gameObject.SetActive(false);
    }

    private void Start()
    {
        successCounter = 70f;
        lineDistance.value = 70;
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
            successCounter -= successIncrement * Time.deltaTime;

            successCounter2 += successIncrement2 * Time.deltaTime;
        }
        else
        {
            successCounter += failDecrement * Time.deltaTime;

            successCounter2 -= failDecrement2 * Time.deltaTime;
        }

        // Clamp counter
        successCounter = Mathf.Clamp(successCounter, successThreshold, failThreshold);
        successCounter2 = Mathf.Clamp(successCounter2, failThreshold2, successCounter2);

        // Update slider value
        lineDistance.value = successCounter;
        goodCatchSlider.value = successCounter2;

        // Checker
        if (successCounter <= successThreshold)
        {
            Debug.Log("Success");
            onEnd?.Invoke(true);
            Destroy(gameObject);

            //successCounter = 0;
            //slider.value = 0;
        }
        else if (successCounter >= failThreshold)
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
        Rect r1 = new Rect(rect1.position.x, rect1.position.y, rect1.rect.width, rect1.rect.height);
        Rect r2 = new Rect(rect2.position.x, rect2.position.y, rect2.rect.width, rect2.rect.height);
        return r1.Overlaps(r2);
    }

    private bool CheckOverlapping2(RectTransform rect3, RectTransform rect4)
    {
        Rect r3 = new Rect(rect3.position.x, rect3.position.y, rect3.rect.width, rect3.rect.height);
        Rect r4 = new Rect(rect4.position.x, rect4.position.y, rect4.rect.width, rect4.rect.height);
        return r3.Overlaps(r4);
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


