using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRay : MonoBehaviour
{
    protected RaycastHit rhit;

    protected Collider CurrentCollider = null;

    [SerializeField]
    protected LayerMask HitMask;

    public InteractableEntity CurrentInteractable = null;

    public virtual void EmitRay()
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



    public virtual void OnHover()
    {
        CursorTooltipUI.TooltipInstance.Show(CurrentInteractable.Name);
    }

    public virtual void UnHover()
    {
        CursorTooltipUI.TooltipInstance.Hide();
    }
}
