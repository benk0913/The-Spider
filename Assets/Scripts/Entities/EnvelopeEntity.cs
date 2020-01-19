using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EnvelopeEntity : MonoBehaviour
{
    [SerializeField]
    LetterPreset PresetLetter;

    [SerializeField]
    Letter CurrentLetter;

    [SerializeField]
    TextMeshProUGUI TitleText;

    [SerializeField]
    TextMeshProUGUI DescriptionText;

    [SerializeField]
    TextMeshProUGUI FromText;

    [SerializeField]
    TextMeshProUGUI ArchiveActionText;

    [SerializeField]
    TextMeshProUGUI DeleteActionText;

    [SerializeField]
    TextMeshProUGUI RetreiveActionText;

    [SerializeField]
    MeshRenderer SealRenderer;

    [SerializeField]
    public PortraitUI FromPortrait;

    [SerializeField]
    public PortraitUI ToPortrait;

    [SerializeField]
    public PortraitUI SubjectPortrait;

    [SerializeField]
    TextMeshProUGUI SideNotesText;

    [SerializeField]
    GameObject QuestPanel;

    [SerializeField]
    GameObject DecipherPanel;

    [SerializeField]
    GameObject DecipherModePanel;

    [SerializeField]
    TextMeshProUGUI QuestName;

    [SerializeField]
    TextMeshProUGUI QuestFirstObjective;

    [SerializeField]
    TextMeshProUGUI QuestAcceptText1;

    [SerializeField]
    TextMeshProUGUI QuestAcceptText2;

    UnityAction<EnvelopeEntity> DisposeAction;

    #region Deciphering

    bool IsDeciphered = false;
    bool IsInDecipherMode = false;

    char DCurrentCharacter;
    int DCurrentIndex;

    [SerializeField]
    TextMeshProUGUI DCurrentLetterText;

    [SerializeField]
    TextMeshProUGUI DTargetLetterText;

    [SerializeField]
    TextMeshProUGUI DOriginalText;

    string decipherContext;

    public int[] GetOccurences(char letter, string inString)
    {
        List<int> occurences = new List<int>();
        for(int i=0;i<inString.Length;i++)
        {
            if(letter == inString[i])
            {
                occurences.Add(i);
            }
        }

        return occurences.ToArray();
    }

    public string GetHighlightedCharacterString(char letter, string inString)
    {
        string startTag = "<mark=#FF8000>";
        string endTag = "</mark>";

        for (int i = 0; i < inString.Length; i++)
        {
            if (letter == inString[i])
            {
                inString = inString.Insert(i, startTag);
                i += startTag.Length;

                if (i + 1 >= inString.Length)
                {
                    inString += endTag;
                }
                else
                {
                    inString = inString.Insert(i + 1, endTag);
                }

                i += endTag.Length;
            }
        }

        return inString;
    }

    public void StartDeciphering()
    {
        DecipherModePanel.gameObject.SetActive(true);
        DecipherPanel.SetActive(false);
        QuestPanel.gameObject.SetActive(false);
        DeleteActionText.text = "Press 'R' To RESTART";
        DOriginalText.text = CurrentLetter.Content;
        decipherContext = DOriginalText.text;

        IsInDecipherMode = true;

        DCurrentIndex = 0;
        SelectLetter(decipherContext[DCurrentIndex]);
    }

    public void StopDeciphering()
    {
        IsInDecipherMode = false;
        RefreshUI();
    }

    void SelectLetter(char letter)
    {
        DescriptionText.text = GetHighlightedCharacterString(letter, decipherContext);
        DCurrentLetterText.text = letter.ToString();

    }

    #endregion


    private void Start()
    {
        if(CurrentLetter != null)
        {
            RefreshUI();
        }
        else if(PresetLetter != null)
        {
            CurrentLetter = new Letter(PresetLetter, null);
            RefreshUI();
        }
    }

    private void Update()
    {
        if (IsInDecipherMode)
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                DCurrentIndex--;

                if(DCurrentIndex < 0)
                {
                    DCurrentIndex = decipherContext.Length-1;
                }

                SelectLetter(decipherContext[DCurrentIndex]);
                
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                DCurrentIndex++;

                if (DCurrentIndex >= decipherContext.Length)
                {
                    DCurrentIndex = 0;
                }

                SelectLetter(decipherContext[DCurrentIndex]);
            }
        }
    }

    public void SetInfo(Letter letter, UnityAction<EnvelopeEntity> disposeAction)
    {
        CurrentLetter = letter;
        DisposeAction = disposeAction;

        RefreshUI();


    }

    void RefreshUI()
    {
        if(CurrentLetter.Preset.Seal != null)
        {
            SealRenderer.gameObject.SetActive(true);
            SealRenderer.material = CurrentLetter.Preset.Seal;
        }
        else
        {
            SealRenderer.gameObject.SetActive(false);
        }

        TitleText.text = CurrentLetter.Title;
        DescriptionText.text = CurrentLetter.Content;
        SideNotesText.text = CurrentLetter.Preset.SideNotes;

        if (CurrentLetter.Parameters != null)
        {
            FromPortrait.gameObject.SetActive(true);
            ToPortrait.gameObject.SetActive(true);

            FromText.text = ((Character)CurrentLetter.Parameters["Letter_From"]).name;
            FromPortrait.SetCharacter(((Character)CurrentLetter.Parameters["Letter_From"]));
            ToPortrait.SetCharacter(((Character)CurrentLetter.Parameters["Letter_To"]));

            if (CurrentLetter.Parameters.ContainsKey("Letter_SubjectCharacter"))
            {
                SubjectPortrait.gameObject.SetActive(true);
                SubjectPortrait.SetCharacter(((Character)CurrentLetter.Parameters["Letter_SubjectCharacter"]));
            }
            else
            {
                SubjectPortrait.gameObject.SetActive(false);
            }
        }
        else
        {
            FromText.text = CurrentLetter.Preset.From;
            FromPortrait.gameObject.SetActive(false);
            SubjectPortrait.gameObject.SetActive(false);
            ToPortrait.gameObject.SetActive(false);
        }

        if (CurrentLetter.Preset.Encryption != null && !IsDeciphered)
        {
            DecipherPanel.SetActive(true);
            QuestPanel.gameObject.SetActive(false);
            DecipherModePanel.gameObject.SetActive(false);

            ArchiveActionText.text = "<color=red>CAN NOT ARCHIVE ENCRYPTED LETTERS</color>";
            DeleteActionText.text = "";
        }
        else
        {
            DecipherPanel.SetActive(false);

            if (CurrentLetter.Preset.QuestAttachment != null && !QuestsPanelUI.Instance.HasQuest(CurrentLetter.Preset.QuestAttachment, CORE.PC))
            {
                QuestPanel.gameObject.SetActive(true);
                QuestName.text = CurrentLetter.Preset.QuestAttachment.name;

                QuestAcceptText1.text = "PRESS '" + InputMap.Map["Accept Interaction"].ToString() + "' TO ACCEPT";
                QuestAcceptText2.text = "PRESS '" + InputMap.Map["Accept Interaction"].ToString() + "' TO ACCEPT";

                if (CurrentLetter.Preset.QuestAttachment.Objectives.Length > 0)
                {
                    QuestFirstObjective.text = CurrentLetter.Preset.QuestAttachment.Objectives[0].name;
                }
                else
                {
                    QuestFirstObjective.text = "";
                }
            }
            else
            {
                QuestPanel.gameObject.SetActive(false);
            }

            ArchiveActionText.text = "Press '" + InputMap.Map["Secondary Interaction"].ToString() + "' to ARCHIVE.";
        }

        DeleteActionText.text = "";
        RetreiveActionText.text = "Press 'Escape' to return...";
    }

    public void Archive()
    {
        if(this.CurrentLetter.Preset.Encryption != null && !IsDeciphered)
        {
            return;
        }

        LettersPanelUI.Instance.AddLetterToLog(this.CurrentLetter);

        DisposeAction?.Invoke(this);

        Destroy(this.gameObject);
    }

    public void Delete()
    {
        if(IsInDecipherMode)
        {
            DescriptionText.text = DOriginalText.text;
            return;
        }

        if (this.CurrentLetter.Preset.Encryption != null && !IsDeciphered)
        {
            return;
        }

        DisposeAction(this);
        Destroy(this.gameObject);
    }

    public void AcceptQuest()
    {
        if(IsInDecipherMode)
        {
            return;
        }

        if(CurrentLetter.Preset.Encryption != null & !IsDeciphered)
        {
            StartDeciphering();
            return;
        }

        if (CurrentLetter.Preset.QuestAttachment == null)
        {
            return;
        }
        
        if(QuestsPanelUI.Instance.HasQuest(CurrentLetter.Preset.QuestAttachment, CORE.PC))
        {
            return;
        }


        Quest questClone = CurrentLetter.Preset.QuestAttachment.CreateClone();
        questClone.ForCharacter = CORE.PC;
        QuestsPanelUI.Instance.AddNewExistingQuest(questClone);
        QuestPanel.gameObject.SetActive(false);
    }
}
