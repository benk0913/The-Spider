using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionAI", menuName = "DataObjects/FactionAI", order = 2)]
public class FactionAI : ScriptableObject
{
    Character CurrentCharacter;

    Dictionary<string, int> FailReasons = new Dictionary<string, int>();

    public virtual void MakeDecision(Character character)
    {
        this.CurrentCharacter = character;
        FailReasons.Clear();

        Expand();

        IOrderedEnumerable<KeyValuePair<string, int>> pairs =
                            from pair in FailReasons
                            orderby pair.Value ascending
                            select pair;

        FailReasons = pairs.ToDictionary(pair => pair.Key, pair => pair.Value);

        ResolveFailures();

        foreach(string reason in FailReasons.Keys)
        {
            Debug.Log("FAIL REASONS -" + reason + " = " + FailReasons[reason]);
        }
    }

    public virtual void Expand()
    {
        FailReason failReason = null;

        List<LocationEntity> ownedProperties = CurrentCharacter.PropertiesInCommand;
        foreach (LocationEntity location in ownedProperties)
        {
            if(location.EmployeesCharacters.Count < location.CurrentProperty.PropertyLevels[location.Level-1].MaxEmployees)
            {
                failReason = location.RecruitEmployee(CurrentCharacter);

                if(failReason != null)
                {
                    AddFailure(failReason);
                    break;
                }
            }
        }


        failReason = null;

        List<Character> charsInCommand = CurrentCharacter.CharactersInCommand;
        List<LocationEntity> purchasables = CORE.Instance.Locations.FindAll((LocationEntity location) =>
        {
            return location.IsBuyable;
        });
        foreach (LocationEntity purchasable in purchasables)
        {
            if (charsInCommand.Count == 0)
            {
                break;
            }

            failReason = purchasable.PurchasePlot(CurrentCharacter, charsInCommand[UnityEngine.Random.Range(0, charsInCommand.Count)]);
        }

        if(failReason != null)
        {
            AddFailure(failReason);
        }
    }

    #region Problem Solving

    public virtual void AddFailure(FailReason reason)
    {
        if(!FailReasons.ContainsKey(reason.Key))
        {
            FailReasons.Add(reason.Key,0);
        }

        FailReasons[reason.Key]++;
    }

    void ResolveFailures()
    {
        for(int i=0;i<FailReasons.Keys.Count;i++)
        {
            ResolveFailure(FailReasons.Keys.ElementAt(i));
        }
    }

    void ResolveFailure(string key)
    {
        switch(key)
        {
            case "Not Enough Gold":
                {
                    AttemptMaximizeGold();
                    break;
                }
            case "Not Enough Connections":
                {
                    AttemptMaximizeConnections();
                    break;
                }
            case "Not Enough Rumors":
                {
                    AttemptMaximizeRumors();
                    break;
                }
        }
    }

    private void AttemptMaximizeRumors()
    {
        List<LocationEntity> locationsOwned = CurrentCharacter.PropertiesInCommand;

        foreach(LocationEntity location in locationsOwned)
        {
            if(location.CurrentAction.RumorsGenerated > 0)
            {
                continue; // Already Generated
            }

            int maxActions = location.CurrentProperty.PropertyLevels[location.Level - 1].MaxActions;
            bool foundAction = false;
            for(int i=0;i<maxActions;i++)
            {
                if(i >= location.CurrentProperty.Actions.Count)
                {
                    break; // No available action
                }

                if (location.CurrentProperty.Actions[i].RumorsGenerated > location.CurrentAction.RumorsGenerated)
                {
                    location.SelectAction(CurrentCharacter, location.CurrentProperty.Actions[i]);
                    foundAction = true;
                    break; // Found a more valuable action
                }
            }

            if (!foundAction)
            {
                foreach (Property possibleProperty in CurrentCharacter.CurrentFaction.FactionProperties) // Find rebrand option
                {
                    if (possibleProperty.PlotType != location.CurrentProperty.PlotType)
                    {
                        continue; //Unfit plot type
                    }

                    maxActions = possibleProperty.PropertyLevels[0].MaxActions;
                    for (int i = 0; i < maxActions; i++)
                    {
                        if (i >= possibleProperty.Actions.Count)
                        {
                            break;
                        }

                        if (possibleProperty.Actions[i].RumorsGenerated > location.CurrentAction.RumorsGenerated)
                        {
                            location.Rebrand(CurrentCharacter, possibleProperty);
                            location.SelectAction(CurrentCharacter, possibleProperty.Actions[i]);
                            break;
                        }
                    }
                }
            }

        }

    }

    private void AttemptMaximizeConnections()
    {
        List<LocationEntity> locationsOwned = CurrentCharacter.PropertiesInCommand;

        foreach (LocationEntity location in locationsOwned)
        {
            if (location.CurrentAction.ConnectionsGenerated > 0)
            {
                continue; // Already Generated
            }

            int maxActions = location.CurrentProperty.PropertyLevels[location.Level - 1].MaxActions;
            bool foundAction = false;
            for (int i = 0; i < maxActions; i++) //Find better action option
            {
                if (i >= location.CurrentProperty.Actions.Count)
                {
                    break; // No available action
                }

                if (location.CurrentProperty.Actions[i].ConnectionsGenerated > location.CurrentAction.ConnectionsGenerated)
                {
                    location.SelectAction(CurrentCharacter, location.CurrentProperty.Actions[i]);
                    foundAction = true;
                    break; // Found a more valuable action
                }
            }

            if(!foundAction)
            {
                foreach(Property possibleProperty in CurrentCharacter.CurrentFaction.FactionProperties) // Find rebrand option
                {
                    if (possibleProperty.PlotType != location.CurrentProperty.PlotType)
                    {
                        continue; //Unfit plot type
                    }

                    maxActions = possibleProperty.PropertyLevels[0].MaxActions;
                    for(int i=0;i<maxActions;i++)
                    {
                        if(i >= possibleProperty.Actions.Count)
                        {
                            break;
                        }

                        if(possibleProperty.Actions[i].ConnectionsGenerated > location.CurrentAction.ConnectionsGenerated)
                        {
                            location.Rebrand(CurrentCharacter, possibleProperty);
                            location.SelectAction(CurrentCharacter, possibleProperty.Actions[i]);
                            break;
                        }
                    }
                }
            }

        }
    }

    private void AttemptMaximizeGold()
    {
        List<LocationEntity> locationsOwned = CurrentCharacter.PropertiesInCommand;

        foreach (LocationEntity location in locationsOwned)
        {
            if (location.CurrentAction.GoldGenerated > 0)
            {
                continue; // Already Generated
            }

            int maxActions = location.CurrentProperty.PropertyLevels[location.Level - 1].MaxActions;
            bool foundAction = false;
            for (int i = 0; i < maxActions; i++)
            {
                if (i >= location.CurrentProperty.Actions.Count)
                {
                    break; // No available action
                }

                if (location.CurrentProperty.Actions[i].GoldGenerated > location.CurrentAction.GoldGenerated)
                {
                    foundAction = true;
                    location.SelectAction(CurrentCharacter, location.CurrentProperty.Actions[i]);
                    break; // Found a more valuable action
                }
            }


            if (!foundAction)
            {
                foreach (Property possibleProperty in CurrentCharacter.CurrentFaction.FactionProperties) // Find rebrand option
                {
                    if (possibleProperty.PlotType != location.CurrentProperty.PlotType)
                    {
                        continue; //Unfit plot type
                    }

                    maxActions = possibleProperty.PropertyLevels[0].MaxActions;
                    for (int i = 0; i < maxActions; i++)
                    {
                        if (i >= possibleProperty.Actions.Count)
                        {
                            break;
                        }

                        if (possibleProperty.Actions[i].GoldGenerated > location.CurrentAction.GoldGenerated)
                        {
                            location.Rebrand(CurrentCharacter, possibleProperty);
                            location.SelectAction(CurrentCharacter, possibleProperty.Actions[i]);
                            break;
                        }
                    }
                }
            }
        }
    }


    #endregion

    #region Tier1

    public virtual FailReason BuyProperty(Character character, LocationEntity targetPlot)
    {
        List<PlayerAction> PlayerActions = targetPlot.GetPossiblePlayerActions();

        PlayerAction buyPropertyAction = PlayerActions.Find((PlayerAction action) => { return action.name == "Buy Property"; });

        FailReason reason = null;
        if (buyPropertyAction.CanDoAction(character, targetPlot, out reason))
        {
            buyPropertyAction.Execute(CurrentCharacter, targetPlot);
        }

        return reason;
    }

    #endregion
}
