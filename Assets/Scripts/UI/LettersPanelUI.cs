﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LettersPanelUI : MonoBehaviour
{
    [SerializeField]
    GameObject LogLetterPrefab;

    [SerializeField]
    Transform LogContainer;

    [SerializeField]
    TextMeshProUGUI TitleText;

    [SerializeField]
    TextMeshProUGUI DescText;

    [SerializeField]
    TextMeshProUGUI SideNotesText;

    [SerializeField]
    TextMeshProUGUI FromText;

    [SerializeField]
    PortraitUI FromPortrait;

    [SerializeField]
    PortraitUI ToPortrait;

    [SerializeField]
    PortraitUI SubjectPortrait;

    LogLetterUI SelectedLetter;

    public static LettersPanelUI Instance;

    public List<LogLetterUI> ArchivedLetters = new List<LogLetterUI>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CORE.Instance.SubscribeToEvent("ShowLettersUI", Show);
        CORE.Instance.SubscribeToEvent("HideLettersUI", Hide);

        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        CORE.Instance.InvokeEvent("QuitLetters");
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Load(Letter[] letters)
    {
        for(int i=0;i<letters.Length;i++)
        {
            AddLetterToLog(letters[i]);
        }
    }

    public void AddLetterToLog(Letter letter)
    {
        GameObject tempObj = Instantiate(LogLetterPrefab);
        tempObj.transform.SetParent(LogContainer, false);

        LogLetterUI logLetter = tempObj.GetComponent<LogLetterUI>();

        logLetter.SetInfo(letter, this);

        ArchivedLetters.Add(logLetter);
    }

    public void RemoveSelectedLetter()
    {
        if(SelectedLetter == null)
        {
            return;
        }

        ArchivedLetters.Remove(SelectedLetter);
        Destroy(SelectedLetter.gameObject);
        SelectedLetter = null;

        TitleText.text = "Letter Removed...";
        DescText.text = "";
        FromText.text = "";
    }

    public void LetterSelected(LogLetterUI letter)
    {
        if(SelectedLetter != null)
        {
            SelectedLetter.Deselect();
        }

        SelectedLetter = letter;
        SelectedLetter.Select();

        TitleText.text = letter.CurrentLetter.Title;
        DescText.text = letter.CurrentLetter.Content;
        SideNotesText.text = letter.CurrentLetter.Preset.SideNotes;
        FromText.text = ((Character)letter.CurrentLetter.Parameters["Letter_From"]).name;
        FromPortrait.SetCharacter(((Character)letter.CurrentLetter.Parameters["Letter_From"]));
        ToPortrait.SetCharacter(((Character)letter.CurrentLetter.Parameters["Letter_To"]));

        if (letter.CurrentLetter.Parameters.ContainsKey("Letter_From"))
        {
            FromPortrait.gameObject.SetActive(true);
            FromPortrait.SetCharacter(((Character)letter.CurrentLetter.Parameters["Letter_From"]));
        }
        else
        {
            FromPortrait.gameObject.SetActive(false);
        }

        if (letter.CurrentLetter.Parameters.ContainsKey("Letter_To"))
        {
            ToPortrait.gameObject.SetActive(true);
            ToPortrait.SetCharacter(((Character)letter.CurrentLetter.Parameters["Letter_To"]));
        }
        else
        {
            ToPortrait.gameObject.SetActive(false);
        }

        if (letter.CurrentLetter.Parameters.ContainsKey("Letter_SubjectCharacter"))
        {
            SubjectPortrait.gameObject.SetActive(true);
            SubjectPortrait.SetCharacter(((Character)letter.CurrentLetter.Parameters["Letter_SubjectCharacter"]));
        }
        else
        {
            SubjectPortrait.gameObject.SetActive(false);
        }


    }
}

