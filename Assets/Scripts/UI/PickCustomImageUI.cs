using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickCustomImageUI : MonoBehaviour
{
    public static PickCustomImageUI Instance;

    public Transform Container;

    CustomImageDisplayUI CurrentSelectedDisplay;

    public TextMeshProUGUI Label;

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }


    public Action<Sprite> OnPick;


    public void Show(Action<Sprite> onPick, List<Sprite> options, string title = "Pick a custom image:")
    {
        this.OnPick = onPick;
        this.gameObject.SetActive(true);
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).transform.SetParent(null);
        }
        foreach(Sprite sprite in options)
        {
            CustomImageDisplayUI customImageDisplay = ResourcesLoader.Instance.GetRecycledObject("CustomImageDisplayUI").GetComponent<CustomImageDisplayUI>();
            customImageDisplay.SetImage(sprite);
            customImageDisplay.transform.localScale = Vector3.one;
            customImageDisplay.transform.position = Vector3.zero;
            customImageDisplay.transform.SetParent(Container,false);
        }

        Label.text = title;
    }

    public void SelectDisplay(CustomImageDisplayUI selected)
    {
        if(CurrentSelectedDisplay != null)
            CurrentSelectedDisplay.SetDeselected();

        CurrentSelectedDisplay = selected;
        CurrentSelectedDisplay.SetSelected();
    }

    public void Confirm()
    {
        if(CurrentSelectedDisplay == null)
        {
            return;
        }

        OnPick?.Invoke(CurrentSelectedDisplay.CurrentSprite);
        Hide();
    }


    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
