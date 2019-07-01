﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickableItem : MonoBehaviour
{
    [SerializeField]
    List<UnityEvent> InteractionEventsFlow = new List<UnityEvent>();

    [SerializeField]
    UnityEvent SecondaryInteraction = new UnityEvent();

    public int CurrentInteractionIndex;

    public bool isPicked = false;

    public void PickUp()
    {
        MouseLook.Instance.PickUpItem(this);
        isPicked = true;
    }
    
    public void Retrieve()
    {
        MouseLook.Instance.RetreiveItem();
        isPicked = false;
    }

    public void ReleaseItem()
    {
        MouseLook.Instance.ReleaseItem();
        isPicked = false;

    }

    public void Interact()
    {
        if(CurrentInteractionIndex >= InteractionEventsFlow.Count)
        {
            return;
        }

        InteractionEventsFlow[CurrentInteractionIndex].Invoke();
        CurrentInteractionIndex++;
    }

    public void SecondaryInteract()
    {
        SecondaryInteraction.Invoke();
    }
}