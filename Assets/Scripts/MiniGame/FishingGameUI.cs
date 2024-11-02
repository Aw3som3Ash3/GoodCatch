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

    public Slider lineDistance;
    float successIncrement = 35f;
    float failDecrement = 30f;
    float successThreshold = -100f;
    float failThreshold = 100f;
    float successCounter = 0f;

    Action<bool> onEnd;

    private void Awake()
    {
        this.gameObject.SetActive(false);
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
        }
        else
        {
            successCounter += failDecrement * Time.deltaTime;
        }

        // Clamp counter
        successCounter = Mathf.Clamp(successCounter, successThreshold, failThreshold);

        // Update slider value
        lineDistance.value = successCounter;

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


