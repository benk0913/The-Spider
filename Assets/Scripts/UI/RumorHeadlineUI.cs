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

    public Rumor CurrentRumor;

    GameObject ShowingObject;

    public void SetInfo(Rumor rumor)
    {
        CurrentRumor = rumor;

        Title.text = CurrentRumor.Title;
    }

    public void Archive()
    {
        this.gameObject.SetActive(false);
        this.transform.SetParent(transform.parent.parent);
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
        ShowingObject.transform.SetSiblingIndex(transform.GetSiblingIndex());
        ShowingObject.GetComponent<RumorContentUI>().SetInfo(CurrentRumor);
    }

    public void Hide()
    {
        ShowingObject.gameObject.SetActive(false);
        ShowingObject.transform.SetParent(transform.parent.parent);
        ShowingObject = null;
        Anim.SetBool("Showing", false);
    }

}
