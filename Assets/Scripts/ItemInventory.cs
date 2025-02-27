using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

[Serializable]
public class ItemInventory
{
    [SerializeField]
    List<ItemSlot> items=new();
    //SerializableDictionary<Item, int> items  = new SerializableDictionary<Item, int>();

    //IReadOnlyList<ItemSlot> cachedItemList;
    public event Action<Item,int> ItemAdded;

    [Serializable]
    public class ItemSlot
    {
        Item item;
        public Item Item { get 
            {
                if (item == null)
                {
                    item = Item.getItemById[itemId];
                    //find item by id
                }
                return item;
            } }
        public string itemId;
        public int amount;
        public ItemSlot(Item item)
        {
            this.item = item;
            itemId =item.ItemId;
            amount = 1;
        }

    }


    public bool Contains(Item item)
    {
        var itemSlot = items.Find((slot) => slot.Item == item);
        return itemSlot != null;
    }
    public int GetAmount( Item item)
    {
        var itemSlot = items.Find((slot) => slot.Item == item);
        if (itemSlot != null )
        {
            return itemSlot.amount;
        }
        return 0;
       
    }
    public void AddItem(Item item, int amount = 1)
    {
        var itemSlot=items.Find((slot) => slot.Item == item);
        Debug.Log("max amount " + item.MaxAmount);
        if (itemSlot != null)
        {
            if (item is KeyItem)
            {
                Debug.Log("already has key item");
                return;
            }
            if (item.MaxAmount==-1||itemSlot.amount<item.MaxAmount)
            {
                itemSlot.amount += amount;
            }
           
        }
        else
        {
            var slot = new ItemSlot(item);
            slot.amount = amount;
            items.Add(slot);

        }
        ItemAdded?.Invoke(item, amount);
    }

    public void RemoveItem(Item item, int amount = 1)
    {
        var itemSlot = items.Find((slot) => slot.Item == item);
        
        if (itemSlot!=null)
        {
            if(item is KeyItem||item.IsDeletable)
            {
                Debug.Log("Item cannot be deleted");
                return;
            }
            itemSlot.amount -= Mathf.Clamp(amount, 0, itemSlot.amount);
            if (itemSlot.amount == 0)
            {
                items.Remove(itemSlot);
            }
        }
    }
    public bool HasKeyItem(KeyItem keyItem)
    {

        return items.Select(slot => slot.Item).Contains(keyItem);
    }
    public IReadOnlyList<ItemSlot> GetListOfItems<T>()
    {

        return items.Where(slot => slot.Item is T).ToList();
    }
   

}
