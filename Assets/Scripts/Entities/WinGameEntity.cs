using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGameEntity : MonoBehaviour
{
    public void Win()
    {
        VictoryWindowUI.Instance.Show();
    }
}
