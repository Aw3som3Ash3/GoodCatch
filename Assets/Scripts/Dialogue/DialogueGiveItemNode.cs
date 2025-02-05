using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGiveItemNode : BasicDialogue
{

    [SerializeField]
    public Item item;
    [SerializeField]
    [Min(1)]
    public int amount;
    public DialogueGiveItemNode()
    {

    }

    public override void Exit()
    {
        base.Exit();

        GameManager.Instance.PlayerInventory.AddItem(item,amount);
    }
}
