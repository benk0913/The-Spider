using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeadlineUI : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI Title;

    [SerializeField]
    protected Animator Anim;

    [SerializeField]
    string content;

    public GameObject ShowingObject;

    public virtual void SetInfo(string headlineTitle)
    {
        Title.text = headlineTitle;
    }

    public virtual void Archive()
    {
        Hide();
        this.gameObject.SetActive(false);
        this.transform.SetParent(transform.parent.parent);
    }

    public virtual void Toggle()
    {
        if (ShowingObject != null)
        {
            Hide();
            return;
        }

        Anim.SetBool("Showing", true);
        ShowingObject = ResourcesLoader.Instance.GetRecycledObject("RumorContentUI");
        ShowingObject.transform.SetParent(transform.parent, false);
        ShowingObject.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        ShowingObject.GetComponent<HeadlineContentUI>().SetInfo(content);
    }

    public virtual void Hide()
    {
        if (ShowingObject == null)
        {
            return;
        }

        ShowingObject.gameObject.SetActive(false);
        ShowingObject.transform.SetParent(transform.parent.parent);
        ShowingObject = null;
        Anim.SetBool("Showing", false);
    }

}
