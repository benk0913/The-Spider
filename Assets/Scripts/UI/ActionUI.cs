using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{

    public const string COLOR_DEFAULT = "#894346";

    public const string COLOR_SELECTED = "#FFFC2B";

    [SerializeField]
    Image ActionIcon;

    [SerializeField]
    TextMeshProUGUI ActionTitle;

    [SerializeField]
    Image ActionBG;

    [SerializeField]
    Image ActionFrame;

    [SerializeField]
    GameObject QuestionMark;


    public Property.PropertyAction CurrentAction;
    public LocationEntity CurrentLocation;

    public void SetInfo(LocationEntity location, Property.PropertyAction action = null)
    {
        CurrentAction = action;
        CurrentLocation = location;

        if (action != null)
        {
            QuestionMark.gameObject.SetActive(false);
            ActionIcon.sprite = action.Icon;
            ActionTitle.text = action.Name;
        }
        else
        {
            QuestionMark.gameObject.SetActive(true);
            ActionTitle.text = "Locked";
        }
    }

    public void AttemptSelect()
    {
        if(CurrentAction == null)
        {
            GlobalMessagePrompterUI.Instance.Show("You cant do that...", 1f);
            //TODO ACTION LOCKED ALERT
            return;
        }

        CurrentLocation.SelectAction(this.CurrentAction);
    }

    public void SetSelected()
    {
        Color clr;

        if (ColorUtility.TryParseHtmlString(COLOR_SELECTED, out clr))
        {
            ActionBG.color = clr;
            ActionFrame.color = clr;
        }
    }

    public void SetDeselected()
    {
        Color clr;

        if (ColorUtility.TryParseHtmlString(COLOR_DEFAULT, out clr))
        {
            ActionBG.color = clr;
            ActionFrame.color = clr;
        }
    }
}
