using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomDisplayUI : MonoBehaviour, IPointerClickHandler
{
    public Image DisplayImage;

    public virtual void SetSelected()
    {
        DisplayImage.color = Color.yellow;
    }

    public virtual void SetDeselected()
    {
        DisplayImage.color = Color.white;
    }

    public virtual void Select()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }
}
