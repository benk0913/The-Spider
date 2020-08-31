using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAOwnerSkillChallenge", menuName = "DataObjects/Dialog/Actions/DDAOwnerSkillChallenge", order = 2)]
public class DDAOwnerSkillChallenge : DialogDecisionAction
{
    public Property TheProperty;
    public Faction OfFaction;
    public BonusType SkillType;
    public float ChallengeAmount;
    public DialogPiece OnWin;
    public DialogPiece OnLose;

    public override void Activate()
    {
        LocationEntity targetLocation = null;
        if (TheProperty != null && OfFaction != null)
        {
            targetLocation = CORE.Instance.Locations.Find(X => X.CurrentProperty == TheProperty && X.FactionInControl != null && X.FactionInControl.name == OfFaction.name);
        }
        else if(DialogWindowUI.Instance.GetDialogParameter("Location") != null)
        {
            targetLocation = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        }

        if (targetLocation == null)
        {
            Debug.LogError("NO RELEVANT LOCATION FOR " + this.name);
            return;
        }

        if(targetLocation.OwnerCharacter == null)
        {
            DialogWindowUI.Instance.ShowDialogPiece(OnLose);
            return;
        }

        float ownerSkill = targetLocation.OwnerCharacter.GetBonus(SkillType).Value;

        if(Random.Range(0f,ownerSkill+ChallengeAmount) > ownerSkill)
        {
            DialogWindowUI.Instance.ShowDialogPiece(OnLose);
            return;
        }

        DialogWindowUI.Instance.ShowDialogPiece(OnWin);
    }
}
