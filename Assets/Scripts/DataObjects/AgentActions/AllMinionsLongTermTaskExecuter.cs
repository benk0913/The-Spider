using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AllMinionsLongTermTaskExecuter", menuName = "DataObjects/AgentActions/AllMinionsLongTermTaskExecuter", order = 2)]
public class AllMinionsLongTermTaskExecuter : AgentAction //DO NOT INHERIT FROM
{
    public LongTermTask Task;
    public bool RandomLocation;
    public bool FriendlyLocation;
    public PropertyTrait LocationTrait;
    public bool DoneOnlyByEmployer = true;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        if(ActionDoneByTarget)
        {
            if(target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
            {
                Character targetCharacter = ((PortraitUI)target).CurrentCharacter;

                if(targetCharacter == null)
                {
                        GlobalMessagePrompterUI.Instance.Show("The target is currently unavailable for this task.", 1f, Color.red);
                        return;
                }

                if (targetCharacter.CurrentTaskEntity != null)
                {
                    if (targetCharacter.CurrentTaskEntity.CurrentTask == Task || !targetCharacter.CurrentTaskEntity.CurrentTask.Cancelable)
                    {
                        GlobalMessagePrompterUI.Instance.Show("The target is currently unavailable for this task.", 1f, Color.red);
                        return;
                    }
                }

                character = targetCharacter;
            }
            else if (target.GetType() == typeof(LocationEntity))
            {
                Character targetCharacter = ((LocationEntity)target).EmployeesCharacters.Find(x => 
                x.CurrentTaskEntity == null 
                || (x.CurrentTaskEntity != null && x.CurrentTaskEntity.CurrentTask != Task && x.CurrentTaskEntity.CurrentTask.Cancelable));

                if (targetCharacter == null)
                {
                    GlobalMessagePrompterUI.Instance.Show("The target is currently unavailable for this task.", 1f, Color.red);
                    return;
                }


                character = targetCharacter;

            }
        }

        base.Execute(requester, character, target);

        List<Character> allCharactersInCommand = character.CharactersInCommand;

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        List<Character> unavailableCharacters = new List<Character>();
        foreach (Character x in allCharactersInCommand)
        {
            reason = null;
            if (!CanDoAction(requester, x, target, out reason))
            {
                unavailableCharacters.Add(x);
            }
        }

        allCharactersInCommand.RemoveAll(x => unavailableCharacters.Contains(x));

        if (FailureResult != null && !RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        if(Task == null)
        {
            return;
        }

        if(LocationTrait == null && !RandomLocation && !FriendlyLocation) // Common state (performance shortcut)
        {
            if (target.GetType() == typeof(LocationEntity))
            {
                character.GoToLocation((LocationEntity)target);
                allCharactersInCommand.ForEach((x)=>x.GoToLocation((LocationEntity)target));
            }
            else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
            {
                character.GoToLocation(((PortraitUI)target).CurrentCharacter.CurrentLocation);
                allCharactersInCommand.ForEach((x) => x.GoToLocation(((PortraitUI)target).CurrentCharacter.CurrentLocation));
            }
        }
        else
        {
            List<LocationEntity> potentialLocations = new List<LocationEntity>();
            potentialLocations.AddRange(CORE.Instance.Locations);

            if(LocationTrait != null)
            {
                potentialLocations.RemoveAll(x => !x.Traits.Contains(LocationTrait));
            }

            if(FriendlyLocation)
            {
                potentialLocations.RemoveAll(x => x.OwnerCharacter == null || x.OwnerCharacter.TopEmployer != character.TopEmployer);
            }

            if(RandomLocation)
            {
                character.GoToLocation(potentialLocations[Random.Range(0,potentialLocations.Count)]);
                allCharactersInCommand.ForEach((x) => x.GoToLocation(potentialLocations[Random.Range(0, potentialLocations.Count)]));
            }
            else
            {
                if (target.GetType() == typeof(LocationEntity))
                {
                    character.GoToLocation((LocationEntity)target);
                    allCharactersInCommand.ForEach((x) => x.GoToLocation((LocationEntity)target));
                }
                else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
                {
                    character.GoToLocation(((PortraitUI)target).CurrentCharacter.CurrentLocation);
                    allCharactersInCommand.ForEach((x) => x.GoToLocation(((PortraitUI)target).CurrentCharacter.CurrentLocation));
                }
            }
        }

        if (target.GetType() == typeof(LocationEntity))
        {
            target = character.CurrentLocation;
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, (LocationEntity)target, null, -1, null, this);
            allCharactersInCommand.ForEach((x) => CORE.Instance.GenerateLongTermTask(this.Task, requester, x, (LocationEntity)target, null, -1, null, this));

        }
        else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetChar = ((PortraitUI)target).CurrentCharacter;
            CORE.Instance.GenerateLongTermTask(this.Task, requester, character, targetChar.CurrentLocation, targetChar, -1, null, this);
            allCharactersInCommand.ForEach((x) => CORE.Instance.GenerateLongTermTask(this.Task, requester, x, targetChar.CurrentLocation, targetChar, -1, null, this));
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (DoneOnlyByEmployer)
        {
            if (requester != character && requester != CORE.Instance.Database.GOD && character.TopEmployer != requester)
            {
                return false;
            }
        }

       

        return true;
    }
}
