using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatVisualizer : MonoBehaviour
{
    // Dictionary<FishMonster, Vector3> fishToDestination;
    public Dictionary<FishMonster, FishObject> fishToObject { get; private set; } = new Dictionary<FishMonster, FishObject>();
    List<FishObject> fishObjects = new List<FishObject>();
    [SerializeField]
    GameObject fishObjectPrefab;
    //public Action CompletedMove;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddFish(FishMonster fish,Vector3 startingLocation)
    {
        FishObject fishObject = Instantiate(fishObjectPrefab, this.transform).GetComponent<FishObject>();
        fishObjects.Add(fishObject);
        fishToObject[fish] = fishObject;
        fishObject.transform.position = startingLocation;

    }

    public void MoveFish(FishMonster fish, Vector3 destination,Action CompletedMove=null)
    {
        fishToObject[fish].SetDestination(destination);
        fishToObject[fish].ReachedDestination += CompletedMove;
        CompletedMove += ()=> fishToObject[fish].ReachedDestination-=CompletedMove;
    }

    public void AnimateAttack(Vector3 start ,Vector3 target, Action CompletedMove = null)
    {
        throw new NotImplementedException();
    }


}
