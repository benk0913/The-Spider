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

    public int SequenceIndex { private set; get; }

    public DialogPiece CurrentPiece
    {
        get
        {
            return DialogSequence[SequenceIndex];
        }
    }


    #endregion



    #region Private Parameters

    private List<DialogPiece> DialogSequence = new List<DialogPiece>();

    private Dictionary<string, object> DialogParameters = new Dictionary<string, object>();

    [SerializeField]
    Transform DecisionContainer;

    [SerializeField]
    TextMeshProUGUI Description;

    [SerializeField]
    Image SceneImage;

    [SerializeField]
    PortraitUI ActorPortrait;

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
    }


    #endregion



    #region Public Methods

    //THE method to start dialogs with...
    public void StartNewDialog(List<DialogPiece> dialogPieces, Dictionary<string, object> parameters)
    {
        WipeCurrentDialog();
        SetDialogParameters(parameters);
        AddToDialog(dialogPieces);

        ShowDialogPiece(0);
    }

    public void AddToDialog(DialogPiece piece)
    {
        DialogSequence.Add(piece);
    }

    public void AddToDialog(List<DialogPiece> pieces)
    {
        if (DialogSequence.Count == 0)
        {
            DialogSequence.InsertRange(0, pieces);
        }
        else
        {
            DialogSequence.InsertRange(DialogSequence.Count - 1, pieces);
        }
    }

    public void InsertNextPiece(DialogPiece piece)
    {
        DialogSequence.Insert(SequenceIndex+1, piece);
    }

    public void InsertNextPiece(List<DialogPiece> pieces)
    {
        DialogSequence.InsertRange(SequenceIndex+1, pieces);
    }


    public void SetDialogParameters(Dictionary<string, object> parameters)
    {
        DialogParameters = parameters;
    }

    public object GetDialogParameter(string key)
    {
        if(!DialogParameters.ContainsKey(key))
        {
            Debug.LogError("DialogWindow - NO PARAMETER WITH KEY: " + key);
            return null;
        }

        return DialogParameters[key];
    }

    public void WipeCurrentDialog()
    {
        DialogSequence.Clear();
        SequenceIndex = 0;
        if(IsShowingDialog)
        {
            HideCurrentDialog();
        }
    }

    public void ShowCurrentDialog()
    {
        IsShowingDialog = true;
        this.gameObject.SetActive(true);

        RefreshUI();
    }

    public void HideCurrentDialog()
    {
        IsShowingDialog = false;
        this.gameObject.SetActive(false);
    }

    public void ShowDialogPiece(int index)
    {
        if(!IsShowingDialog)
        {
            ShowCurrentDialog();
        }

        SequenceIndex = index;

        RefreshUI();
    }

    public void ShowNextDialogPiece()
    {
        if(SequenceIndex + 1 > DialogSequence.Count)
        {
            HideCurrentDialog();
            return;
        }

        ShowDialogPiece(SequenceIndex + 1);
    }

    #endregion
}
