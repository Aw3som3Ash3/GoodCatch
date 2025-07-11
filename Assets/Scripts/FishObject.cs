using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using static Unity.VisualScripting.Member;

public class FishObject : MonoBehaviour
{
    CombatManager.Turn turn;
    GameObject model;
    GameObject outline;
    Vector3 destination;
    bool shouldMove;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    Material outlineMat;
    [SerializeField]
    int outlineLayer;
    public Action ReachedDestination;

    public Action<Vector2> Navigate;
    public Action<CombatManager.Turn> selectedFish;

    public Action ObjectDestroyed;

    bool isSelectable;
    
    PlayableGraph playableGraph;
    //PlayableGraph idlePlayableGraph;
    AnimationPlayableOutput playableOutput;
    AnimationClipPlayable attackClipPlayable;
    AnimationMixerPlayable mixerPlayable;

    [SerializeField]
    AudioClip defaultClip;
    AudioSource source;
    Material[] defaultMats;
    public Transform hookLocation { get; private set; }
    private void Awake()
    {
        source= GetComponent<AudioSource>();
        EventTrigger eventTrigger= this.AddComponent<EventTrigger>();
        EventTrigger.Entry hoverEvent = new EventTrigger.Entry();
        hoverEvent.eventID = EventTriggerType.PointerEnter;
        hoverEvent.callback.AddListener((eventData) => { OnHover(true); });
        eventTrigger.triggers.Add(hoverEvent);

        EventTrigger.Entry hoverExitEvent = new EventTrigger.Entry();
        hoverExitEvent.eventID = EventTriggerType.PointerExit;
        hoverExitEvent.callback.AddListener((eventData) => { OnHover(false); });
        eventTrigger.triggers.Add(hoverExitEvent);

        EventTrigger.Entry selectedEvent = new EventTrigger.Entry();
        selectedEvent.eventID = EventTriggerType.Select;
        selectedEvent.callback.AddListener((eventData) => { OnHover(true); });
        eventTrigger.triggers.Add(selectedEvent);

        EventTrigger.Entry deselectedEvent = new EventTrigger.Entry();
        deselectedEvent.eventID = EventTriggerType.Deselect;
        deselectedEvent.callback.AddListener((eventData) => { OnHover(false); });
        eventTrigger.triggers.Add(deselectedEvent);

        EventTrigger.Entry clickEvent = new EventTrigger.Entry();
        clickEvent.eventID = EventTriggerType.PointerClick;
        clickEvent.callback.AddListener((eventData) => { Select(); });
        eventTrigger.triggers.Add(clickEvent);

        EventTrigger.Entry submitEvent = new EventTrigger.Entry();
        submitEvent.eventID = EventTriggerType.Submit;
        submitEvent.callback.AddListener((eventData) => { Select(); EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); });
        eventTrigger.triggers.Add(submitEvent);


        EventTrigger.Entry moveEvent = new EventTrigger.Entry();
        moveEvent.eventID = EventTriggerType.Move;
        moveEvent.callback.AddListener((eventData) => { Navigate?.Invoke(-InputManager.Input.UI.Navigate.ReadValue<Vector2>()); });
        eventTrigger.triggers.Add(moveEvent);
        playableGraph = PlayableGraph.Create();    
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

       

        
       


    }

    private void Select()
    {
        if (!isSelectable) return;
        selectedFish?.Invoke(turn);
        //throw new NotImplementedException();
    }

    private void OnHover(bool v)
    {
        if (!isSelectable) return;
        var rend = model.GetComponentInChildren<Renderer>();
        if (v)
        {
           
            var list= rend.materials.ToList();
            int amount = list.Count;
            for (int i = 0; i < amount; i++)
            {
                list.Add(outlineMat);
            }
            rend.materials=list.ToArray();
        }
        else
        {
            
            rend.materials = defaultMats;
            
        }
       
        print("hovering over fish");
        //throw new NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {


    }
    private void OnDestroy()
    {
        ObjectDestroyed?.Invoke();
        playableGraph.Destroy();
    }
    public void EnableSelection()
    {
        isSelectable = true;
    }
    public void DisableSelection()
    {
        OnHover(false);
        isSelectable = false;
        
    }
    public void SetFish(CombatManager.Turn turn)
    {
        this.turn = turn;
        
        model = Instantiate(turn.fish.Model, this.transform);
        model.transform.localPosition = Vector3.zero;
        model.layer = outlineLayer;
        defaultMats = model.GetComponentInChildren<Renderer>().materials;
        foreach (Transform child in model.transform)
        {
            child.gameObject.layer = outlineLayer;
        }
        //outline = Instantiate(model, this.transform);
        //outline.layer = outlineLayer;
        //foreach (Transform child in outline.transform)
        //{
        //    child.gameObject.layer = outlineLayer;
        //}
        ////outline.transform.localScale = Vector3.one*1.25f;
        //var rend = outline.GetComponentInChildren<Renderer>();
        //rend.material = outlineMat;
        //outline.SetActive(false);

        var anim = model.GetComponent<Animator>();
        if (anim == null) {
            model.AddComponent<Animator>();
        }
        playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", anim);
        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 3);
        playableOutput.SetSourcePlayable(mixerPlayable);
        

        
        var idleClipPlayable = AnimationClipPlayable.Create(playableGraph, turn.fish.IdleAnimation);

        attackClipPlayable = AnimationClipPlayable.Create(playableGraph, turn.fish.AttackAnimation);
        var buffClipPlayable = AnimationClipPlayable.Create(playableGraph, turn.fish.BuffAnimation);
        
        playableGraph.Connect(idleClipPlayable, 0, mixerPlayable, 0);
        playableGraph.Connect(attackClipPlayable, 0, mixerPlayable, 1);
        playableGraph.Connect(buffClipPlayable, 0, mixerPlayable, 2);
        mixerPlayable.SetInputWeight(0, 1.0f);
        mixerPlayable.SetInputWeight(1, 0f);
        mixerPlayable.SetInputWeight(2, 0f);
        
        attackClipPlayable.Pause();
        playableGraph.Play();


        hookLocation = model.GetComponent<FishModel>().HookLocation;

    }
    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        shouldMove = true;
        StartCoroutine(MoveToDestination());
    }
    public void AttackAnimation(Action animationCompleted)
    {
        mixerPlayable.SetInputWeight(0, 0);
        mixerPlayable.SetInputWeight(1, 1.0f);
        mixerPlayable.SetInputWeight(2, 0);
        StartCoroutine(AnimationTimer(animationCompleted));
        
        if (defaultClip != null)
        {
            source.PlayOneShot(defaultClip);
        }
       
        //playableGraph.

    }
    public void BuffAnimation(Action animationCompleted)
    {
        mixerPlayable.SetInputWeight(0, 0);
        mixerPlayable.SetInputWeight(1, 0);
        mixerPlayable.SetInputWeight(2, 1.0f);
        StartCoroutine(AnimationTimer(animationCompleted));

        if (defaultClip != null)
        {
            source.PlayOneShot(defaultClip);
        }

        //playableGraph.

    }
    IEnumerator AnimationTimer(Action animationCompleted)
    {
       
        
        attackClipPlayable.SetTime(0);
        playableGraph.Evaluate();
        float time = turn.fish.AttackAnimation.length;
        //playableOutput.SetSourcePlayable(clipPlayable);
        attackClipPlayable.Play();
        while (time > 0)
        {
            yield return new WaitForEndOfFrame();
            time-= Time.deltaTime;
        }
        mixerPlayable.SetInputWeight(0, 1.0f);
        mixerPlayable.SetInputWeight(1, 0);
    }
    public void StopMoving() 
    {
        shouldMove = false;
    }
    IEnumerator MoveToDestination()
    {
        while (shouldMove && Vector3.Distance(this.transform.position, destination) > 0.01f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination, moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        shouldMove = false;
        ReachedDestination?.Invoke();

    }

    
}
