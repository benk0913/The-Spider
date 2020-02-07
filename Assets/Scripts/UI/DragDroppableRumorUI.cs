using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragDroppableRumorUI : MonoBehaviour
{
    public KnowledgeRumor CurrentRumor;

    [SerializeField]
    TextMeshProUGUI Label;

    [SerializeField]
    Image BackgroundImage;

    [SerializeField]
    Animator Anim;

    bool isDragging;

    public void SetInfo(KnowledgeRumor rumor)
    {
        this.CurrentRumor = rumor;
        RefreshUI();
    }

    void RefreshUI()
    {
        this.Label.text = CurrentRumor.Description;
    }

    public void OnDragStart()
    {
        ResearchCharacterWindowUI.Instance.CurrentDragged = this;
        isDragging = true;
        BackgroundImage.color = Color.green;
        BackgroundImage.raycastTarget = false;
    }

    private void Update()
    {
        if(isDragging)
        {
            transform.position = Input.mousePosition;

            if(Input.GetMouseButtonUp(0))
            {
                OnDrop();
            }
        }
    }

    public void OnDrop()
    {
        if (ResearchCharacterWindowUI.Instance.CurrentDragged == this)
        {
            ResearchCharacterWindowUI.Instance.CurrentDragged = null;
        }

        isDragging = false;
        BackgroundImage.color = Color.white;
        BackgroundImage.raycastTarget = true;
    }

    public void Consume()
    {
        if (ResearchCharacterWindowUI.Instance.CurrentDragged == this)
        {
            ResearchCharacterWindowUI.Instance.CurrentDragged = null;
        }

        Anim.SetTrigger("Consume");
    }

    public void ConsumeIncorrect()
    {
        if (ResearchCharacterWindowUI.Instance.CurrentDragged == this)
        {
            ResearchCharacterWindowUI.Instance.CurrentDragged = null;
        }

        Anim.SetTrigger("ConsumeIncorrect");
    }

    public void OnAnimationExit()
    {
        gameObject.SetActive(false);
        transform.SetParent(transform.parent.parent);
    }
}
