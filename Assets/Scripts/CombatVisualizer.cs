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
    [SerializeField]
    GameObject projectilPrefab;
    //public Action CompletedMove;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RemoveFish(FishMonster fish)
    {
        Destroy(fishToObject[fish].gameObject);
    }
    public void AddFish(FishMonster fish,Vector3 startingLocation)
    {
        FishObject fishObject = Instantiate(fishObjectPrefab, this.transform).GetComponent<FishObject>();
        fishObject.SetFish(fish);
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

    public void AnimateAttack(FishMonster fish, FishMonster target, Action CompletedMove = null)
    {
        StartCoroutine(TempAttackAnim(fishToObject[fish].transform.position, fishToObject[target].transform.position,CompletedMove));
        //throw new NotImplementedException();
    }

    IEnumerator TempAttackAnim(Vector3 start, Vector3 destination, Action CompletedMove)
    {

        GameObject projectile= Instantiate(projectilPrefab, start, projectilPrefab.transform.rotation);
        while(Vector3.Distance(projectile.transform.position,destination)>0.01f)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position,destination,Time.deltaTime*5);
            yield return new WaitForEndOfFrame();
        }
        CompletedMove?.Invoke();
    }


}
