using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : ToggleableUIMenus
{
    [SerializeField]
    Button nameButton, typeButton, amountButton;
    [SerializeField]
    GameObject inventoryItemPrefab;
    [SerializeField]
    Transform contentZone;
    VerticalLayoutGroup layoutGroup;



    public enum OrderBy
    {
        Name,
        Type,
        Amount
    }
    OrderBy orderBy;
    bool descending;
    protected override InputAction input => InputManager.Input.UI.Inventory;

    // Start is called before the first frame update
    void Start()
    {
        layoutGroup = contentZone.GetComponent<VerticalLayoutGroup>();
        nameButton.onClick.AddListener(() => OrderList(OrderBy.Name));
        typeButton.onClick.AddListener(() => OrderList(OrderBy.Type));
        //amountButton.onClick.AddListener(()=>OrderList(OrderBy.Amount));
    }
    private void OnEnable()
    {

    }
    void OrderList(OrderBy orderBy)
    {
        if (this.orderBy == orderBy)
        {
            descending = !descending;

        }
        else
        {
            this.orderBy = orderBy;
            descending = false;
            PopulateList();
        }
        layoutGroup.reverseArrangement = descending;

    }
    void PopulateList()
    {
        var dictionary = GameManager.Instance.PlayerInventory.GetDictionaryOfItems<Item>();
        var list = OrderedList(dictionary.Keys.ToList(), orderBy);

        for (int i = 0; i < (list.Count > contentZone.childCount ? list.Count : contentZone.childCount); i++)
        {
            if (i < contentZone.childCount)
            {
                contentZone.GetChild(i).GetComponent<InventoryUIItem>().SetValues(list[i].name, list[i].Type,dictionary[list[i]]);
                if (i > list.Count)
                {
                    Destroy(contentZone.GetChild(i));
                }
            }
            else
            {
                var obj = Instantiate(inventoryItemPrefab, contentZone);
                obj.GetComponent<InventoryUIItem>().SetValues(list[i].name, list[i].Type, dictionary[list[i]]);
            }

        }
        //foreach (var item in GameManager.Instance.playerInventory.items)
        //{
        //    var obj = Instantiate(inventoryItemPrefab, contentZone);
        //    obj.GetComponent<InventoryUIItem>().SetValues(item.Key.name, item.Key.Type, item.Value);
        //}
    }
    List<Item> OrderedList(IEnumerable<Item> list, OrderBy orderBy)
    {

        if (orderBy == OrderBy.Name)
        {
            return list.OrderBy(x => x.name).ToList();
        }
        else if (orderBy == OrderBy.Type)
        {
            return list.OrderBy(x => x.GetType().Name).ThenBy(x => x.name).ToList();
        }
        return list.ToList();
    }
    // Update is called once per frame
    void Update()
    {

    }

    protected override void UpdateUI()
    {
        PopulateList();
    }
}
