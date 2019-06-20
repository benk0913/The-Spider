using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEventInvokerEntity : MonoBehaviour
{
    public void InvokeDynamicEvent(string eventKey)
    {
        CORE.Instance.InvokeEvent(eventKey);
    }
}
