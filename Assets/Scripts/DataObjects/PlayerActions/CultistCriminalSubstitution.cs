using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CultistCriminalSubstitution", menuName = "DataObjects/PlayerActions/CultistCriminalSubstitution", order = 2)]
public class CultistCriminalSubstitution : PlayerAction
{

    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("This is not a cultist.", 1f, Color.yellow);
            return;
        }

        base.Execute(requester, target);

        Character targetChar = ((PortraitUI)target).CurrentCharacter;

        SelectCharacterViewUI.Instance.Show(
            (targetPrisoner)=>
            {
                targetPrisoner.ExitPrison();
                CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, targetChar, targetChar.CurrentLocation);
            }
            ,
            x => x.TopEmployer == CORE.PC
            && x.PrisonLocation != null
            && x.PrisonLocation.Traits.Contains(CORE.Instance.Database.LawAreaTrait)
            && x != targetChar
            ,"Select Prisoner To Replace");
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;

        Character character = ((PortraitUI)target).CurrentCharacter;

        if(character == null)
        {
            return false;
        }

        if(character.Traits.Find(x=>x==CORE.Instance.Database.CultistTrait) == null)
        {
            return false;
        }

        if (character.Traits.Find(x => x == CORE.Instance.Database.CultistReligiousTrait) == null)
        {
            reason = new FailReason("This character is not a devoted member of your cult.");
            return false;
        }

        if (character.PrisonLocation != null)
        {
            reason = new FailReason("This character is already in prison!");

            return false;
        }

        if (!base.CanDoAction(requester,target,out reason))
        {
            return false;
        }

        return true;
    }
}
