using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDALoseGame", menuName = "DataObjects/Dialog/Actions/DDALoseGame", order = 2)]
public class DDALoseGame : DialogDecisionAction
{

    public override void Activate()
    {
        LoseWindowUI.Instance.Show();
    }
}
