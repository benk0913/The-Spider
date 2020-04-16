using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDASetTutorialScreen", menuName = "DataObjects/Dialog/Actions/DDASetTutorialScreen", order = 2)]
public class DDASetTutorialScreen : DialogDecisionAction
{
    [SerializeField]
    TutorialScreenInstance Tutorial;

    public override void Activate()
    {
        TutorialScreenUI.Instance.Show(Tutorial.name);
    }
}
