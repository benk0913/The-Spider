using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogWindowUI : MonoBehaviour
{
    #region Public Parameters 

    public static DialogWindowUI Instance { private set; get; }

    public bool IsShowingDialog { private set; get; }

    public int SequenceIndex { private set; get; }

    #endregion



    #region Private Parameters

    private List<DialogPiece> DialogSequence = new List<DialogPiece>();

    private Dictionary<string, object> DialogParameters = new Dictionary<string, object>();

    #endregion



    #region Private Methods

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
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
        DialogSequence.InsertRange(DialogSequence.Count-1, pieces);
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

        if(IsShowingDialog)
        {
            HideCurrentDialog();
        }
    }

    public void ShowCurrentDialog()
    {
        IsShowingDialog = true;
        this.gameObject.SetActive(true);
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
