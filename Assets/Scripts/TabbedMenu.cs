
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TabbedMenu : VisualElement
{
    public new class UxmlFactory : UxmlFactory<TabMenuButton, UxmlTraits> { }
   
    private const string k_styleName = "TabbedView";
    private const string s_UssClassName = "unity-tabbed-view";
    private const string s_ContentContainerClassName = "unity-tabbed-view__content-container";
    private const string s_TabsContainerClassName = "unity-tabbed-view__tabs-container";

    private readonly VisualElement m_TabContent;
    private readonly VisualElement m_Content;
    private VisualElement changeTabRight, changeTabLeft;

    private readonly List<TabMenuButton> m_Tabs = new List<TabMenuButton>();
    private TabMenuButton m_ActiveTab;

    public override VisualElement contentContainer => m_Content;
    int index;
    public TabbedMenu()
    {
        AddToClassList(s_UssClassName);
        styleSheets.Add(Resources.Load<StyleSheet>("UXMLs/TabbedView"));

        var tabParent = new VisualElement();
        tabParent.name = "unity-topbar-container";
        hierarchy.Add(tabParent);
        m_TabContent = new VisualElement();
        m_TabContent.name = "unity-tabs-container";
        m_TabContent.AddToClassList(s_TabsContainerClassName);
       


        changeTabLeft = new();
        changeTabLeft.name = "changeTabLeftIcon";
        changeTabLeft.style.width = 60;
        changeTabLeft.style.height = 60;
        changeTabLeft.AddToClassList("ChangeTabIcons");


        changeTabRight = new();
        changeTabRight.AddToClassList("ChangeTabIcons");
        changeTabRight.name = "changeTabRightIcon";        
        changeTabRight.style.width = 60;
        changeTabRight.style.height = 60;


        tabParent.Add(changeTabLeft);
        tabParent.Add(m_TabContent);
        tabParent.Add(changeTabRight);
        tabParent.style.flexDirection = FlexDirection.Row;

        m_Content = new VisualElement();
        m_Content.name = "unity-content-container";
        m_Content.AddToClassList(s_ContentContainerClassName);
        hierarchy.Add(m_Content);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnInputChange += ChangeTabIcons;
            ChangeTabIcons( GameManager.Instance.inputMethod);
        }
        else
        {
            ChangeTabIcons(InputMethod.mouseAndKeyboard);
        }
        RegisterCallback<AttachToPanelEvent>(ProcessEvent);
       
    }

    public void AddTab(TabMenuButton tabButton, bool activate)
    {
        m_Tabs.Add(tabButton);
        m_TabContent.Add(tabButton);
        //changeTabRight.BringToFront();
        tabButton.OnClose += RemoveTab;
        tabButton.OnSelect += Activate;
        
        if (activate)
        {
            Activate(tabButton);
        }
    }

    public void RemoveTab(TabMenuButton tabButton)
    {
        int index = m_Tabs.IndexOf(tabButton);

        // If this tab is the active one make sure we deselect it first...
        if (m_ActiveTab == tabButton)
        {
            DeselectTab(tabButton);
            m_ActiveTab = null;
        }

        m_Tabs.RemoveAt(index);
        m_TabContent.Remove(tabButton);

        tabButton.OnClose -= RemoveTab;
        tabButton.OnSelect -= Activate;

        // If we closed the active tab AND we have any tabs left - active the next valid one...
        if ((m_ActiveTab == null) && m_Tabs.Any())
        {
            int clampedIndex = Mathf.Clamp(index, 0, m_Tabs.Count - 1);
            TabMenuButton tabToActivate = m_Tabs[clampedIndex];

            Activate(tabToActivate);
        }
    }

    private void ProcessEvent(AttachToPanelEvent e)
    {
        // This code takes any existing tab buttons and hooks them into the system...
        for (int i = 0; i < m_Content.childCount; ++i)
        {
            VisualElement element = m_Content[i];

            if (element is TabMenuButton button)
            {
                m_Content.Remove(element);

                if (button.Target == null)
                {
                    string targetId = button.TargetId;

                    button.Target = this.Q(targetId);
                }
                AddTab(button, false);
                --i;
            }
            else
            {
                element.style.display = DisplayStyle.None;
            }
        }

        // Finally, if we need to, activate this tab...
        if (m_ActiveTab != null)
        {
            SelectTab(m_ActiveTab);
        }
        else if (m_Tabs.Count > 0)
        {
            m_ActiveTab = m_Tabs[0];

            SelectTab(m_ActiveTab);
        }
    }

    private void SelectTab(TabMenuButton tabButton)
    {
        VisualElement target = tabButton.Target;

        tabButton.Select();
        if (target != null)
            Add(target);
    }

    private void DeselectTab(TabMenuButton tabButton)
    {
        VisualElement target = tabButton.Target;

        if (target != null)
            Remove(target);
        tabButton.Deselect();
    }

    public void Activate(TabMenuButton button)
    {
        if (m_ActiveTab != null)
        {
            DeselectTab(m_ActiveTab);
        }

        m_ActiveTab = button;
        SelectTab(m_ActiveTab);
    }
    public virtual void ChangeTab(int delta)
    {
        int targetIndex = Mathf.Clamp(index + delta, 0, 2);
        if (index == targetIndex)
        {
            return;
        }
        index = targetIndex;
        Activate(m_Tabs[index]);

        if (m_Tabs[index].Target.childCount <= 0)
        {
            return;
        }
       
    }


    protected virtual void OnChangedTab(VisualElement element)
    {
        
    }
    void ChangeTabIcons(InputMethod inputMethod)
    {
        foreach (var binding in InputManager.Input.UI.ChangeTab.bindings.Where((x) => x.groups == (inputMethod == InputMethod.mouseAndKeyboard ? "Keyboard&Mouse" : "Gamepad")))
        {
            if (binding.isPartOfComposite)
            {
                if (binding.name == "positive")
                {
                    InputDisplayer.GetInputIcon(binding, inputMethod).Completed += (x) => changeTabRight.style.backgroundImage = x.Result;


                    Debug.Log("has binding " + binding.effectivePath);

                }
                else if (binding.name == "negative")
                {
                    InputDisplayer.GetInputIcon(binding, inputMethod).Completed += (x) => changeTabLeft.style.backgroundImage = x.Result;
                }
            }




        }

    }


}


public class TabMenuButton : VisualElement
{
    internal new class UxmlFactory : UxmlFactory<TabMenuButton, UxmlTraits> { }

    internal new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription { name = "text" };
        private readonly UxmlStringAttributeDescription m_Target = new UxmlStringAttributeDescription { name = "target" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            TabMenuButton item = ve as TabMenuButton;

            item.m_Label.text = m_Text.GetValueFromBag(bag, cc);
            item.TargetId = m_Target.GetValueFromBag(bag, cc);
        }
    }

    static readonly string styleName = "TabButtonStyles";
    static readonly string UxmlName = "TabButton";
    static readonly string s_UssClassName = "unity-tab-button";
    static readonly string s_UssActiveClassName = s_UssClassName + "--active";

    private Label m_Label;

    public bool IsCloseable { get; set; }
    public string TargetId { get; private set; }
    public VisualElement Target { get; set; }

    public event Action<TabMenuButton> OnSelect;
    public event Action<TabMenuButton> OnClose;

    public TabMenuButton()
    {
        Init();
    }

    public TabMenuButton(string text, VisualElement target)
    {
        Init();
        m_Label.text = text;
        Target = target;
    }

    private void PopulateContextMenu(ContextualMenuPopulateEvent populateEvent)
    {
        DropdownMenu dropdownMenu = populateEvent.menu;

        if (IsCloseable)
        {
            dropdownMenu.AppendAction("Close Tab", e => OnClose(this));
        }
    }

    private void CreateContextMenu(VisualElement visualElement)
    {
        ContextualMenuManipulator menuManipulator = new ContextualMenuManipulator(PopulateContextMenu);

        visualElement.focusable = false;
        visualElement.pickingMode = PickingMode.Position;
        visualElement.AddManipulator(menuManipulator);

        visualElement.AddManipulator(menuManipulator);
    }

    private void Init()
    {
        AddToClassList(s_UssClassName);
        styleSheets.Add(Resources.Load<StyleSheet>("UXMLs/TabbedButtons"));

        VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UXMLs/TabButton");
        visualTree.CloneTree(this);

        m_Label = this.Q<Label>("Label");

        CreateContextMenu(this);

        RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
    }

    public void Select()
    {
        AddToClassList(s_UssActiveClassName);

        if (Target != null)
        {
            Target.style.display = DisplayStyle.Flex;
            Target.style.flexGrow = 1;
        }
    }

    public void Deselect()
    {
        RemoveFromClassList(s_UssActiveClassName);
        MarkDirtyRepaint();

        if (Target != null)
        {
            Target.style.display = DisplayStyle.None;
            Target.style.flexGrow = 0;
        }
    }

    private void OnMouseDownEvent(MouseDownEvent e)
    {
        switch (e.button)
        {
            case 0:
                {
                    OnSelect?.Invoke(this);
                    break;
                }

            case 2 when IsCloseable:
                {
                    OnClose?.Invoke(this);
                    break;
                }
        }        // End of switch.

        e.StopImmediatePropagation();
    }
}
