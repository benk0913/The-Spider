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

    public void Interact()
    {
        Actions.Invoke();
    }
}
