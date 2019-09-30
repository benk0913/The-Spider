using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAPossibleDialogStatDuel", menuName = "DataObjects/Dialog/Actions/DDAPossibleDialogStatDuel", order = 2)]
public class DDAPossibleDialogStatDuel : DialogDecisionAction
{
    [SerializeField]
    protected DialogPiece WinPiece;

    [SerializeField]
    protected DialogPiece LosePiece;

    [SerializeField]
    protected BonusType ActorSkill;


    [SerializeField]
    protected BonusChallenge Challenge;

    public override void Activate()
    {
        float actorSkill = ((Character)DialogWindowUI.Instance.GetDialogParameter("Actor")).GetBonus(ActorSkill).Value;

        if(Random.Range(0f, actorSkill+ Challenge.RarityValue + Challenge.ChallengeValue) < (actorSkill+ Challenge.RarityValue))
        {
            if (WinPiece == null)
            {
                return;
            }

            DialogWindowUI.Instance.ShowDialogPiece(WinPiece);
        }
        else
        {
            if (LosePiece == null)
            {
                return;
            }

            DialogWindowUI.Instance.ShowDialogPiece(LosePiece);
        }
    }
}
