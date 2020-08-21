using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAMessagePrompt", menuName = "DataObjects/Dialog/Actions/DDAMessagePrompt", order = 2)]
public class DDAMessagePrompt : DialogDecisionAction
{
    public string Message;
    public float Time;
    public Color WithColor;

    public override void Activate()
    {
        GlobalMessagePrompterUI.Instance.Show(Message, Mathf.Clamp(Time,1f,Mathf.Infinity), WithColor);
    }
}
