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
    float successIncrement = 15f;
    float failDecrement = 30f;
    float successThreshold = -100f;
    float failThreshold = 100f;
    float successCounter = 0f;

    [Header("Good Catch Slider Parameters")]
    public Slider goodCatchSlider;
    float goodCatchIncrement = 25f;
    float badCatchDecrement = 20f;
    float successThreshold2 = 100;
    float badCatchThreshold = -100f;
    float goodCatchCounter = 0f;
    [HideInInspector] public int advantageOdds; 

    Action<bool> onEnd;

    private void Awake()
    {
        //this.gameObject.SetActive(false);
    }

    private void Start()
    {
        successCounter = 100f;
        lineDistance.value = 100;
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

            goodCatchCounter += goodCatchIncrement * Time.deltaTime;
        }
        else
        {
            goodCatchCounter -= badCatchDecrement * Time.deltaTime;
        }

        // Clamp counter
        successCounter = Mathf.Clamp(successCounter, successThreshold, failThreshold);
        goodCatchCounter = Mathf.Clamp(goodCatchCounter, badCatchThreshold, goodCatchCounter);

        // Update slider value
        lineDistance.value = successCounter;
        goodCatchSlider.value = goodCatchCounter;

        // Checker
        if (successCounter <= successThreshold)
        {
            Debug.Log("Success");
            onEnd?.Invoke(true);
            Destroy(gameObject);
        }
        else if (goodCatchCounter <= badCatchThreshold)
        {
            Debug.Log("Failed");
            onEnd?.Invoke(false);
            Destroy(gameObject);
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

    // Place these code bits when ready to fully implement to the current minigame system
    /*
     * public GameObject minigame
     * 
     * 
     * private void StartMinigame()
     * {
     *      minigame.SetActive(true);
     * }
     * 
     * //successCounter += failDecrement * Time.deltaTime;
     * 
     * 
     *      //successCounter = 0;
     *      //slider.value = 0;
     */
}


