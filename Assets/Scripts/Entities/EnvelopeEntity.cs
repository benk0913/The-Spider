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
    MeshRenderer SealRenderer;

    private void Start()
    {
        //DEBUG
        RefreshUI();
    }

    public void SetInfo(Letter letter)
    {
        CurrentLetter = letter;

        RefreshUI();
    }

    void RefreshUI()
    {
        if(CurrentLetter.Seal != null)
        {
            SealRenderer.gameObject.SetActive(true);
            SealRenderer.material = CurrentLetter.Seal;
        }
        else
        {
            SealRenderer.gameObject.SetActive(false);
        }

        TitleText.text = CurrentLetter.Title;
        DescriptionText.text = CurrentLetter.Description;
        FromText.text = CurrentLetter.From;
    }
}
