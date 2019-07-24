using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventInvokerEntity : MonoBehaviour
{
    [SerializeField]
    List<UnityEvent> EventsList = new List<UnityEvent>();

    public void InvokeEventInIndex(int index)
    {
        EventsList[index].Invoke();
    }
}
