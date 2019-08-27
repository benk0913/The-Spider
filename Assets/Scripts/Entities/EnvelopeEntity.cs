using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private void Start()
    {
        if(CurrentLetter != null)
        {
            RefreshUI();
        }
    }

    public void SetInfo(Letter letter)
    {
        CurrentLetter = letter;

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
        FromText.text = ((Character)CurrentLetter.Parameters["Letter_From"]).name;

        ArchiveActionText.text = "Press '" + InputMap.Map["Interact"].ToString() + "' to ARCHIVE.";
        ArchiveActionText.text = "Press '" + InputMap.Map["Secondary Interaction"].ToString() + "' to DISPOSE.";
        ArchiveActionText.text = "Press 'Escape' to return...";
    }

    public void Archive()
    {
        LettersPanelUI.Instance.AddLetterToLog(this.CurrentLetter);
        Destroy(this.gameObject);
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }
}
