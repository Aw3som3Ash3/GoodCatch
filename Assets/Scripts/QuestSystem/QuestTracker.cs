using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestTracker : MonoBehaviour,ISaveable,IUseDevCommands
{
    static public QuestTracker Instance;
    [SerializeField]
    Quest startingQuest;

   
    [SerializeField]
    List<Quest.QuestInstance> activeQuests=new();
    [SerializeField]
    List<Quest.QuestInstance> completedQuests= new();
    public Quest.QuestInstance currentQuest { get; private set; }
    int currentQuestIndex;

    public object DataToSave => (activeQuests, completedQuests, currentQuestIndex);

    public event Action<Quest.QuestInstance> OnCurrentQuestUpdate;
    public event Action<Quest.QuestInstance> OnQuestUpdate;
    //public event Action<Quest.QuestInstance> OnNewQuest;

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
    [DevConsoleCommand("CompleteCurrentQuest")]
    public static void CompleteCurrentQuest()
    {

        Instance.ForceCompleteQuest(Instance.currentQuest.Quest);
    }
    public void ForceCompleteQuest(Quest quest)
    {
        List<Quest.QuestInstance> questInstanceToRemove = new();
        activeQuests.ForEach((questInstance) =>
        {
            if (questInstance.Quest == quest)
            {
                questInstanceToRemove.Add(questInstance);
            }
        });
        questInstanceToRemove.ForEach((questInstance) => 
        {
            activeQuests.Remove(questInstance);
            completedQuests.Add(questInstance);
        });
    }
    public void AddQuest(Quest quest,bool makeCurrent=false)
    {
        if(activeQuests.Select(x=>x.Quest).ToList().Contains(quest)|| completedQuests.Select(x => x.Quest).ToList().Contains(quest))
        {
            return;
        }
        var newQuest = quest.GenerateQuest();
        //newQuest.Progressed += (state,requirement) => OnQuestUpdate(newQuest);
        activeQuests.Add(newQuest);
        if (makeCurrent|| currentQuest==null)
        {
            MakeCurrent(newQuest);
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
        currentQuestIndex=activeQuests.IndexOf(currentQuest);

        currentQuest.Progressed += CurrentQuestProgressed;

        currentQuest.Completed += CurrentQuestCompleted;

        currentQuest.NewState += CurrentQuestNewState;
        OnCurrentQuestUpdate?.Invoke(currentQuest);
    }

    private void CurrentQuestProgressed(Quest.QuestState state, Quest.QuestRequirement requirement)
    {
        OnCurrentQuestUpdate?.Invoke(currentQuest);
    }

    private void CurrentQuestCompleted()
    {
        OnCurrentQuestUpdate?.Invoke(currentQuest);
        RemoveCurrent();

        if (activeQuests.Count > 0)
        {
            MakeCurrent(activeQuests[0]);
        }
       
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
        var data = JsonUtility.FromJson<(List<Quest.QuestInstance> active, List<Quest.QuestInstance> complete,int currentQuestIndex)>(json);
        activeQuests=data.active;
        completedQuests = data.complete;
        currentQuestIndex = data.currentQuestIndex;
        MakeCurrent(activeQuests[currentQuestIndex]);
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
            if(quest==_quest.Quest &&quest.States.First(x=> { Debug.Log(x.Name+" vs"+ name); return x.Name.Equals(name); }).Name.Equals(name))
            {
                Debug.Log("has quest state");
                return true;
            }
            
            
        }
        return false;
    }
}
