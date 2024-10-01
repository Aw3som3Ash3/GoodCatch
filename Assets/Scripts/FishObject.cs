using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishObject : MonoBehaviour
{
    FishMonster fish;
    GameObject model;
    Vector3 destination;
    bool shouldMove;
    [SerializeField]
    float moveSpeed;

    public Action ReachedDestination;
   

    //public FishObject(FishMonster fish)
    //{
    //    this.fish = fish;
    //}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    public void SetFish(FishMonster fish)
    {
        model =Instantiate(fish.Model,this.transform);
        model.transform.localPosition = Vector3.zero;
    }
    public void SetDestination(Vector3 destination)
    {
       this.destination = destination;
        shouldMove = true;
        StartCoroutine(MoveToDestination());
    }

    IEnumerator MoveToDestination()
    {
        while(shouldMove && Vector3.Distance(this.transform.position, destination) > 0.01f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination, moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        shouldMove = false;
        ReachedDestination?.Invoke();
        
    }
}
