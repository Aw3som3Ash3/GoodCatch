using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpeakToNPCQuestRequirement : Quest.QuestRequirement
{
    [SerializeField]
    string npcName;
    public string NpcName { get { return npcName; } }
    public override string Objective => $"Speak to {npcName}";

}
