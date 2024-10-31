using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatcherMovement : MonoBehaviour
{
    public float maxUp = 250f;
    public float maxDown = -250f;
    public float moveSpeed = 250f;
    InputAction fishMover;
    private void Start()
    {
        fishMover = InputManager.Input.Fishing.FishMover;
        fishMover.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = fishMover.ReadValue<float>();
        //If input is received
        if (moveInput != 0)
        {
            MoveCatcher(moveInput);
        }
    }

    private void MoveCatcher(float moveInput)
    {
        Vector3 movement = Vector3.up * moveInput * moveSpeed * Time.deltaTime;

        Vector3 newPosition = transform.localPosition + movement;
        newPosition.y = Mathf.Clamp(newPosition.y, maxDown,maxUp );

        transform.localPosition = newPosition;
    }
}
