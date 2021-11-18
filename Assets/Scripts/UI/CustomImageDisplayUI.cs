using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomImageDisplayUI : CustomDisplayUI
{
    public Sprite CurrentSprite;

    public void SetImage(Sprite sprite)
    {
        CurrentSprite = sprite;
        DisplayImage.sprite = CurrentSprite;
        DisplayImage.color = Color.white;
    }
    
    public override void Select()
    {
        PickCustomImageUI.Instance.SelectDisplay(this);
    }
    
}
