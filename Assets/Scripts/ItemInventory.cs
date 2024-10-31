using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemInventory
{
    public Dictionary<Item, int> items { get; private set; } = new Dictionary<Item, int>();

    IReadOnlyList<Item> cachedItemList;


    public void AddItem(Item item, int amount = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] += 1;
        }
        else
        {
            items.Add(item, amount);
        }
    }

    public void RemoveItem(Item item, int amount = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] -= Mathf.Clamp(amount, 0, items[item]);
            if (items[item] == 0)
            {
                items.Remove(item);
            }
        }
    }

    public IReadOnlyList<Item> GetListOfItems()
    {
        if (cachedItemList != null || cachedItemList.Count != items.Count)
        {
            cachedItemList = items.Keys.ToList();
        }
        return cachedItemList;
    }

    //int Sort(Item x, Item y)
    //{
    //    if (orderBy == OrderBy.Name)
    //    {
    //        return x.name.CompareTo(y.name);
    //    }else if (orderBy==OrderBy.Type)
    //    {
    //        return x.GetType().Name.CompareTo(y.GetType().Name);
    //    }
    //    else
    //    {
    //        return -1;
    //    }
    //}
}
