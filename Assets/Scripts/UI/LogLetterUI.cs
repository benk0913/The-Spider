using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogLetterUI : MonoBehaviour
{
    LettersPanelUI CurrentParent;

    [SerializeField]
    TextMeshProUGUI TitleText;

    [SerializeField]
    Image CurrentImage;

    public Letter CurrentLetter;

    public void SetInfo(Letter letter, LettersPanelUI parent)
    {
        CurrentLetter = letter;
        TitleText.text = letter.Title;
        CurrentParent = parent;
    }

    public void Interact()
    {
        CurrentParent.LetterSelected(this);
    }

    public void Select()
    {
        CurrentImage.color = Color.yellow;
        TitleText.color = Color.black;
    }

    public void Deselect()
    {
        CurrentImage.color = Color.white;
        TitleText.color = Color.white;
    }
}
