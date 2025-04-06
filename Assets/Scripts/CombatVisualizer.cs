using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static CombatManager;
using static UnityEngine.Rendering.DebugUI;

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
    CombatManager combatManager;
    CombatUI combatUI { get { return combatManager.combatUI; } }
    [SerializeField]
    GameObject fishUIPrefab;

    int selected;
    CombatManager.Turn selectedFish;
    CombatDepth currentDepth;

    List<CombatDepth> combatDepths;
    [SerializeField]
    CombatHooking floaterPrefab;

    [SerializeField]
    List<DepthSelectors> depthSelectors;
    EventSystem eventSystem;
    [SerializeField]
    Transform floaterStart;
    [SerializeField] Transform cameraTarget;
    //public Action CompletedMove;


    Action canceled;
    Action<int> DepthSelection;

    private void Awake()
    {
        combatManager = FindObjectOfType<CombatManager>();
        combatDepths = combatManager.depths.ToList();
    }
    // Start is called before the first frame update
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
        if (!turnToObject.ContainsKey(turn))
        {
            return;
        }
        var ui = FishUI[turnToObject[turn]];
        Destroy(turnToObject[turn].gameObject);
        turnToObject.Remove(turn);
        if (ui != null&&ui.parent!=null)
        {
            ui.parent.Remove(ui);
        }
       
    }
    public void AddFish(CombatManager.Turn turn, Vector3 startingLocation, CombatManager.Team team)
    {
        FishObject fishObject = Instantiate(fishObjectPrefab, this.transform).GetComponent<FishObject>();
        fishObject.transform.localEulerAngles = new Vector3(0, team == CombatManager.Team.player ? 90 : -90,0);
        fishObject.SetFish(turn);
        FishUI[fishObject] = combatUI.AddFishUI(turn, fishObject.transform);
        fishObjects.Add(fishObject);
        turnToObject[turn] = fishObject;
        fishObject.transform.position = startingLocation;
        

    }
   
    public void AnimateDamageNumbers(Turn turn,float damage,Element.Effectiveness effectiveness, bool healing=false)
    {
        if (!turnToObject.ContainsKey(turn))
        {
            return;
        }
        var textObj = new GameObject("damage text", typeof(TextMeshPro));
        textObj.transform.position = turnToObject[turn].transform.position;
        textObj.transform.rotation = Camera.main.transform.rotation;
        textObj.layer = 16;
        var tmp=textObj.GetComponent<TextMeshPro>();
        tmp.text = damage.ToString("00");
        tmp.alignment=TMPro.TextAlignmentOptions.Center;
        tmp.fontSize = 11;
        switch (effectiveness)
        {
            case Element.Effectiveness.none:
                tmp.color = Color.white;
                break;
            case Element.Effectiveness.strong:
                tmp.color = Color.gray;
                break;
            case Element.Effectiveness.veryStrong:
                tmp.color = Color.gray;
                break;
            case Element.Effectiveness.weak:
                tmp.color = Color.yellow;
                break;
            case Element.Effectiveness.veryWeak:
                tmp.color = Color.red;
                break;
            case Element.Effectiveness.healing:
                tmp.color = Color.green;
                break;
            default:
                tmp.color = Color.white;
                break;
        }
        StartCoroutine(AnimateDamageNumbers(textObj));
    }
    IEnumerator AnimateDamageNumbers(GameObject damageNumber)
    {
        float time = 2;
        while (time>0)
        {
            damageNumber.transform.Translate(Vector3.up * 5 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            time -= Time.deltaTime;
        }
        Destroy(damageNumber);
    }

    public void TargetCameraToFish(Turn turn)
    {
        cameraTarget.position = turnToObject[turn].transform.position;
    }
    public void MoveFish(CombatManager.Turn turn, Vector3 destination, Action CompletedMove = null)
    {
        if (!turnToObject.ContainsKey(turn))
        {
            return;
        }
        turnToObject[turn].SetDestination(destination);
        turnToObject[turn].ReachedDestination += CompletedMove;
        CompletedMove += () => turnToObject[turn].ReachedDestination -= CompletedMove;
    }
    public void AnimateBasicVFX(CombatManager.Turn target, ParticleSystem vfxPrefab)
    {
        if (!turnToObject.ContainsKey(target)||vfxPrefab==null)
        {
            return;
        }
        var vfx = Instantiate(vfxPrefab, turnToObject[target].transform.position, turnToObject[target].transform.rotation);
        var main = vfx.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
        vfx.Play();
    }
    public void AnimateAttack(Ability ability,CombatManager.Turn turn, CombatManager.Turn target, Action CompletedMove = null)
    {
        if (target==null||!turnToObject.ContainsKey(target))
        {
            return;
        }
        //StartCoroutine(TempAttackAnim(turnToObject[turn].transform.position, turnToObject[target].transform.position, CompletedMove));
        if (ability.TargetedTeam == Ability.TargetTeam.enemy)
        {
            turnToObject[turn].AttackAnimation(() => { });
        }
        else
        {
            turnToObject[turn].BuffAnimation(() => { });
        }

        //if (ability.AbilityVFX)
        //{

        //}
        StartCoroutine(ParticleAttackAnim(ability, turnToObject[turn].transform.position, turnToObject[target].transform.position, CompletedMove));
        //throw new NotImplementedException();
    }

    public void SelectFish(CombatManager.Team team,Action<CombatManager.Turn> action,Action canceledCallback)
    {
        StopTargeting();
        canceled += canceledCallback;
        foreach (var fish in turnToObject)
        {
            if (fish.Key.team == team)
            {
                if (selectedFish == null&&InputManager.inputMethod==InputMethod.controller)
                {
                    selectedFish = fish.Key;
                }
                
                fish.Value.EnableSelection();
                fish.Value.selectedFish = (a) => { action?.Invoke(a); FinishedSelecting(); };
                fish.Value.Navigate += OnFishNavigate;
            }
           
        }
        foreach (CombatDepth depth in combatDepths)
        {
            if (depth.HasFish(selectedFish))
            {
                currentDepth = depth;
            }
        }
        if (InputManager.inputMethod == InputMethod.controller)
        {
            eventSystem.SetSelectedGameObject(turnToObject[selectedFish].gameObject);

        }
          
    }
    public void StopSelectingFish()
    {
        foreach (var fish in turnToObject)
        {
            if (fish.Key.team == Team.enemy)
            {

                fish.Value.DisableSelection();
                fish.Value.selectedFish = null;
                fish.Value.Navigate -= OnFishNavigate;
            }

        }
    }

    public void FinishedSelecting()
    {
        Debug.Log("should disable selections");
        foreach(var fish in turnToObject)
        {
            fish.Value.DisableSelection();
            fish.Value.selectedFish = null;
            fish.Value.Navigate -= OnFishNavigate;
            selectedFish = null;
        }
    }
    public void DoStatusEffect(StatusEffect effect, Turn turn, Action CompletedMove)
    {
        StartCoroutine(ParticleStatusEffect(effect, turn, CompletedMove));
    }

    IEnumerator ParticleStatusEffect(StatusEffect effect,Turn turn ,Action CompletedMove)
    {
        if (effect.ParticlePrefab != null)
        {
            var particleEffect = Instantiate(effect.ParticlePrefab, turnToObject[turn].transform.position, Quaternion.identity);
            var main = particleEffect.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
            //beam.SetParticles(beam,)
            //lifetime.constant = 0;
            particleEffect.Play();
            yield return new WaitForSeconds(particleEffect.main.duration + particleEffect.main.startLifetime.constant);
        }
        else
        {
            Debug.LogWarning("missing animation on ability");
            yield return new WaitForNextFrameUnit();
        }
        CompletedMove?.Invoke();
    }
    IEnumerator ParticleAttackAnim(Ability ability,Vector3 start, Vector3 destination, Action CompletedMove)
    {
        
       
        if (ability.AbilityVFX!=null)
        {
            Vector3 targetDir = destination - start;
            var beam = Instantiate(ability.AbilityVFX, start, Quaternion.LookRotation(targetDir));
            var main = beam.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
            main.startLifetime = targetDir.magnitude / beam.main.startSpeed.constant;
            //beam.SetParticles(beam,)
            //lifetime.constant = 0;
            float dist = beam.main.startSpeed.constant * beam.main.startLifetime.constant;
            beam.Play();
            
            
            yield return new WaitForSeconds(beam.main.duration + beam.main.startLifetime.constant);
        }
        else
        {
            Debug.LogWarning("missing animation on ability");
            yield return new WaitForNextFrameUnit();
        }
        if(ability.TargetVFX != null)
        {

        }
        CompletedMove?.Invoke();
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
    //IEnumerator TempFishAttackAnim()
    //{

    //}
    void OnNaviagte(float i)
    {
        print(i);
        if (i > 0)
        {
            selected++;

        }
        else if (i < 0)
        {
            selected--;

        }
        selected = Mathf.Clamp(selected, 0, depthSelectors.Count - 1);
        print(selected);

        eventSystem.SetSelectedGameObject(depthSelectors[selected].gameObject);

    }
    void OnFishNavigate(Vector2 input)
    {
        print("navigate input:" +input);
        if (input.y > 0)
        {
            int tries=0;
            CombatDepth targetDepth=null;
            do
            {
                targetDepth = combatDepths[Mathf.Clamp(combatDepths.IndexOf(currentDepth) + (1+tries), 0, combatDepths.Count)];
               
                tries++;
            } while (tries<combatDepths.Count &&targetDepth.enemy[0]==null);
            selectedFish = targetDepth.enemy[0];
            currentDepth = targetDepth;
        }
        else if(input.y<0)
        {
            int tries = 0;
            CombatDepth targetDepth = null;
            do
            {
                targetDepth = combatDepths[Mathf.Clamp(combatDepths.IndexOf(currentDepth) - (1 + tries), 0, combatDepths.Count)];

                tries++;
            } while (tries < combatDepths.Count && targetDepth.enemy[0] == null);
            selectedFish = targetDepth.enemy[0];
            currentDepth = targetDepth;

        } else if (input.x>0)
        {
            int targetIndex = currentDepth.enemy.IndexOf(selectedFish) + 1;
            if (targetIndex < currentDepth.enemy.Count)
            {
                selectedFish = currentDepth.enemy[targetIndex];
            }
        }else if (input.x < 0)
        {
            int targetIndex = currentDepth.enemy.IndexOf(selectedFish) - 1;
            if (targetIndex >= 0)
            {
                selectedFish = currentDepth.enemy[targetIndex];
            }
        }
        eventSystem.SetSelectedGameObject(turnToObject[selectedFish].gameObject);

    }
    public void StartTargeting(Func<int,Depth,bool> targetable,int ablityIndex ,Action<int> targeted)
    {
        StopTargeting();
       
        DepthSelection = null;
        DepthSelection += targeted;
        DepthSelection +=(x)=>StopTargeting();
        selected = -1;
        foreach (DepthSelectors selector in depthSelectors)
        {
            if (targetable(ablityIndex,selector.CurrentDepth))
            {
               
                selector.SetSelection(true);
                if (selected < 0 && InputManager.inputMethod == InputMethod.controller)
                {
                    selected = depthSelectors.IndexOf(selector);
                    eventSystem.SetSelectedGameObject(selector.gameObject);
                }

            }
            else
            {
                selector.SetSelection(false);
            }

        }
        

    }
    public void StartTargeting(Action<int> targeted)
    {
        StopTargeting();
       
        DepthSelection = null;
        foreach (DepthSelectors selector in depthSelectors)
        {
            selector.SetSelection(true);
        }
        if (InputManager.inputMethod == InputMethod.controller)
        {
            eventSystem.SetSelectedGameObject(depthSelectors[0].gameObject);
        }
        DepthSelection =targeted;

    }
    void StopTargeting()
    {
        //DepthSelection?.Invoke(-1);
        DepthSelection = null;
        foreach (DepthSelectors selector in depthSelectors)
        {
            selector.SetSelection(false);
            selector.PreviewSelection(false);
        }
        DepthSelection = null;
        //foreach (var fish in turnToObject)
        //{
        //    fish.Value.DisableSelection();

        //}
        StopSelectingFish();
        //isActive = true;
       
    }

    public bool CancelMove()
    {
        if (canceled == null)
        {
            return false;
        }
        DepthSelection?.Invoke(-1);
       
        StopTargeting();
        canceled?.Invoke();
        canceled = null;
        return true;

    }

    public void CatchFishAnimation(Turn turn,bool catchingCalc,Action completed)
    {
        var targetFish = turnToObject[turn];
        Vector3 startPos = new Vector3(targetFish.hookLocation.position.x,floaterStart.position.y+1, targetFish.hookLocation.position.z);
        CombatHooking hook = Instantiate(floaterPrefab, startPos, floaterPrefab.transform.rotation);
        hook.HookingLocation(targetFish, catchingCalc, completed);
    }
}
