using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputKeyEventEntity : MonoBehaviour
{
    public List<InputKeyInstance> Instances = new List<InputKeyInstance>();

    private void Update()
    {
        foreach(InputKeyInstance input in Instances)
        {
            if(Input.GetKeyDown(InputMap.Map[input.InputKey]))
            {
                input.Event.Invoke();
            }
        }
    }
}

[System.Serializable]
public class InputKeyInstance
{
    public string InputKey;
    public UnityEvent Event;
}
