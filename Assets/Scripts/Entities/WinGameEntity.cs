using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGameEntity : MonoBehaviour
{

    public bool NoContinue = true;

    public void Win()
    {
        MouseLook.Instance.State = MouseLook.ActorState.Focusing;
        VictoryWindowUI.Instance.Show(NoContinue);
    }
}
