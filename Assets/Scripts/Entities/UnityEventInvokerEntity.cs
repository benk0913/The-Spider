﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventInvokerEntity : MonoBehaviour
{
    [SerializeField]
    public List<UnityEvent> EventsList = new List<UnityEvent>();


    public void InvokeEventInIndex(int index)
    {
        if(EventsList.Count <= index)
        {
            return;
        }

        EventsList[index].Invoke();
    }
}
