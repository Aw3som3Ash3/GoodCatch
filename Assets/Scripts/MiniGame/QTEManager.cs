using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class QTEManager : MonoBehaviour
{
    public GameObject displayBox;
    public GameObject passBox;
    public int qteGen;
    public int waitForKey;
    public int correctKey;
    public int countDown;

    InputAction input;

    private void Start()
    {
        input = InputManager.Input.Fishing.MiniGame;
    }

    private void Update()
    {
        if (waitForKey == 0)
        {
            qteGen = Random.Range(0, 5);
            countDown = 1;

            if (qteGen == 1)
            {
                waitForKey = 1;

                displayBox.GetComponent<Text>().text = "[Up]";
            }
            if (qteGen == 2)
            {
                waitForKey = 1;

                displayBox.GetComponent<Text>().text = "[Down]";
            }
            if (qteGen == 3)
            {
                waitForKey = 1;

                displayBox.GetComponent<Text>().text = "[Left]";
            }
            if (qteGen == 4)
            {
                waitForKey = 1;

                displayBox.GetComponent<Text>().text = "[Right]";
            }
        }

        if (qteGen == 1)
        {
            if (input.ReadValue<float>() == 1)
            {
                correctKey = 1;
            }
            else
            {
                correctKey = 2;
            }

            StartCoroutine(KeyPressing());
        }
        if (qteGen == 2)
        {
            if (input.ReadValue<float>() == 2)
            {
                correctKey = 1;
            }
            else
            {
                correctKey = 2;
            }

            StartCoroutine(KeyPressing());
        }
        if (qteGen == 3)
        {
            if (input.ReadValue<float>() == 3)
            {
                correctKey = 1;
            }
            else
            {
                correctKey = 2;
            }

            StartCoroutine(KeyPressing());
        }
        if (qteGen == 4)
        {
            if (input.ReadValue<float>() == 4)
            {
                correctKey = 1;
            }
            else
            {
                correctKey = 2;
            }

            StartCoroutine(KeyPressing());
        }
    }

    IEnumerator KeyPressing()
    {
        qteGen = 5;
    }
}
