using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickableItem : MonoBehaviour
{
    [SerializeField]
    List<UnityEvent> InteractionEventsFlow = new List<UnityEvent>();

    public int CurrentInteractionIndex;

    public bool isPicked = false;

    public void PickUp()
    {
        MouseLook.Instance.PickUpItem(this);
    }
    
    public void Retrieve()
    {
        MouseLook.Instance.RetreiveItem();
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
}
