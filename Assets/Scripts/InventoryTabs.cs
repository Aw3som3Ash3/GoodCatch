using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryTabs : TabbedMenu
{
    readonly string[] menus = new string[]{ "Consumables","Abilities","Misc" };
    public event Action<IEnumerable<object>> onSelectionChange;

    public new class UxmlFactory : UxmlFactory<InventoryTabs, UxmlTraits> { }
    public InventoryTabs()
    {


        for(int i = 0; i < menus.Length; i++)
        {
            MultiColumnListView listView = new();
            if (GameManager.Instance != null)
            {
                SetList(listView, GetListOfItemType(i));

            }
            else
            {
                var dummyList = new List<ItemInventory.ItemSlot>();
     
                SetList(listView, dummyList);
            }

            listView.selectionChanged += SelectionChanged;
            TabMenuButton menu = new TabMenuButton(menus[i], listView);
            menu.focusable = false;
            AddTab(menu,i==0? true:false);
        }
        
        void SelectionChanged(IEnumerable<object> enemmumerable)
        {
            onSelectionChange.Invoke(enemmumerable);
        }

        
    }
    
    List<ItemInventory.ItemSlot> GetListOfItemType(int index)
    {
        switch (index)
        {
            case 0:
                return GameManager.Instance.PlayerInventory.GetListOfItems<CombatItem>().ToList();
            case 1:
                return GameManager.Instance.PlayerInventory.GetListOfItems<AbilityTrainerItem>().ToList();
            case 2:
            default:
                return GameManager.Instance.PlayerInventory.GetListOfItems<Item>().ToList();


        }
       
    }

    protected override void OnChangedTab(VisualElement element)
    {
        base.OnChangedTab(element);
        if (element.Q<ListView>().childCount > 0)
        {
            element.Q<ListView>().SetSelection(0);
        }
       
    }
    void SetList(MultiColumnListView listView,List<ItemInventory.ItemSlot> playerInventory)
    {
        var nameColumn = new Column();
        nameColumn.title= "name";
        nameColumn.name = "name";
        //nameColumn.width = 50;
        nameColumn.sortable = true;
        nameColumn.stretchable = true;
        //nameColumn.width =
        listView.columns.Add(nameColumn);
        

        var amountColumn = new Column();
        amountColumn.title = "amount";
        amountColumn.name = "amount";
        
        //amountColumn.width = 50;
        amountColumn.sortable = true;
        amountColumn.width = 250;
        
        //amountColumn.stretchable = true;
        listView.columns.Add(amountColumn);
        listView.showBorder = true;
        listView.sortingEnabled = true;
        listView.columnSortingChanged += () =>
        {
            Debug.Log("amount or of sorted columns "+listView.sortedColumns.Count());
            
            foreach(var sortedColumns in listView.sortedColumns)
            {
                Debug.Log(sortedColumns.columnName);
                if (sortedColumns.columnName=="name")
                {
                    if (sortedColumns.direction == SortDirection.Ascending)
                    {
                        playerInventory= playerInventory.OrderBy((x) => x.Item.name).ToList();
                    }
                    else
                    {
                        playerInventory = playerInventory.OrderByDescending((x) => x.Item.name).ToList();
                    }

                }
                else
                if (sortedColumns.columnName == "amount")
                {
                    if (sortedColumns.direction == SortDirection.Ascending)
                    {
                        playerInventory =playerInventory.OrderBy((x) => x.amount).ToList();
                    }
                    else
                    {
                        playerInventory = playerInventory.OrderByDescending((x) => x.amount).ToList();
                    }
                   
                }
                listView.itemsSource = playerInventory;
            }
            listView.columns["name"].bindCell = (VisualElement element, int index) =>
           (element as Label).text = playerInventory[index].Item.name;

            listView.columns["amount"].bindCell = (VisualElement element, int index) =>
                (element as Label).text = playerInventory[index].amount.ToString();

        };
        

        listView.itemsSource = playerInventory;
        //var nameCell= new Label();
        //var amountCell= new Label();



        listView.columns["name"].makeCell = () =>
        {
            Label leftLabel = new Label();
            leftLabel.AddToClassList("left-column-text");
            return leftLabel;
        };
        listView.columns["amount"].makeCell = () =>
        {
            Label rightLabel = new Label();
            rightLabel.AddToClassList("right-column-text");
            return rightLabel;
        };
        listView.showAlternatingRowBackgrounds=AlternatingRowBackground.All;

        listView.columns["name"].bindCell = (VisualElement element, int index) =>
           (element as Label).text = playerInventory[index].Item.name;

        listView.columns["amount"].bindCell = (VisualElement element, int index) =>
            (element as Label).text = playerInventory[index].amount.ToString();

        // Set a fixed item height matching the height of the item provided in makeItem. 
        // For dynamic height, see the virtualizationMethod property.
        listView.fixedItemHeight = 45;

       
    }

    
}
