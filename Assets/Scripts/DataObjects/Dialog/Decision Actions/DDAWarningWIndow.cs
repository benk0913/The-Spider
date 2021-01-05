using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAWarningWIndow", menuName = "DataObjects/Dialog/Actions/DDAWarningWIndow", order = 2)]
public class DDAWarningWIndow : DialogDecisionAction
{
    public DialogDecisionAction OnAccept;

    public string Message;

    public override void Activate()
    {
        WarningWindowUI.Instance.Show(Message, ()=> 
        {
            OnAccept.Activate();
        });
    }
}
