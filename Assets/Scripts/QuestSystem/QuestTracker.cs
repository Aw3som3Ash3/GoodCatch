using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestTracker : MonoBehaviour,ISaveable
{
    static public QuestTracker Instance;
    [SerializeField]
    Quest startingQuest;

   
    [SerializeField]
    List<Quest.QuestInstance> activeQuests=new();
    [SerializeField]
    List<Quest.QuestInstance> completedQuests= new();
    public Quest.QuestInstance currentQuest { get; private set; }

    public object DataToSave => (activeQuests, completedQuests);

    public event Action<Quest.QuestInstance> OnCurrentQuestUpdate;
    public event Action<Quest.QuestInstance> OnQuestUpdate;

    public string ID => "QuestTracker";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
       
       
    }
    // Start is called before the first frame update
    void Start()
    {
        //if(activeQuests)
        AddQuest(startingQuest);
        MakeCurrent(activeQuests[0]);
        OnCurrentQuestUpdate?.Invoke(currentQuest);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddQuest(Quest quest,bool makeCurrent=false)
    {
        var newQuest = quest.GenerateQuest();
        //newQuest.Progressed += (state,requirement) => OnQuestUpdate(newQuest);
        activeQuests.Add(newQuest);
        if (makeCurrent|| currentQuest==null)
        {
            MakeCurrent(currentQuest);
        }
        newQuest.Completed += () => 
        { 
            activeQuests.Remove(newQuest); 
            completedQuests.Add(newQuest);
        };
        newQuest.Progressed += (questState, questRequirement) =>
        {
            OnQuestUpdate?.Invoke(newQuest);
        };
        newQuest.NewState += (questState) =>
        {
            OnQuestUpdate?.Invoke(newQuest);
        };
    }
    void MakeCurrent(Quest.QuestInstance quest)
    {
        RemoveCurrent();
        currentQuest = quest;

        currentQuest.Progressed += CurrentQuestProgressed;

        currentQuest.Completed += CurrentQuestCompleted;

        currentQuest.NewState += CurrentQuestNewState;
    }

    private void CurrentQuestProgressed(Quest.QuestState state, Quest.QuestRequirement requirement)
    {
        OnCurrentQuestUpdate?.Invoke(currentQuest);
    }

    private void CurrentQuestCompleted()
    {
        OnCurrentQuestUpdate?.Invoke(currentQuest);
        RemoveCurrent();
       
    }

    private void CurrentQuestNewState(Quest.QuestState state)
    {
        OnCurrentQuestUpdate?.Invoke(currentQuest);
       
    }

    void RemoveCurrent()
    {
        if (currentQuest == null)
        {
            return;
        }
        currentQuest.Progressed -= CurrentQuestProgressed;

        currentQuest.Completed -= CurrentQuestCompleted;

        currentQuest.NewState -= CurrentQuestNewState;


        currentQuest = null;
    }
    public void Load(string json)
    {
        var data = JsonUtility.FromJson<(List<Quest.QuestInstance> active, List<Quest.QuestInstance> complete)>(json);
        activeQuests=data.active;
        completedQuests = data.complete;
        OnCurrentQuestUpdate?.Invoke(currentQuest);
        
    }

    

    public List<T> FindActiveRequirements<T>(Func<T, bool> predicate) where T : Quest.QuestRequirement
    {
        List<T> list = new();
        foreach(Quest.QuestInstance quest in activeQuests)
        {
            Debug.Log(quest.CurrentState.requirements.Where((x) => x is T).ToList() as List<T>);
            list.AddRange(quest.CurrentState.requirements.Where((x) => x is T).Select(x=>x as T));
        }
        return list;
       //return activeQuests.Select(x => 
       //{
       //    //Debug.Log(x);
       //    return (x.CurrentState.requirements.ToList() as IEnumerable<T>)?.FirstOrDefault(predicate); 
       //});
    }

    public bool IsQuestStateActive(Quest quest,string name)
    {

        foreach( var _quest in activeQuests)
        {
            Debug.Log(_quest);
            if(quest==_quest.Quest &&quest.States.First(x=> { Debug.Log(x.Name+" vs"+ name); return x.Name.Equals(name); }) == _quest.CurrentState)
            {
                Debug.Log("has quest state");
                return true;
            }
            
            
        }
        return false;
    }
}
