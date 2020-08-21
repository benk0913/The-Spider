using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FireAgent", menuName = "DataObjects/PlayerActions/FireAgent", order = 2)]
public class FireAgent : PlayerAction
{
    public override void Execute(Character requester, AgentInteractable target)
    {
        FailReason reason;
        if (!CanDoAction(requester, target, out reason))
        {
            GlobalMessagePrompterUI.Instance.Show("This agent is not yours to fire.", 1f, Color.yellow);
            return;
        }

        Character character = ((PortraitUI)target).CurrentCharacter;

        WarningWindowUI.Instance.Show("Are you sure?, firing "+character.name+" may have consequences.", () => 
        {
            if (character.WorkLocation.OwnerCharacter.GetRelationsWith(character) > 5)
            {
                character.WorkLocation.OwnerCharacter.DynamicRelationsModifiers.Add
                (
                new DynamicRelationsModifier(
                new RelationsModifier("Took an employee I liked!", -2)
                , 10
                , requester)
                );
            }

            if (character.CurrentTaskEntity != null)
            {
                string taskName = character.CurrentTaskEntity.CurrentTask.name;

                if (character.IsInTrouble)
                {
                    CORE.Instance.ShowHoverMessage(
                        "Abandoned Employee, <color=red>REP -1</color>"
                        , ResourcesLoader.Instance.GetSprite("DeceasedIcon")
                        , character.WorkLocation.transform);

                    if (character.TopEmployer == CORE.PC)
                    {
                        CORE.Instance.SplineAnimationObject("BadReputationCollectedWorld",
                          character.CurrentLocation.transform,
                          StatsViewUI.Instance.transform,
                          null,
                          false);
                    }

                    character.TopEmployer.Reputation -= 2;
                }
            }

            //Snitch on colleagues when fired while arrested.
            if(character.PrisonLocation != null && character.PrisonLocation.OwnerCharacter != null && character.PrisonLocation.OwnerCharacter.CurrentFaction.name == "Constabulary")
            {
                List<Character> SnitchTargets = new List<Character>();
                SnitchTargets.AddRange(character.WorkLocation.EmployeesCharacters.FindAll(x => x != character && x.PrisonLocation == null));
                SnitchTargets.AddRange(character.WorkLocation.GuardsCharacters.FindAll(x => x != character && x.PrisonLocation == null));

                if (character.Employer.PrisonLocation == null)
                {
                    SnitchTargets.Add(character.Employer);
                }

                string charactersString = "";
                SnitchTargets.ForEach(x => charactersString += "," + x.name);
                if (charactersString.Length > 0)
                {
                    charactersString.Remove(0);
                }

                WarningWindowUI.Instance.Show(this.name + " snitched on: " + charactersString,()=> 
                {
                    foreach (Character snitchTarget in SnitchTargets)
                    {
                        CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, snitchTarget, character.CurrentLocation);
                    }
                });

            }

            character.WorkLocation.FiredEmployeees.Add(character);

            if (character.Employer == requester)
            {
                character.StopDoingCurrentTask();
                character.StopWorkingForCurrentLocation();
            }
            else
            {
                character.StopDoingCurrentTask();
                character.StopWorkingForCurrentLocation();
            }
        });
        
    }


    bool CanBeFired(Character character, out FailReason reason)
    {
        reason = null;
        if (character == null)
        {
            return false;
        }

        return true;
    }

    public override bool CanDoAction(Character requester, AgentInteractable target, out FailReason reason)
    {
        reason = null;
        PortraitUI portrait = ((PortraitUI)target);

        if (portrait.CurrentCharacter == null)
        {
            return false;
        }

        if (portrait.CurrentCharacter == requester)
        {
            return false;
        }

        if (portrait.CurrentCharacter.TopEmployer != requester)
        {
            return false;
        }
        
        if(!CanBeFired(portrait.CurrentCharacter, out reason))
        {
            return false;
        }

        return true;
    }
}
