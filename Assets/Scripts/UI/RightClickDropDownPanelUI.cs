using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RightClickDropDownPanelUI : MonoBehaviour
{
    public static RightClickDropDownPanelUI Instance;

    [SerializeField]
    Transform MenuItemsContainer;

    [SerializeField]
    PortraitUI AgentPortrait;

    [SerializeField]
    TextMeshProUGUI AgentNameText;

    Transform CurrentTargetTransform;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(List<DescribedAction> MenuItems, Transform targetTransform, Character byCharacter = null)
    {
        if(byCharacter != null)
        {
            AgentPortrait.gameObject.SetActive(true);
            AgentNameText.gameObject.SetActive(true);

            AgentPortrait.SetCharacter(byCharacter);
            AgentNameText.text = byCharacter.name;
        }
        else
        {
            AgentPortrait.gameObject.SetActive(false);
            AgentNameText.gameObject.SetActive(false);
        }


        CurrentTargetTransform = targetTransform;

        this.gameObject.SetActive(true);

        ClearContainer();

        GameObject tempItem;
        for(int i=0;i<MenuItems.Count;i++)
        {
            tempItem = ResourcesLoader.Instance.GetRecycledObject(DEF.RIGHT_CLICK_DROPDOWN_ITEM);

            UnityAction[] actions = new UnityAction[] { MenuItems[i].Action, Hide };
            tempItem.GetComponent<RightClickMenuItemUI>().SetInfo(MenuItems[i].Key , actions, MenuItems[i].Description, MenuItems[i].Interactable);

            tempItem.transform.SetParent(MenuItemsContainer, false);

            tempItem.transform.localScale = Vector3.one;
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
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
