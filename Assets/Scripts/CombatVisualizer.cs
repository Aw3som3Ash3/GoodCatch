using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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
    UIDocument ui;
    CombatUI combatUI;
    [SerializeField]
    GameObject fishUIPrefab;

    int selected;
    [SerializeField]
    List<DepthSelectors> depthSelectors;
    EventSystem eventSystem;
    //public Action CompletedMove;
    // Start is called before the first frame update

    Action<int> DepthSelection;
    private void Awake()
    {
        combatUI = ui.rootVisualElement.Q<CombatUI>();
    }
    void Start()
    {
        eventSystem = EventSystem.current;
        for (int i = 0; i < depthSelectors.Count; i++)
        {
            depthSelectors[i].SetIndex(i);
            depthSelectors[i].Selected = (i) => { DepthSelection?.Invoke(i); StopTargeting(); };
            depthSelectors[i].Navigate += OnNaviagte;

        }
       
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var k in FishUI)
        {
            k.Value.UpdatePosition();
        }

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
        
        //GameObject uiObj= Instantiate(fishUIPrefab, canvas.transform);
        //uiObj.transform.SetSiblingIndex(0);
        FishUI[fishObject] = combatUI.AddFishUI(turn, fishObject.transform);
        //FishUI[fishObject].SetFish(turn, fishObject.transform);
        //fishObject.ObjectDestroyed = () => { Destroy(FishUI[fishObject].gameObject); FishUI.Remove(fishObject); };
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

    public void SelectFish(CombatManager.Team team,Action<CombatManager.Turn> action)
    {
        
        foreach(var fish in turnToObject)
        {
            if (fish.Key.team == team)
            {
                fish.Value.EnableSelection();
                fish.Value.selectedFish = (a) => { action?.Invoke(a); FinishedSelecting(); };
            }
           
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

    void OnNaviagte(float i)
    {
        float value = i;

        if (value > 0)
        {
            selected++;

        }
        else if (value < 0)
        {
            selected--;

        }
        selected = Mathf.Clamp(selected, 0, depthSelectors.Count - 1);
        print(selected);

        eventSystem.SetSelectedGameObject(depthSelectors[selected].gameObject);

    }
    public void StartTargeting(Func<int,Depth,bool> targetable,int ablityIndex ,Action<int> targeted)
    {
        DepthSelection += targeted;
        foreach (DepthSelectors selector in depthSelectors)
        {
            if (targetable(ablityIndex,selector.CurrentDepth))
            {
                eventSystem.SetSelectedGameObject(selector.gameObject);
                selected = depthSelectors.IndexOf(selector);
                selector.SetSelection(true);
                
            }
            else
            {
                selector.SetSelection(false);
            }

        }


    }
    public void StartTargeting(Action<int> targeted)
    {
        eventSystem.SetSelectedGameObject(depthSelectors[0].gameObject);
        foreach (DepthSelectors selector in depthSelectors)
        {
            selector.SetSelection(true);
        }
        DepthSelection +=targeted;

    }
    void StopTargeting()
    {
        foreach (DepthSelectors selector in depthSelectors)
        {
            selector.SetSelection(false);
            selector.PreviewSelection(false);
        }
        DepthSelection = null;
        
        //isActive = true;
    }
}
