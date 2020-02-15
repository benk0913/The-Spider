using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QRInvokeDialogAction", menuName = "DataObjects/Quests/Rewards/QRInvokeDialogAction", order = 2)]
public class QRInvokeDialogAction : QuestReward
{
    public DialogDecisionAction action;

    public override void Claim(Character byCharacter)
    {
        base.Claim(byCharacter);

        action.Activate();
    }
}