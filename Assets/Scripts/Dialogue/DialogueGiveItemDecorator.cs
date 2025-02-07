using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGiveItemDecorator : DialogueDecorator
{

    [SerializeField]
    public Item item;
    [SerializeField]
    [Min(1)]
    public int amount;
    public DialogueGiveItemDecorator()
    {

    }

    public override void Enter()
    {
        //throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        

        GameManager.Instance.PlayerInventory.AddItem(item,amount);
    }
}
