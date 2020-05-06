using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TellOnColleague", menuName = "DataObjects/AgentActions/TellOnColleague", order = 2)]
public class TellOnColleague : AgentAction
{
    Character CurrentCharacter;
    public LongTermTask Task;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        CurrentCharacter = character;

        //TODO - REPLACE THIS!
        Character snitchTarget = null;

        if (Random.Range(0, 2) == 0 && character.Employer != null && CanBeSnitchedUpon(character.Employer))
        {
            snitchTarget = character.Employer;
            
        }
        else
        {
            if (character.WorkLocation != null)
            {
                List<Character> possibleEmployees = new List<Character>();
                possibleEmployees.InsertRange(0, character.WorkLocation.EmployeesCharacters);

                for(int i=0;i<possibleEmployees.Count;i++)
                {
                    if (!CanBeSnitchedUpon(possibleEmployees[i]))
                    {
                        possibleEmployees.Remove(possibleEmployees[i]);
                        i--;
                    }
                }

                if (possibleEmployees.Count > 0)
                {
                    snitchTarget = possibleEmployees[Random.Range(0, possibleEmployees.Count)];
                }
            }
        }

        if (snitchTarget != null)
        {
            character.Reputation -= 2;
            CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, snitchTarget, target);
        }


        CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target, null, -1, null, this);
    }

    bool CanBeSnitchedUpon(Character character)
    {
        if(character == null)
        {
            return false;
        }

        if(character == CurrentCharacter)
        {
            return false;
        }

        if (character.CurrentTaskEntity != null)
        {
            switch (character.CurrentTaskEntity.CurrentTask.name)
            {
                case "Being Hanged":
                    {
                        return false;
                    }
                case "Being Interrogated":
                    {
                        return false;
                    }
                case "Locked In Prison":
                    {
                        return false;
                    }
                case "Obsolescence":
                    {
                        return false;
                    }
            }
        }

        if(CurrentCharacter.GetRelationsWith(character) > 5)
        {
            return false;
        }

        return true;
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        LocationEntity location = (LocationEntity)target;

        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
        {
            return false;
        }

        return true;
    }
}
