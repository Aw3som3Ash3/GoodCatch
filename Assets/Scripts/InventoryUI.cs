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
        var itemList = GameManager.Instance.PlayerInventory.GetListOfItems<Item>();
        var list = OrderedList(itemList.ToList(), orderBy);

        for (int i = 0; i < (list.Count > contentZone.childCount ? list.Count : contentZone.childCount); i++)
        {
            if (i < contentZone.childCount)
            {
                contentZone.GetChild(i).GetComponent<InventoryUIItem>().SetValues(list[i].Item.name, list[i].Item.Type, list[i].amount);
                if (i > list.Count)
                {
                    Destroy(contentZone.GetChild(i));
                }
            }
            else
            {
                var obj = Instantiate(inventoryItemPrefab, contentZone);
                obj.GetComponent<InventoryUIItem>().SetValues(list[i].Item.name, list[i].Item.Type, list[i].amount);
            }

        }
    }
    List<ItemInventory.ItemSlot> OrderedList(IList<ItemInventory.ItemSlot> list, OrderBy orderBy)
    {

        if (orderBy == OrderBy.Name)
        {
            return list.OrderBy(x => x.Item.name).ToList();
        }
        else if (orderBy == OrderBy.Type)
        {
            return list.OrderBy(x => x.Item.GetType().Name).ThenBy(x => x.Item.name).ToList();
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
