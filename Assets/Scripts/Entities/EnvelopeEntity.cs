using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EnvelopeEntity : MonoBehaviour
{
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

    UnityAction<EnvelopeEntity> DisposeAction;


    private void Start()
    {
        if(CurrentLetter != null)
        {
            RefreshUI();
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
            FromText.text = "";
            FromPortrait.gameObject.SetActive(false);
            SubjectPortrait.gameObject.SetActive(false);
            ToPortrait.gameObject.SetActive(false);
        }


        ArchiveActionText.text = "Press '" + InputMap.Map["Interact"].ToString() + "' to ARCHIVE.";
        DeleteActionText.text = "Press '" + InputMap.Map["Secondary Interaction"].ToString() + "' to DISPOSE.";
        RetreiveActionText.text = "Press 'Escape' to return...";
    }

    public void Archive()
    {
        LettersPanelUI.Instance.AddLetterToLog(this.CurrentLetter);
        DisposeAction(this);
        Destroy(this.gameObject);
    }

    public void Delete()
    {
        DisposeAction(this);
        Destroy(this.gameObject);
    }
}
