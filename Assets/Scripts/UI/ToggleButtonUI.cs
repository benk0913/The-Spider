using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleButtonUI : MonoBehaviour
{
    public bool isOn = false;

    [SerializeField]
    UnityEvent OnToggleOn;

    [SerializeField]
    UnityEvent OnToggleOff;

    public void Toggle()
    {
        isOn = !isOn;

        if(isOn)
        {
            OnToggleOn.Invoke();
        }
        else
        {
            OnToggleOff.Invoke();
        }
    }
}
