using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Quest;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest System/Quest", order = 3)]

public class Quest : ScriptableObject,ISerializationCallbackReceiver
{
    public static Dictionary<string, Quest> getQuestById = new();
    
    public enum QuestType
    {
        Main,
        SideQuest,

    }

    [Serializable]
    public class QuestState
    {
        [SerializeField]
        string name;
        public string Name { get { return name; } }
        [SerializeField]
        [Multiline]
        string description;
        public string Description { get { return description; } }


        //[SerializeField]
        //string objective;
        public string Objective 
        { 
            get 
            {
                string newString= "";
                foreach (var requirement in requirements)
                {
                    if (requirement.IsCompleted)
                    {
                        newString += "<s>" + requirement.Objective + " </s>\n";
                    }
                    else
                    {
                        newString += requirement.Objective + "\n";
                    }
                   
                }
                return newString;

            } 
        }

        [SerializeField]
        [SerializeReference]
        public QuestRequirement[] requirements;
        //public QuestRequirement[] Requirements { get { return requirements; } }

        public event Action Completed;
        public event Action<QuestState, QuestRequirement> Progressed;
        
        //public event Action<QuestRequirment> Progressed;
        public void Initialize()
        {
            
            foreach (QuestRequirement requirment in requirements)
            {
                
                requirment.Init();
                requirment.Completed += CheckIfCompleted;
                requirment.Progressed += OnProgress;
            }

        }
        public void Reinitialize()
        {

            foreach (QuestRequirement requirment in requirements)
            {
                requirment.Completed -= CheckIfCompleted;
                requirment.Progressed -= OnProgress;
                requirment.ReInit();
                requirment.Completed += CheckIfCompleted;
                requirment.Progressed += OnProgress;
            }

        }
        void OnProgress(QuestRequirement questRequirement)
        {
            Progressed?.Invoke(this, questRequirement);
        }
        void CheckIfCompleted()
        {
            foreach (var requirment in requirements)
            {
                if (!requirment.IsCompleted)
                {
                    return;
                }

            }
            Completed?.Invoke();
        }

        //public object Clone()
        //{
        //    Debug.Log(this + " has been cloned");
        //    var state= new QuestState();
        //    state.name = name;
        //    state.description = description;
        //    state.requirements = requirements.Select(a => (QuestRequirement)a.Clone()).ToArray(); ;
        //    return state;
            
        //}
    }
    [Serializable]
    public abstract class QuestRequirement
    {
        [HideInInspector]
        public string Name;
        public event Action Completed;
        public event Action<QuestRequirement> Progressed;
       
        public abstract string Objective { get; }
        [HideInInspector]
        public bool IsCompleted;
        public QuestRequirement()
        {
            Name = this.GetType().Name;
        }
        public virtual void Init() 
        {
            IsCompleted = false;
        }
        public virtual void ReInit()
        {

        }
        public void RequirementCompleted()
        {
            IsCompleted = true;
            Completed?.Invoke();
        }

        protected void RequirementProgressed()
        {
            Progressed?.Invoke(this);
        }

        //public object Clone()
        //{
        //    Debug.Log(this + " has been cloned");
        //    //req.Name = Name;
        //    return this.MemberwiseClone();
        //}
    }

    [Serializable]
    public class QuestInstance
    {
        Quest quest;
        public Quest @Quest { get { if (quest == null) { quest = Quest.getQuestById[questId]; } return quest; } set { quest = value; } }
        [SerializeField]
        string questId;
        public string QuestName { get { return @Quest.name; } }
        public string QuestDescription { get { return @Quest.description; } }

        //public string QuestDescription { get { return quest.description; } }
        public QuestType type { get { return @Quest.type; } }
        [SerializeField]
        public QuestState[] states;
        
        [SerializeField]
        int currentStateIndex;
       
        public QuestState CurrentState 
        { 
            get 
            {
                if (states.Length <= 0|| states==null || currentStateIndex>=states.Length)
                {
                    return null;
                }
                return states[currentStateIndex]; 
            } 
        }
        public event Action Completed;
        public event Action<QuestState, QuestRequirement> Progressed;
        public event Action<QuestState> NewState;
        
        public QuestInstance(Quest quest)
        {
            this.quest = quest;
            questId = quest.questId;
            states = quest.states;
            //states = quest.states.Select(a => (QuestState)a.Clone()).ToArray();
            //foreach (QuestState state in states)
            //{
                
            //    state.Initialize();
            //}
            currentStateIndex = 0;
           
            NewQuestState();
        }

        void NewQuestState()
        {
           
            if (CurrentState == null)
            {
                return;
            }
            CurrentState.Initialize();
            CurrentState.Completed += StateCompleted;
            CurrentState.Progressed += Progressed;

            NewState?.Invoke(CurrentState);
        }
        void StateCompleted()
        {
            CurrentState.Completed -= StateCompleted;
            CurrentState.Progressed -= Progressed;
            currentStateIndex++;
            if (currentStateIndex < states.Length)
            {
                NewQuestState();
            }
            else
            {
                Completed?.Invoke();
            }

        }
        public void ReInitialize()
        {

            CurrentState.Completed -= StateCompleted;
            CurrentState.Progressed -= Progressed;
            CurrentState.Completed += StateCompleted;
            CurrentState.Progressed += Progressed;
            CurrentState.Reinitialize();
        }

    }
    string questId;
    [SerializeField]
    QuestType type;
    [SerializeField]
    [Multiline]
    string description;
    [SerializeField]
    QuestState[] states;
    public QuestState[] States { get { return states; } }

    public QuestInstance GenerateQuest()
    {
        return new QuestInstance(this);
    }
    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        if (questId == null || questId == "" || (getQuestById.ContainsKey(questId) && getQuestById[questId] != this))
        {
            questId = Guid.NewGuid().ToString();

        }
        getQuestById[questId] = this;


        
    }
}


[Serializable]
public class CatchNumOfFishRequirement : Quest.QuestRequirement
{
    [SerializeField]
    protected int targetOfFish;
    [SerializeField]
    [HideInInspector]
    protected int currentAmount;

    public override string Objective => $"Catch fish {currentAmount}/{targetOfFish}";

    public override void Init()
    {
        base.Init();
        currentAmount = 0;
        GameManager.Instance.CaughtFish += OnFishCaught;
        //throw new NotImplementedException();
    }

    protected virtual void OnFishCaught(FishMonsterType type)
    {
        currentAmount++;
       
        if (currentAmount >= targetOfFish)
        {
            RequirementCompleted();
            GameManager.Instance.CaughtFish -= OnFishCaught;
        }
        else
        {
            RequirementProgressed();
        }
    }
}
[Serializable]
public class CatchNumOfSpecificFishRequirement : CatchNumOfFishRequirement
{
    [SerializeField]
    FishMonsterType fishMonsterType;

    public override string Objective => $"Catch  {fishMonsterType.name}: {currentAmount}/{targetOfFish}";
    protected override void OnFishCaught(FishMonsterType type)
    {
        if (fishMonsterType == type)
        {
            base.OnFishCaught(type);
        }
        
    }
}

[Serializable]
public class GatherAmountOfItems : Quest.QuestRequirement
{
    [SerializeField]
    Item item;
    [SerializeField]
    int amount;
    int progress;

    public override string Objective => $"Gather {amount} {item}: {progress}/{amount} ";

    public override void Init()
    {
        base.Init();
        amount = 0;
      
        GameManager.Instance.PlayerInventory.ItemAdded += ItemHasBeenAdded;
    }

    private void ItemHasBeenAdded(Item item, int amount)
    {
        if (item == this.item)
        {
            progress += amount;
           

            if (progress >= amount)
            {
                RequirementCompleted();
                GameManager.Instance.PlayerInventory.ItemAdded -= ItemHasBeenAdded;
            }
            else
            {
                RequirementProgressed();
            }
        }

        


        //throw new NotImplementedException();
    }
}


public class ArbritaryQuestRequirment : QuestRequirement
{
    [SerializeField]
    string tag;
    public string Tag { get { return tag; } }
    [SerializeField]
    string objective;
    public override string Objective => objective;

   
}


