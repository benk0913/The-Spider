using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverPanelUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    Image IconImage;

    [SerializeField]
    WorldPositionLerperUI Lerper;

    public void Show(Vector2 position, Color textColor, string text = "", Sprite icon = null)
    {
        Title.color = textColor;

        Show(position, text, icon);
    }

    public void Show(Vector2 position ,string text = "",Sprite icon = null)
    {

        gameObject.SetActive(true);

        Lerper.SetPosition(position);

        Title.text = text;

        if (icon == null)
        {
            IconImage.color = Color.clear;
        }
        else
        {
            IconImage.sprite = icon;
            IconImage.color = Color.white;
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
