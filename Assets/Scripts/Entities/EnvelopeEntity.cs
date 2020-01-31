using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EnvelopeEntity : MonoBehaviour, ISaveFileCompatible
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
    GameObject EncryptionPanel;

    [SerializeField]
    TextMeshProUGUI QuestName;

    [SerializeField]
    TextMeshProUGUI QuestFirstObjective;

    [SerializeField]
    TextMeshProUGUI QuestAcceptText1;

    [SerializeField]
    TextMeshProUGUI QuestAcceptText2;

    UnityAction<EnvelopeEntity> DisposeAction;


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

    public void SetInfo(Letter letter, UnityAction<EnvelopeEntity> disposeAction)
    {
        CurrentLetter = letter;
        PresetLetter = letter.Preset;
        DisposeAction = disposeAction;

        RefreshUI();


    }

    void RefreshUI()
    {
        if(CurrentLetter == null)
        {
            return; 
        }

        if(CurrentLetter.Preset == null)
        {
            return;
        }

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

        if(CurrentLetter.Preset.Encryption != null && !CurrentLetter.IsDeciphered)
        {
            EncryptionPanel.gameObject.SetActive(true);
            QuestPanel.gameObject.SetActive(false);

            ArchiveActionText.text = "Cannot Archive Encrypted Letters...";
            DeleteActionText.text = "Cannot Delete Encrypted Letters...";
            RetreiveActionText.text = "Press 'Escape' to return...";

            return;
        }

        EncryptionPanel.gameObject.SetActive(false);

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

        if(CurrentLetter.Preset.QuestAttachment != null && !QuestsPanelUI.Instance.HasQuest(CurrentLetter.Preset.QuestAttachment, CORE.PC))
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
        DeleteActionText.text = "";
        RetreiveActionText.text = "Press 'Escape' to return...";
    }

    public void Archive()
    {
        if(CurrentLetter.Preset.Encryption != null && !CurrentLetter.IsDeciphered)
        {
            return;
        }

        LettersPanelUI.Instance.AddLetterToLog(this.CurrentLetter);

        DisposeAction?.Invoke(this);

        Destroy(this.gameObject);
    }

    public void Delete()
    {
        if(CurrentLetter.Preset.Encryption != null && !CurrentLetter.IsDeciphered)
        {
            return;
        }

        DisposeAction(this);
        Destroy(this.gameObject);
    }

    public void AcceptQuest()
    {
        if(CurrentLetter.Preset.Encryption != null && !CurrentLetter.IsDeciphered)
        {
            DecipherWindowUI.Instance.Show(CurrentLetter.Content, CurrentLetter.Preset.Encryption, ()=> { this.CurrentLetter.IsDeciphered = true; RefreshUI(); SealRenderer.gameObject.SetActive(false); });
            return;
        }

        if(CurrentLetter.Preset.QuestAttachment == null)
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

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        if(PresetLetter == null)
        {
            PresetLetter = CurrentLetter.Preset;
        }

        node["PresetLetter"] = PresetLetter.name;
        node["Letter"] = CurrentLetter.ToJSON();

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        PresetLetter = CORE.Instance.Database.PresetLetters.Find(x => x.name == node["PresetLetter"]);

        if (CurrentLetter == null)
        {
            CurrentLetter = new Letter(PresetLetter, null);
        }

        CurrentLetter.FromJSON(node["Letter"]);
    }

    public void ImplementIDs()
    {
        CurrentLetter.ImplementIDs();
        RefreshUI();
    }
}
