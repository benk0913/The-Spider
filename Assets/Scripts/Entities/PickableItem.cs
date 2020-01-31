using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickableItem : MonoBehaviour
{
    [SerializeField]
    List<UnityEvent> InteractionEventsFlow = new List<UnityEvent>();

    [SerializeField]
    UnityEvent SecondaryInteraction = new UnityEvent();

    [SerializeField]
    UnityEvent AcceptInteraction = new UnityEvent();

    [SerializeField]
    UnityEvent OnRetreive;

    [SerializeField]
    LayerMask CollLayerMask;

    public int CurrentInteractionIndex;

    public bool isPicked = false;

    public bool DisableLookaround = false;

    public bool Retreiveable = true;

    

    public void PickUp()
    {
        MouseLook.Instance.PickUpItem(this);
        isPicked = true;
    }
    
    public void Retrieve()
    {
        if(Retreiveable)
        {
            MouseLook.Instance.RetreiveItem();

            CurrentInteractionIndex = 0;

            isPicked = false;
        }
        else
        {
            ReleaseItem();
        }

        OnRetreive.Invoke();

    }

    public void ReleaseItem()
    {
        CurrentInteractionIndex = 0;
        isPicked = false;
        MouseLook.Instance.ReleaseItem();

        RaycastHit rhit;
        if(Physics.Raycast(transform.position, -Vector3.up, out rhit, CollLayerMask))
        {
            transform.position = rhit.point;
            transform.rotation = Quaternion.Euler(rhit.normal+ new Vector3(90f, 0f, 0f));
        }
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

    public void AcceptInteract()
    {
        AcceptInteraction.Invoke();
    }
}
