using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnReportItemUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Content;

    [SerializeField]
    PortraitUI Portrait;

    [SerializeField]
    Image Icon;

    public void SetInfo(TurnReportLogItemInstance logItem)
    {
        SetInfo(logItem.Content, logItem.Icon, logItem.RelevantCharacter);
    }

    public void SetInfo(string content = "Something new happened", Sprite sprite = null, Character character = null)
    {
        Content.text = content;
        
        if(sprite == null)
        {
            Icon.gameObject.SetActive(false);
        }
        else
        {
            Icon.gameObject.SetActive(true);
            Icon.sprite = sprite;
        }

        if(character == null)
        {
            Portrait.gameObject.SetActive(false);
        }
        else
        {
            Portrait.gameObject.SetActive(true);
            Portrait.SetCharacter(character);
        }
    }
}
