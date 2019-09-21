﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RumorHeadlineUI : HeadlineUI
{

    [SerializeField]
    GameObject archiveButton;

    [SerializeField]
    GameObject marker;

    public Rumor CurrentRumor;

    RumorsPanelUI ParentPanel;

    public void SetInfo(Rumor rumor, RumorsPanelUI parentPanel, bool canArchive = false)
    {
        CurrentRumor = rumor;
        ParentPanel = parentPanel;

        Title.text = CurrentRumor.Title;

        archiveButton.SetActive(canArchive);
        marker.SetActive(false);
    }

    public override void Archive()
    {
        Hide();
        this.gameObject.SetActive(false);
        this.transform.SetParent(transform.parent.parent);
        ParentPanel.Archive(CurrentRumor);
    }

    public override void Toggle()
    {
        if(ShowingObject != null)
        {
            Hide();
            return;
        }

        Anim.SetBool("Showing", true);
        ShowingObject = ResourcesLoader.Instance.GetRecycledObject("RumorContentUI");
        ShowingObject.transform.SetParent(transform.parent, false);
        ShowingObject.transform.SetSiblingIndex(transform.GetSiblingIndex()+1);
        ShowingObject.GetComponent<RumorContentUI>().SetInfo(CurrentRumor);
    }

    public override void Hide()
    {
        if(ShowingObject == null)
        {
            return;
        }

        ShowingObject.gameObject.SetActive(false);
        ShowingObject.transform.SetParent(transform.parent.parent);
        ShowingObject = null;
        Anim.SetBool("Showing", false);
    }



}
