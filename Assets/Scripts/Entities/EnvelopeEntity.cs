using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EnvelopeEntity : MonoBehaviour, ISaveFileCompatible
{
    [SerializeField]
    public LetterPreset PresetLetter;

    [SerializeField]
    public Letter CurrentLetter;

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
    TextMeshProUGUI RTLDescription;

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

    public bool RoomSetLetter = false;

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
        
        if (RoomSetLetter)
        {
            CORE.Instance.UnsubscribeFromEvent("GameLoadComplete", GameLoadComplete);
            CORE.Instance.SubscribeToEvent("GameLoadComplete", GameLoadComplete);
            GameLoadComplete();
        }
    }

    public void SetInfo(Letter letter, UnityAction<EnvelopeEntity> disposeAction)
    {
        CurrentLetter = letter;
        PresetLetter = letter.Preset;
        DisposeAction = disposeAction;

        RefreshUI();
    }

    public void GameLoadComplete()
    {
        if (LettersPanelUI.Instance != null && LettersPanelUI.Instance.ArchivedLetters.Find(x =>
        {
            if (x.CurrentLetter.Preset == null)
            {
                return false;
            }

            return x.CurrentLetter.Title == this.CurrentLetter.Title;
        }) != null)
        {
            Delete();
        }
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
            if (SealRenderer != null)
            {
                SealRenderer.gameObject.SetActive(true);
                SealRenderer.material = CurrentLetter.Preset.Seal;
            }
        }
        else
        {
            if (SealRenderer != null)
            {
                SealRenderer.gameObject.SetActive(false);
            }
        }

        TitleText.text = CurrentLetter.Title;

        DescriptionText.text = CurrentLetter.Content;
        RTLDescription.text = CurrentLetter.Preset.RTLDescription;
        SideNotesText.text = CurrentLetter.Preset.SideNotes;

        if(CurrentLetter.Preset.Encryption != null && !CurrentLetter.IsDeciphered)
        {
            EncryptionPanel.gameObject.SetActive(true);
            QuestPanel.gameObject.SetActive(false);

            ArchiveActionText.text = "Cannot Archive Encrypted Letters...";
            DeleteActionText.text = "Cannot Delete Encrypted Letters...";
            RetreiveActionText.text = "Press 'Escape' to return...";
            
            if(CurrentLetter.Preset.Encryption.Font != null)
            {
                DescriptionText.font = CurrentLetter.Preset.Encryption.Font;
                DescriptionText.lineSpacing = CurrentLetter.Preset.Encryption.LineSpacing;
            }
            else
            {
                DescriptionText.font = CORE.Instance.Database.DefaultFont;
                DescriptionText.lineSpacing = 190.5f;
            }

            return;
        }
        else
        {
            DescriptionText.font = CORE.Instance.Database.DefaultFont;
        }

        EncryptionPanel.gameObject.SetActive(false);

        if (CurrentLetter.Parameters != null)
        {
            FromPortrait.gameObject.SetActive(true);
            ToPortrait.gameObject.SetActive(true);

            if (CurrentLetter.Parameters.ContainsKey("Letter_From") && CurrentLetter.Parameters["Letter_From"] != null)
            {
                FromText.text = ((Character)CurrentLetter.Parameters["Letter_From"]).name;
                FromPortrait.SetCharacter(((Character)CurrentLetter.Parameters["Letter_From"]));
            }

            if (CurrentLetter.Parameters.ContainsKey("Letter_To") && CurrentLetter.Parameters["Letter_To"] != null)
            {
                ToPortrait.SetCharacter(((Character)CurrentLetter.Parameters["Letter_To"]));
            }

            if (CurrentLetter.Parameters.ContainsKey("Letter_SubjectCharacter") && CurrentLetter.Parameters["Letter_SubjectCharacter"] != null)
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

        if (CurrentLetter.Preset.QuestAttachment != null && CurrentLetter.Preset.QuestAttachment.MustQuest)
        {
            AcceptQuest();
        }

        LettersPanelUI.Instance.AddLetterToLog(this.CurrentLetter);

        DisposeAction?.Invoke(this);

        if (!string.IsNullOrEmpty(CurrentLetter.Preset.VoiceLine))
        {
            AudioControl.Instance.StopSound(CurrentLetter.Preset.VoiceLine);
        }

        Destroy(this.gameObject);
    }

    public void Delete()
    {
        if(CurrentLetter.Preset.Encryption != null && !CurrentLetter.IsDeciphered)
        {
            return;
        }

        if (!string.IsNullOrEmpty(CurrentLetter.Preset.VoiceLine))
        {
            AudioControl.Instance.StopSound(CurrentLetter.Preset.VoiceLine);
        }

        DisposeAction?.Invoke(this);
        Destroy(this.gameObject);
    }

    public void AcceptQuest()
    {
        if(CurrentLetter.Preset.Encryption != null && !CurrentLetter.IsDeciphered)
        {
            System.Action onDecipherAction = () =>
            {
                this.CurrentLetter.IsDeciphered = true;
                RefreshUI();

                if (SealRenderer != null)
                {
                    SealRenderer.gameObject.SetActive(false);
                }
            };

            if (CurrentLetter.Preset.Encryption.IsDoubleTransposition)
            {
                DTWindowUI.Instance.Show(CurrentLetter.Content, CurrentLetter.DTKeyword, onDecipherAction);
            }
            else
            {
                DecipherWindowUI.Instance.Show(CurrentLetter.Content, CurrentLetter.Preset.Encryption, onDecipherAction);
            }

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

        CORE.Instance.DelayedInvokation(0.1f, () =>
        {
            if (!string.IsNullOrEmpty(CurrentLetter.Preset.VoiceLine))
            {
                AudioControl.Instance.StopSound(CurrentLetter.Preset.VoiceLine);
            }
        });
    }

    public void OpenLetter()
    {
        if(CurrentLetter.Preset.LockPassTime)
        {
            GameClock.Instance.LockingLetter = null;
        }

        if (!string.IsNullOrEmpty(CurrentLetter.Preset.VoiceLine))
        {
            AudioControl.Instance.StopSound(CurrentLetter.Preset.VoiceLine);
            AudioControl.Instance.Play(CurrentLetter.Preset.VoiceLine);
        }
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

    public void Retreive()
    {
        CORE.Instance.DelayedInvokation(0.1f, () =>
        {
            if (!string.IsNullOrEmpty(CurrentLetter.Preset.VoiceLine))
            {
                AudioControl.Instance.StopSound(CurrentLetter.Preset.VoiceLine);
            }
        });
    }
}
