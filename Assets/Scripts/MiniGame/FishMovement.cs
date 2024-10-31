using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float maxUp = 250f;
    public float maxDown = -250f;

    public float moveSpeed = 250f;
    public float changeFreq = .01f;

    public float targetPosition;
    public bool movingUp = true;


    // Start is called before the first frame update
    void Start()
    {
        //Randomly assign a target position on start for the fish to move
        targetPosition = Random.Range(maxDown, maxUp);
    }

    // Update is called once per frame
    void Update()
    {
        //Move the fish icon
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(transform.localPosition.x, targetPosition, transform.localPosition.z), moveSpeed * Time.deltaTime);


        //Confirm if fish reaches target
        if(Mathf.Approximately(transform.localPosition.y, targetPosition))
        {
            //Create a new target position
            targetPosition = Random.Range(maxDown, maxUp); 
        }

        //Change direction
        if (Random.value < changeFreq)
        {
            movingUp = !movingUp;
            targetPosition = movingUp ? maxUp : maxDown;
        }

    }
}
