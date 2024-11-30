using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemInventory
{
    [SerializeField]
    List<ItemSlot> items=new(); 
    //SerializableDictionary<Item, int> items  = new SerializableDictionary<Item, int>();

    //IReadOnlyList<ItemSlot> cachedItemList;

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
        if (itemSlot != null)
        {
            itemSlot.amount = amount;
        }
        else
        {
            items.Add(new ItemSlot(item));
        }
        
    }

    public void RemoveItem(Item item, int amount = 1)
    {
        var itemSlot = items.Find((slot) => slot.Item == item);

        if (itemSlot!=null)
        {
            itemSlot.amount -= Mathf.Clamp(amount, 0, itemSlot.amount);
            if (itemSlot.amount == 0)
            {
                items.Remove(itemSlot);
            }
        }
    }

    public IReadOnlyList<ItemSlot> GetListOfItems<T>()
    {

        return items.Where(slot => slot.Item is T).ToList();
    }
   

}
