using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RightSelectCharacterPanelUI : MonoBehaviour
{
    public static RightSelectCharacterPanelUI Instance;

    [SerializeField]
    Transform MenuItemsContainer;

    [SerializeField]
    CircleLayoutUI CircleLayout;

    Transform CurrentTargetTransform;

    AgentAction RelevantAction;
    AgentInteractable RelevantTarget;

    List<Character> RelevantCharacters = new List<Character>();

    Predicate<Character> CurrentFilter;
    Action<Character> CurrentOnSelect;
    UnityAction CurrentFallbackAction;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if(CORE.Instance != null)
            CORE.Instance.UnoccupyFocusView(this);
    }



    public void Show(Action<Character> onSelect, Predicate<Character> filter, string title = "Select Agent:", AgentAction agentAction = null, AgentInteractable relevantTarget = null, UnityAction fallbackAction = null)
    {
       

        CORE.Instance.DelayedInvokation(0.01f, () => {
            CORE.Instance.UnsubscribeFromEvent("PassTimeComplete", OnTurnPassed);
            CORE.Instance.SubscribeToEvent("PassTimeComplete", OnTurnPassed);

            CurrentFilter = filter;
            CurrentOnSelect = onSelect;
            CurrentFallbackAction = fallbackAction;
            CurrentTargetTransform = relevantTarget.transform;

            RelevantAction = agentAction;
            RelevantTarget = relevantTarget;

            this.gameObject.SetActive(true);

            CORE.Instance.OccupyFocusView(this);

            //TitleText.text = title;

            RefreshUI();
        });
    }

    void RefreshUI()
    {
        ClearContainer();

        RelevantCharacters.Clear();
        RelevantCharacters.AddRange(CORE.Instance.Characters.FindAll(CurrentFilter));
        if (RelevantCharacters.Count == 0)
        {
            GlobalMessagePrompterUI.Instance.Show("No Available Agents...", 1f, Color.red);
            Hide();
            return;
        }

        GameObject tempItem;
        foreach (Character character in RelevantCharacters)
        {

            tempItem = ResourcesLoader.Instance.GetRecycledObject("SelectablePortraitUI");


            Transform portraitChild = tempItem.transform.GetChild(0);
            portraitChild.GetComponent<PortraitUI>().SetCharacter(character);

            tempItem.transform.SetParent(MenuItemsContainer, false);

            tempItem.transform.localScale = Vector3.one;

            FailReason potentialReason = null;

            Button tempButton = tempItem.GetComponent<Button>();
            if (character  == CORE.PC)
            {
                portraitChild.GetComponent<CanvasGroup>().alpha = 0.5f;

                tempButton.onClick.AddListener(() =>
                {
                    GlobalMessagePrompterUI.Instance.Show(CORE.PC.name + " doesn't take part in tasks such as this.", 1f, Color.red);
                });
            }
            else if (!character.IsAgent)
            {
                portraitChild.GetComponent<CanvasGroup>().alpha = 0.5f;

                tempButton.onClick.AddListener(() =>
                {
                    GlobalMessagePrompterUI.Instance.Show("This character is not an agent.", 1f, Color.red);
                });
            }
            else if (RelevantAction != null && !RelevantAction.CanDoAction(character.TopEmployer, character, RelevantTarget, out potentialReason))
            {
                portraitChild.GetComponent<CanvasGroup>().alpha = 0.5f;

                tempButton.onClick.AddListener(() =>
                {
                    GlobalMessagePrompterUI.Instance.Show(character.name + " can not do this" + (potentialReason != null ? (", " + potentialReason.Key) : "."), 1f, Color.red);
                });
            }
            else
            {
                portraitChild.GetComponent<CanvasGroup>().alpha = 1f;

                tempButton.onClick.RemoveAllListeners();
                tempButton.onClick.AddListener(() =>
                {
                    CurrentOnSelect(character);
                    Hide();
                });
            }
        }

        RightClickMenuItemUI showAll = ResourcesLoader.Instance.GetRecycledObject("RightClickMenuItem").GetComponent<RightClickMenuItemUI>();
        showAll.transform.SetParent(MenuItemsContainer, false);

        showAll.transform.localScale = Vector3.one;


        UnityAction[] actions = new UnityAction[]
        {
                Hide,
                CurrentFallbackAction
        };

        showAll.GetComponent<RightClickMenuItemUI>().SetInfo("Show All", actions, "Will display all of the available agents.", ResourcesLoader.Instance.GetSprite("connections"), true, null);

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
        Hide();
    }

    void Update()
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

        if(Input.GetMouseButtonDown(0) && this.gameObject.activeInHierarchy && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Hide();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
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
