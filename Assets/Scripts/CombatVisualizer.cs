using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatVisualizer : MonoBehaviour
{
    // Dictionary<FishMonster, Vector3> fishToDestination;
    public Dictionary<CombatManager.Turn, FishObject> turnToObject { get; private set; } = new Dictionary<CombatManager.Turn, FishObject>();
    Dictionary<FishObject, FishUI> FishUI = new Dictionary<FishObject, FishUI>();
    List<FishObject> fishObjects = new List<FishObject>();
    [SerializeField]
    GameObject fishObjectPrefab;
    [SerializeField]
    GameObject projectilPrefab;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    GameObject fishUIPrefab;
    //public Action CompletedMove;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void RemoveFish(CombatManager.Turn turn)
    {
        Destroy(turnToObject[turn].gameObject);
    }
    public void AddFish(CombatManager.Turn turn, Vector3 startingLocation, CombatManager.Team team)
    {
        FishObject fishObject = Instantiate(fishObjectPrefab, this.transform).GetComponent<FishObject>();
        fishObject.transform.localEulerAngles = new Vector3(0, team == CombatManager.Team.player ? 90 : -90, 0);
        fishObject.SetFish(turn);
        GameObject uiObj= Instantiate(fishUIPrefab, canvas.transform);
        FishUI[fishObject] = uiObj.GetComponent<FishUI>();
        FishUI[fishObject].SetFish(turn, fishObject.transform);
        fishObject.ObjectDestroyed = () => { Destroy(FishUI[fishObject].gameObject); FishUI.Remove(fishObject); };
        fishObjects.Add(fishObject);
        turnToObject[turn] = fishObject;
        fishObject.transform.position = startingLocation;

    }

    public void MoveFish(CombatManager.Turn turn, Vector3 destination, Action CompletedMove = null)
    {
        turnToObject[turn].SetDestination(destination);
        turnToObject[turn].ReachedDestination += CompletedMove;
        CompletedMove += () => turnToObject[turn].ReachedDestination -= CompletedMove;
    }

    public void AnimateAttack(CombatManager.Turn turn, CombatManager.Turn target, Action CompletedMove = null)
    {
        StartCoroutine(TempAttackAnim(turnToObject[turn].transform.position, turnToObject[target].transform.position, CompletedMove));
        //throw new NotImplementedException();
    }

    public void SelectFish(Action<CombatManager.Turn> action)
    {
        foreach(var fish in turnToObject)
        {
            fish.Value.EnableSelection();
            fish.Value.selectedFish = (a)=> {action?.Invoke(a); FinishedSelecting();};
        }
    }

    void FinishedSelecting()
    {
        foreach(var fish in turnToObject)
        {
            fish.Value.DisableSelection();
            fish.Value.selectedFish = null;

        }
    }

    IEnumerator TempAttackAnim(Vector3 start, Vector3 destination, Action CompletedMove)
    {

        GameObject projectile = Instantiate(projectilPrefab, start, projectilPrefab.transform.rotation);
        while (Vector3.Distance(projectile.transform.position, destination) > 0.01f)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, destination, Time.deltaTime * 15);
            yield return new WaitForEndOfFrame();
        }
        Destroy(projectile.gameObject);
        CompletedMove?.Invoke();
    }


}
