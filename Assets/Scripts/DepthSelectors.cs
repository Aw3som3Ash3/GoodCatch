using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DepthSelectors : MonoBehaviour
{
    [SerializeField]
    Depth currentDepth;
    [SerializeField]
    GameObject visualizer, targetingMarker;
    [SerializeField]
    Color color, hoverColor;
    public Depth CurrentDepth { get { return currentDepth; } }
    public Action<int> Selected;
    public Action<float> Navigate;

    [SerializeField]
    Transform playerSide, enemySide;
    bool selectorEnabled;

    int index;
    // Start is called before the first frame update
    void Start()
    {

        visualizer.GetComponent<Renderer>().material.color = color;

        EventTrigger.Entry hoverEvent = new EventTrigger.Entry();
        hoverEvent.eventID = EventTriggerType.PointerEnter;
        hoverEvent.callback.AddListener((eventData) => { OnHover(true); });
        GetComponent<EventTrigger>().triggers.Add(hoverEvent);

        EventTrigger.Entry hoverExitEvent = new EventTrigger.Entry();
        hoverExitEvent.eventID = EventTriggerType.PointerExit;
        hoverExitEvent.callback.AddListener((eventData) => { OnHover(false); });
        GetComponent<EventTrigger>().triggers.Add(hoverExitEvent);

        EventTrigger.Entry selectedEvent = new EventTrigger.Entry();
        selectedEvent.eventID = EventTriggerType.Select;
        selectedEvent.callback.AddListener((eventData) => { OnHover(true); });
        GetComponent<EventTrigger>().triggers.Add(selectedEvent);

        EventTrigger.Entry deselectedEvent = new EventTrigger.Entry();
        deselectedEvent.eventID = EventTriggerType.Deselect;
        deselectedEvent.callback.AddListener((eventData) => { OnHover(false); });
        GetComponent<EventTrigger>().triggers.Add(deselectedEvent);

        EventTrigger.Entry clickEvent = new EventTrigger.Entry();
        clickEvent.eventID = EventTriggerType.PointerClick;
        clickEvent.callback.AddListener((eventData) => { if((eventData as PointerEventData).button == PointerEventData.InputButton.Left)SelectDepth();});
        GetComponent<EventTrigger>().triggers.Add(clickEvent);

        EventTrigger.Entry submitEvent = new EventTrigger.Entry();
        submitEvent.eventID = EventTriggerType.Submit;
        submitEvent.callback.AddListener((eventData) => { SelectDepth(); EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); });
        GetComponent<EventTrigger>().triggers.Add(submitEvent);

       
        EventTrigger.Entry moveEvent = new EventTrigger.Entry();
        moveEvent.eventID = EventTriggerType.Move;
        moveEvent.callback.AddListener((eventData) => { Navigate?.Invoke(Mathf.RoundToInt(-InputManager.Input.UI.Navigate.ReadValue<Vector2>().y));});
        GetComponent<EventTrigger>().triggers.Add(moveEvent);

        visualizer.SetActive(false);
        targetingMarker.SetActive(false);
    }
    public void SetIndex(int index)
    {
        this.index = index;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnNavigate()
    {

    }

    public void SetSelection(bool b)
    {
        selectorEnabled = b;
        visualizer.SetActive(b);
        if (!b)
        {
            targetingMarker.SetActive(false);
        }

    }
    public void PreviewSelection(bool b)
    {
        if (!selectorEnabled)
            visualizer.SetActive(b);
    }

    public void SelectDepth()
    {
        if (!selectorEnabled)
        {
            return;
        }
        targetingMarker.SetActive(false);
        Selected?.Invoke(index);

    }
    public void OnHover(bool b)
    {
        targetingMarker.SetActive(b&& selectorEnabled);
        visualizer.GetComponent<Renderer>().material.color = b ? hoverColor : color;

    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(playerSide.position, Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(enemySide.position, Vector3.one);
    }
#endif
}
