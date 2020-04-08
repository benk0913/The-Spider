using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableEntity : MonoBehaviour
{
    [SerializeField]
    public string Name;

    [SerializeField]
    public UnityEvent Actions = new UnityEvent();

    [SerializeField]
    public DialogDecisionAction DDAction;

    public void Interact()
    {
        Actions.Invoke();
        DDAction?.Activate();
    }
}
