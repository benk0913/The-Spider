using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindowUI : MonoBehaviour
{
    #region Public Parameters 

    public static DialogWindowUI Instance { private set; get; }

    public bool IsShowingDialog { private set; get; }

    public DialogPiece CurrentPiece;

    public DialogPiece LastLobbyPiece;

    #endregion



    #region Private Parameters

    private Dictionary<string, object> DialogParameters = new Dictionary<string, object>();

    [SerializeField]
    Transform DecisionContainer;

    [SerializeField]
    TextMeshProUGUI Description;

    [SerializeField]
    Image SceneImage;

    [SerializeField]
    PortraitUI ActorPortrait;

    [SerializeField]
    Transform TargetPortraitsContainer;

    [SerializeField]
    GameObject ItemsLootedPanel;

    [SerializeField]
    Transform ItemsLootedContainer;

    #endregion



    #region Private Methods

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void ClearDecisionContainer()
    {
        while(DecisionContainer.childCount > 0)
        {
            DecisionContainer.GetChild(0).gameObject.SetActive(false);
            DecisionContainer.GetChild(0).SetParent(transform);
        }
    }

    void RefreshUI()
    {
        Description.text = Util.FormatTags(CurrentPiece.Description, DialogParameters);


        SceneImage.sprite = CurrentPiece.Image;

        Character actor = (Character)GetDialogParameter("Actor");

        if (actor == null)
        {
            ActorPortrait.gameObject.SetActive(false);
        }
        else
        {
            ActorPortrait.gameObject.SetActive(true);
            ActorPortrait.SetCharacter(actor);
        }

        if(CurrentPiece.TargetCharacters != null && CurrentPiece.TargetCharacters.Length > 0)
        {
            TargetPortraitsContainer.gameObject.SetActive(true);

            while(TargetPortraitsContainer.childCount > 0)
            {
                TargetPortraitsContainer.GetChild(0).gameObject.SetActive(false);
                TargetPortraitsContainer.GetChild(0).SetParent(transform);
            }

            foreach (Character targetCharacter in CurrentPiece.TargetCharacters)
            {
                PortraitUI portrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI").GetComponent<PortraitUI>();
                portrait.transform.SetParent(TargetPortraitsContainer, false);
                portrait.transform.localScale = Vector3.one;
                portrait.transform.position = Vector3.zero;
                portrait.SetCharacter(targetCharacter);
            }
        }
        else
        {
            TargetPortraitsContainer.gameObject.SetActive(false);
        }

        ClearDecisionContainer();

        bool skipDecision = false;
        foreach(DialogDecision decision in CurrentPiece.Decisions)
        {
            foreach(DialogDecisionCondition condition in decision.AppearanceConditions)
            {
                if(!condition.CheckCondition())
                {
                    skipDecision = true;
                    break;
                }
            }
            
            if(skipDecision)
            {
                skipDecision = false;
                continue;
            }

            DialogDecisionItemUI tempDecision = ResourcesLoader.Instance.GetRecycledObject("DecisionItemUI").GetComponent<DialogDecisionItemUI>();
            tempDecision.transform.SetParent(DecisionContainer, false);
            tempDecision.SetInfo(decision);

        }

        if(!string.IsNullOrEmpty(CurrentPiece.SimpleDecision.Title))
        {
            DialogDecisionItemUI tempDecision = ResourcesLoader.Instance.GetRecycledObject("DecisionItemUI").GetComponent<DialogDecisionItemUI>();
            tempDecision.transform.SetParent(DecisionContainer, false);

            DialogDecision tempdc = CORE.Instance.Database.SampleDecision.Clone();
            tempdc.name = CurrentPiece.SimpleDecision.Title;
            tempdc.Icon = CurrentPiece.SimpleDecision.Icon;
            tempdc.NextPiece = CurrentPiece.SimpleDecision.NextPiece;
            if (CurrentPiece.SimpleDecision.Action != null)
            {
                tempdc.Actions.Add(CurrentPiece.SimpleDecision.Action);
            }
            tempDecision.SetInfo(tempdc);
        }
    }


    #endregion



    #region Public Methods

    //THE method to start dialogs with...
    public void StartNewDialog(DialogPiece piece, Dictionary<string, object> parameters)
    {
        SetDialogParameters(parameters);
        ShowDialogPiece(piece);
    }


    public void SetDialogParameters(Dictionary<string, object> parameters)
    {
        DialogParameters = parameters;
    }

    public object GetDialogParameter(string key)
    {
        if(!DialogParameters.ContainsKey(key))
        {
            return null;
        }

        return DialogParameters[key];
    }

    public void SetDialogParameter(string key, object value)
    {
        if(DialogParameters.ContainsKey(key))
        {
            DialogParameters[key] = value;
        }
        else
        {
            DialogParameters.Add(key, value);
        }
    }

    public void ShowCurrentDialog()
    {
        CORE.Instance.FocusViewLocked = true;
        IsShowingDialog = true;
        this.gameObject.SetActive(true);

        RefreshUI();
    }

    public void HideCurrentDialog()
    {
        CORE.Instance.FocusViewLocked = false;
        CurrentPiece = null;
        IsShowingDialog = false;
        this.gameObject.SetActive(false);

        CORE.Instance.InvokeEvent("DialogClosed");
    }

    public void ShowDialogPiece(DialogPiece piece)
    {
        CurrentPiece = piece;

        if (!IsShowingDialog)
        {
            ShowCurrentDialog();
        }

        if(CurrentPiece.LobbyPiece)
        {
            LastLobbyPiece = CurrentPiece;
        }

        RefreshUI();
    }

    public void ShowItemsLooted(Item[] items)
    {
        ItemsLootedPanel.gameObject.SetActive(true);

        while(ItemsLootedContainer.childCount > 0)
        {
            ItemsLootedContainer.GetChild(0).gameObject.SetActive(false);
            ItemsLootedContainer.GetChild(0).SetParent(transform);
        }

        foreach(Item item in items)
        {
            GameObject rumorPanel = ResourcesLoader.Instance.GetRecycledObject("ItemUI");
            rumorPanel.transform.SetParent(ItemsLootedContainer, false);
            rumorPanel.GetComponent<ItemUI>().SetInfo(item);
        }
    }

    #endregion
}
