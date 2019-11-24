using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RightClickDropDownPanelUI : MonoBehaviour
{
    public static RightClickDropDownPanelUI Instance;

    [SerializeField]
    Transform MenuItemsContainer;

    [SerializeField]
    PortraitUI AgentPortrait;

    [SerializeField]
    TextMeshProUGUI AgentNameText;

    [SerializeField]
    CircleLayoutUI CircleLayout;

    Transform CurrentTargetTransform;

    List<DescribedAction> CurrentMenuItems;

    Character CurrentByCharacter;

    AgentInteractable CurrentSource;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(List<DescribedAction> MenuItems, Transform targetTransform, Character byCharacter = null, AgentInteractable source = null)
    {
        Hide();

        CORE.Instance.SubscribeToEvent("PassTimeComplete", OnTurnPassed);

        this.gameObject.SetActive(true);

        CurrentSource = source;
        CurrentMenuItems = MenuItems;
        CurrentTargetTransform = targetTransform;
        CurrentByCharacter = byCharacter;

        RefreshUI();
    }

    void RefreshUI()
    {
        if (CurrentByCharacter != null)
        {
            AgentPortrait.gameObject.SetActive(true);
            AgentNameText.gameObject.SetActive(true);

            AgentPortrait.SetCharacter(CurrentByCharacter);
            AgentNameText.text = CurrentByCharacter.name;
        }
        else
        {
            AgentPortrait.gameObject.SetActive(false);
            AgentNameText.gameObject.SetActive(false);
        }

        ClearContainer();

        //if (Input.mousePosition.y > Screen.height / 2)
        //{
        //    MenuItemsContainer.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
        //    MenuItemsContainer.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
        //    MenuItemsContainer.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
        //    MenuItemsContainer.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
        //    MenuItemsContainer.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
        //}
        //else
        //{
        //    MenuItemsContainer.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.LowerRight;
        //    MenuItemsContainer.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.LowerRight;
        //    MenuItemsContainer.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 0f);
        //    MenuItemsContainer.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 0f);
        //    MenuItemsContainer.GetComponent<RectTransform>().pivot = new Vector2(1f, 0f);
        //}

        GameObject tempItem;
        for (int i = 0; i < CurrentMenuItems.Count; i++)
        {
            tempItem = ResourcesLoader.Instance.GetRecycledObject("RightClickMenuItem");

            UnityAction[] actions = new UnityAction[] { CurrentMenuItems[i].Action, Hide };
            tempItem.GetComponent<RightClickMenuItemUI>().SetInfo(CurrentMenuItems[i].Key, actions, CurrentMenuItems[i].Description, CurrentMenuItems[i].Icon, CurrentMenuItems[i].Interactable, CurrentMenuItems[i].TooltipBonuses);

            tempItem.transform.SetParent(MenuItemsContainer, false);

            tempItem.transform.localScale = Vector3.one;
        }

        CircleLayout.RefreshLayout();

        transform.SetAsLastSibling();
    }

    public void Hide()
    {
        CORE.Instance.UnsubscribeFromEvent("PassTimeComplete", OnTurnPassed);
        this.gameObject.SetActive(false);
    }

    void OnTurnPassed()
    {
        if(CurrentSource == null)
        {
            return;
        }

        CurrentSource.ShowActionMenu();
    }

    private void Update()
    {
        if (CurrentTargetTransform != null && Camera.current != null)
        {
            if (CurrentTargetTransform.GetType() == typeof(RectTransform))
            {
                transform.position = CurrentTargetTransform.position;
            }
            else
            {
                transform.position = Camera.current.WorldToScreenPoint(CurrentTargetTransform.position);
            }
        }
    }

    void ClearContainer()
    {
        while(MenuItemsContainer.childCount > 0)
        {
            MenuItemsContainer.GetChild(0).gameObject.SetActive(false);
            MenuItemsContainer.GetChild(0).SetParent(transform);
        }
    }

    
}
