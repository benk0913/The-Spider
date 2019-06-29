using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DynamicEventListenerEntity : MonoBehaviour
{
    [SerializeField]
    public List<DynamicEventListenerInstance> Events = new List<DynamicEventListenerInstance>();

    public bool ShutOnInit = false;

    private void Start()
    {
        foreach (DynamicEventListenerInstance eventListener in Events)
        {
            eventListener.Action = new UnityAction(
                delegate
                {
                    eventListener.Event.Invoke();
                });

            CORE.Instance.SubscribeToEvent(eventListener.Key, eventListener.Action);
        }

        if(ShutOnInit)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (!ShutOnInit)
        {
            foreach (DynamicEventListenerInstance eventListener in Events)
            {
                CORE.Instance.UnsubscribeFromEvent(eventListener.Key, eventListener.Action);
            }
        }
    }


}

[System.Serializable]
public class DynamicEventListenerInstance
{
    [SerializeField]
    public string Key;

    [SerializeField]
    public UnityEvent Event;

    public UnityAction Action;
}
