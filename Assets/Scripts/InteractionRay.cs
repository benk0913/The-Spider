using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRay : MonoBehaviour
{
    RaycastHit rhit;

    Collider CurrentCollider = null;

    [SerializeField]
    LayerMask HitMask;

    InteractableEntity CurrentInteractable = null;

    private void Update()
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

    private void LateUpdate()
    {
        if(Input.GetKey(InputMap.Map["Interact"]))
        {
            Interact();
        }
    }

    void Interact()
    {
        if(CurrentInteractable == null)
        {
            return;
        }

        CurrentInteractable.Interact();
        UnHover();
    }

    void OnHover()
    {
        CursorTooltipUI.Instance.Show(CurrentInteractable.Name);
    }

    void UnHover()
    {
        CursorTooltipUI.Instance.Hide();
    }
}
