using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeadlineContentUI : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI ContentText;

    [SerializeField]
    protected Image Icon;

    [SerializeField]
    protected Sprite DefaultIcon;

    public virtual void SetInfo(string content = "", Sprite icon = null)
    {
        ContentText.text = content;

        if (icon != null)
        {
            Icon.sprite = icon;
        }
        else
        {
            Icon.sprite = DefaultIcon;
        }
        
    }
}
