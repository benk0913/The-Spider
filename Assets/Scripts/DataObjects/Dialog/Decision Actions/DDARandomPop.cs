using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDARandomPop", menuName = "DataObjects/Dialog/Actions/DDARandomPop", order = 2)]
public class DDARandomPop : DialogDecisionAction
{
    [SerializeField]
    Sprite img1;

    [SerializeField]
    Sprite img2;

    [SerializeField]
    string AudioKey;


    public override void Activate()
    {
        RandomPopEffectUI.Instance.Show(img1, img2, AudioKey);
    }


}
