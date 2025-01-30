using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

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
        AddQuest(startingQuest);
        currentQuest = activeQuests[0];
        currentQuest.Progressed += (state, requirement) => { OnQuestUpdate?.Invoke(currentQuest); };
        OnQuestUpdate?.Invoke(currentQuest);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddQuest(Quest quest)
    {
        var newQuest = quest.GenerateQuest();
       
        activeQuests.Add(newQuest);

    }

    public void Load(string json)
    {
        var data = JsonUtility.FromJson<(List<Quest.QuestInstance> active, List<Quest.QuestInstance> complete)>(json);
        activeQuests=data.active;
        completedQuests = data.complete;
    }
}
