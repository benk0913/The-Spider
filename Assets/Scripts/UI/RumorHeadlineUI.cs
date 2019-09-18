using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RumorHeadlineUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    Animator Anim;

    [SerializeField]
    GameObject archiveButton;

    [SerializeField]
    GameObject oldMarker;

    public Rumor CurrentRumor;

    GameObject ShowingObject;

    RumorsPanelUI ParentPanel;

    public void SetInfo(Rumor rumor, RumorsPanelUI parentPanel, bool canArchive = true)
    {
        CurrentRumor = rumor;
        ParentPanel = parentPanel;

        Title.text = CurrentRumor.Title;

        archiveButton.SetActive(canArchive);
        oldMarker.SetActive(false);
    }

    public void Archive()
    {
        Hide();
        this.gameObject.SetActive(false);
        this.transform.SetParent(transform.parent.parent);
        ParentPanel.Archive(CurrentRumor);
    }

    public void Toggle()
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

    public void Hide()
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
