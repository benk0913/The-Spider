using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInteractionRay : MonoBehaviour
{
    RaycastHit rhit;

    Collider CurrentCollider;

    PointAndClickEntity CurrentInteractable;


    [SerializeField]
    Camera m_Camera;

    [SerializeField]
    LayerMask HitMask;

    [SerializeField]
    EventSystem _UIEventSystem;

    private void Update()
    {
        if (_UIEventSystem != null && _UIEventSystem.IsPointerOverGameObject())
        {
            return;
        }

        EmitRay();

        if (CurrentInteractable != null && Input.GetMouseButtonDown(0))
        {
            CurrentInteractable.OnClick();
        }
    }

    public void EmitRay()
    {
        Physics.Raycast(m_Camera.ScreenPointToRay(Input.mousePosition), out rhit, HitMask);


        if (rhit.collider != null)
        {
            if (rhit.collider != CurrentCollider)
            {
                if(CurrentInteractable != null)
                {
                    UnHover();
                }

                CurrentCollider = rhit.collider;
                CurrentInteractable = rhit.collider.GetComponent<PointAndClickEntity>();
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

        UnHover();
        CurrentInteractable = null;
    }

    public void OnHover()
    {
        //Tooltip.Show(CurrentInteractable.Name);
        CurrentInteractable.Hover();
    }

    public void UnHover()
    {
        //Tooltip.Hide();

        if (CurrentInteractable != null)
        {
            CurrentInteractable.Unhover();
        }
    }
}
