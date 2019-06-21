using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRay : MonoBehaviour
{
    RaycastHit rhit;

    Collider CurrentCollider = null;

    [SerializeField]
    LayerMask HitMask;

    [SerializeField]
    CursorTooltipUI Tooltip;

    public InteractableEntity CurrentInteractable = null;

    public void EmitRay()
    {
        Physics.Raycast(transform.position, transform.forward,  out rhit, HitMask);

        if (rhit.collider != null)
        {
            if (rhit.collider != CurrentCollider)
            {
                CurrentCollider = rhit.collider;
                CurrentInteractable = rhit.collider.GetComponent<InteractableEntity>();
                if (CurrentInteractable != null)
                {
                    OnHover();
                    return;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            CurrentCollider = null;
        }
        
        CurrentInteractable = null;
        UnHover();
    }

    

    public void OnHover()
    {
        Tooltip.Show(CurrentInteractable.Name);
    }

    public void UnHover()
    {
        Tooltip.Hide();
    }
}
