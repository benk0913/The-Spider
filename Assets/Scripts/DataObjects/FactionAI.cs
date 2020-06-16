using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionAI", menuName = "DataObjects/FactionAI", order = 2)]
public class FactionAI : ScriptableObject
{
    public const int GOLD_SCARCE_VALUE = 150;


    protected Character CurrentCharacter;

    protected Dictionary<string, int> FailReasons = new Dictionary<string, int>();

    [SerializeField]
    public AIAgression AgressionType = AIAgression.Normal;


    public virtual void MakeDecision(Character character)
    {
        this.CurrentCharacter = character;

        if(character.PropertiesOwned.Count == 0)
        {
            return;
        }

        if(!character.PropertiesOwned[0].gameObject.activeInHierarchy)
        {
            return;
        }

        FailReasons.Clear();

        BotCheats();
        
        Agression();

        Expand();

        IOrderedEnumerable<KeyValuePair<string, int>> pairs =
                            from pair in FailReasons
                            orderby pair.Value ascending
                            select pair;

        FailReasons = pairs.ToDictionary(pair => pair.Key, pair => pair.Value);

        ResolveFailures();

        if (CORE.Instance.DEBUG)
        {
            foreach (string reason in FailReasons.Keys)
            {
                Debug.Log("FAIL REASONS -" + reason + " = " + FailReasons[reason]);
            }
        }
    }

    protected virtual void BotCheats()
    {
        //this.CurrentCharacter.CGold += 1;
        //this.CurrentCharacter.CRumors += 1;
        //this.CurrentCharacter.CConnections += 1;
    }

    public virtual void Expand()
    {
        FailReason failReason = null;

        //Recruit employees
        AttemptMaximizeEmployees();
        
        //Recruit Guards
        AttemptMaximizeGuards();

        //Buy Locations
        AttemptMaximizeProperties();

        AttemptMaintainance();
    }

    public virtual void Agression()
    {
        if(AgressionType == AIAgression.Passive)
        {
            return;
        }

        Faction factionToAttack = FindPotentialAgressionTarget();
        
        if(factionToAttack == null)
        {
            return;
        }

        //Assassinate / Abduction / Arson / Raid / Assault
        List<SchemeType> PotentialSchemes = new List<SchemeType>();

        PotentialSchemes.AddRange(CORE.Instance.Database.Schemes);
        PotentialSchemes.RemoveAll(x => x.name == "Liberate");

        if(PotentialSchemes.Count == 0)
        {
            return;
        }

        AttemptPlot(PotentialSchemes[UnityEngine.Random.Range(0, PotentialSchemes.Count)], factionToAttack);
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

    protected virtual void ResolveFailures()
    {
        for(int i=0;i<FailReasons.Keys.Count;i++)
        {
            ResolveFailure(FailReasons.Keys.ElementAt(i));
        }
    }

    protected virtual void ResolveFailure(string key)
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
            case "Not Enough Agents":
                {
                    AttemptRecruitAgents();
                    break;
                }
            case "Location Is Full Of Employees":
                {
                    break;
                }
            case "Location Is Full Of Agents":
                {
                    AttemptMaximizeAgentsSlots();
                    break;
                }
            case "Not Enough Agent Properties":
                {
                    AttemptMaximizeAgentProperties();
                    break;
                }
            case "Location Is Ruined":
                {
                    AttemptRepairLocations();
                    break;
                }

        }
    }

    protected virtual void AttemptMaximizeRumors()
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
                return; //TODO PROPER REBRAND
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

                        if ((possibleProperty.Actions[i].RumorsGenerated * possibleProperty.PropertyLevels[0].MaxEmployees)
                           >
                           (location.CurrentAction.RumorsGenerated * location.CurrentProperty.PropertyLevels[0].MaxEmployees))
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

    protected virtual void AttemptMaximizeConnections()
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
                return; //TODO PROPER REBRAND
                foreach (Property possibleProperty in CurrentCharacter.CurrentFaction.FactionProperties) // Find rebrand option
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

                        if ((possibleProperty.Actions[i].ConnectionsGenerated * possibleProperty.PropertyLevels[0].MaxEmployees)
                            >
                            (location.CurrentAction.ConnectionsGenerated * location.CurrentProperty.PropertyLevels[0].MaxEmployees))
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

    protected virtual void AttemptMaximizeGold()
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
                return; //TODO PROPER REBRAND
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

                        if ((possibleProperty.Actions[i].GoldGenerated * possibleProperty.PropertyLevels[0].MaxEmployees) 
                            >
                            (location.CurrentAction.GoldGenerated * location.CurrentProperty.PropertyLevels[0].MaxEmployees))
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

    protected virtual void AttemptMaximizeEmployees()
    {
        FailReason failReason;

        List<LocationEntity> ownedProperties = CurrentCharacter.PropertiesInCommand;
        foreach (LocationEntity location in ownedProperties)
        {
            if (location.EmployeesCharacters.Count < location.CurrentProperty.PropertyLevels[location.Level - 1].MaxEmployees)
            {
                failReason = location.RecruitEmployee(CurrentCharacter);

                if (failReason != null)
                {
                    AddFailure(failReason);
                    break;
                }
            }
        }
    }

    protected virtual void AttemptMaximizeGuards()
    {
        FailReason failReason;

        List<LocationEntity> ownedProperties = CurrentCharacter.PropertiesInCommand;
        foreach (LocationEntity location in ownedProperties)
        {
            if (location.GuardsCharacters.Count < location.CurrentProperty.PropertyLevels[location.Level - 1].MaxGuards)
            {
                failReason = location.RecruitEmployee(CurrentCharacter, true);

                if (failReason != null)
                {
                    AddFailure(failReason);
                    break;
                }
            }
        }
    }

    protected virtual void AttemptMaximizeProperties()
    {
        if(CurrentCharacter.PropertiesInCommand.Count >= CurrentCharacter.CurrentFaction.RecommendedPropertyCap)
        {
            return;
        }

        FailReason failReason = null;

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
            
            if(failReason == null)
            {
                return;
            }
        }

        if (failReason != null)
        {
            AddFailure(failReason);
        }
    }

    protected virtual void AttemptRecruitAgents()
    {
        //Attempt Recruit current properties.
        List<LocationEntity> properties = CurrentCharacter.PropertiesInCommand;

        LocationEntity potentialProperty = properties.Find(x => x.CurrentProperty.EmployeesAreAgents && x.CurrentProperty.PropertyLevels[x.Level - 1].MaxEmployees > x.EmployeesCharacters.Count);

        if(potentialProperty != null)
        {
            FailReason failReason = potentialProperty.RecruitEmployee(CurrentCharacter);

            if (failReason != null)
            {
                AddFailure(failReason);
                return;
            }
        }

    }

    protected virtual void AttemptMaximizeAgentsSlots()
    {
        List<LocationEntity> properties = CurrentCharacter.PropertiesInCommand;
        properties.RemoveAll(x => !x.CurrentProperty.EmployeesAreAgents);//Not agent properties
        properties.RemoveAll(x => x.CurrentProperty.PropertyLevels.Count == x.Level); // Max Level Properties
        properties.RemoveAll(x => x.CurrentProperty.PropertyLevels[x.Level - 1 + 1].UpgradePrice > 99); //Upgrade to expensive properties

        FailReason failReason;
        if (properties.Count == 0)
        {
            failReason = new FailReason("Not Enough Agent Properties");
            AddFailure(failReason);
            return;
        }

        failReason = UpgradeProperty(properties[0]);

        if(failReason != null)
        {
            AddFailure(failReason);
            return;
        }
        
    }

    protected virtual void AttemptMaximizeAgentProperties()
    {
        return; //TODO PROPER REBRAND

        if (CurrentCharacter.CGold <= GOLD_SCARCE_VALUE) //In scarcity we shall not bother the system.
        {
            return;
        }

        List<LocationEntity> potentialProperties = CurrentCharacter.PropertiesInCommand;
        potentialProperties.RemoveAll(x => x.CurrentProperty.EmployeesAreAgents);

        if (potentialProperties.Count == 0)
        {
            return;
        }

        LocationEntity chosenEntity = potentialProperties[UnityEngine.Random.Range(0, potentialProperties.Count)];
        Property potentialRebrandTarget = CORE.Instance.Database.Properties.Find(x =>
                x.PlotType != CORE.Instance.Database.UniquePlotType
                && x.PlotType == chosenEntity.CurrentProperty.PlotType
                && x.EmployeesAreAgents);

        if(potentialRebrandTarget == null)
        {
            return;
        }

        FailReason failReason = chosenEntity.Rebrand(CurrentCharacter, potentialRebrandTarget);

        if(failReason != null)
        {
            AddFailure(failReason);
            return;
        }

    }

    protected virtual void AttemptRepairLocations()
    {
        List<LocationEntity> properties = CurrentCharacter.PropertiesInCommand;
        properties.RemoveAll(x => !x.IsRuined);//Not ruined

        FailReason failReason = null;
        foreach(LocationEntity property in properties)
        {
            failReason = property.RepairRuins(CurrentCharacter);

            if(failReason != null)
            {
                AddFailure(failReason);
                return;
            }
        }
    }

    protected virtual void AttemptMaintainance()
    {

        List<LocationEntity> locations = CurrentCharacter.PropertiesInCommand;

        foreach (LocationEntity location in locations)
        {
            if (location.CurrentProperty == location.CurrentProperty.PlotType.BaseProperty)
            {
                List<Property> availableProperties = new List<Property>();
                availableProperties.AddRange(CurrentCharacter.CurrentFaction.FactionProperties);
                availableProperties.RemoveAll(x => x.PlotType != location.CurrentProperty.PlotType);

                if(availableProperties.Count == 0)
                {
                    continue;
                }

                location.Rebrand(CurrentCharacter, availableProperties[UnityEngine.Random.Range(0, availableProperties.Count)]);
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

    public virtual FailReason UpgradeProperty(LocationEntity property)
    {
        return property.PurchaseUpgrade(CurrentCharacter);
    }

    public virtual Faction FindPotentialAgressionTarget()
    {
        Faction cFaction = CurrentCharacter.CurrentFaction;

        List<FactionRelationInstance> badRelations;

        if (AgressionType == AIAgression.Normal)
        {
            badRelations = cFaction.Relations.Relations.FindAll(x => x.WithFaction != x.OfFaction && x.TotalValue < -3);
        }
        else if (AgressionType == AIAgression.Agressive)
        {
            badRelations = cFaction.Relations.Relations.FindAll(x => x.WithFaction != x.OfFaction);
        }
        else
        {
            badRelations = new List<FactionRelationInstance>();
        }

        if (badRelations.Count == 0)
        {
            return null;
        }

        return badRelations[UnityEngine.Random.Range(0, badRelations.Count)].WithFaction;
    }

    public virtual FailReason AttemptPlot(SchemeType currentSchemeType, Faction againstFaction)
    {
        //Find Target
        if (againstFaction.FactionHead == null)
        {
            return null;
        }

        if(againstFaction.name == CurrentCharacter.CurrentFaction.name)
        {
            return null;
        }

        AgentInteractable plotTarget = null;

        Character factionHead = CORE.Instance.Characters.Find(x => x.name == againstFaction.FactionHead.name);
        
        if(factionHead == null)
        {
            return null;
        }

        if(currentSchemeType.TargetIsLocation)
        {
            List<LocationEntity> possibleLocationTargets = factionHead.PropertiesInCommand.FindAll(x => !x.IsRuined);
            if(possibleLocationTargets.Count == 0)
            {
                return null;
            }

            possibleLocationTargets = possibleLocationTargets.OrderBy(x => x.GuardsCharacters.Count).ToList();
            plotTarget = possibleLocationTargets[0];
        }
        else
        {
            List<Character> possibleCharacters = factionHead.CharactersInCommand.FindAll(x=>!x.IsDead && !x.IsInTrouble && x.PrisonLocation == null && !x.IsInHiding);
            if (possibleCharacters.Count == 0)
            {
                return null;
            }

            possibleCharacters = possibleCharacters.OrderBy(x => x.Rank).ToList();

            PortraitUI portrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI").GetComponent<PortraitUI>();
            portrait.transform.position = new Vector3(9999, 9999, 9999);
            portrait.SetCharacter(possibleCharacters[0]);
            plotTarget = portrait;
        }




        //Find Plotter
        List<Character> potentialPlotters = CurrentCharacter.CharactersInCommand;
        potentialPlotters.RemoveAll(x => !x.IsAgent || x.PrisonLocation != null || x.IsPuppetOf(againstFaction) || x.IsDead);
        
        if(potentialPlotters.Count == 0)
        {
            return new FailReason("Not Enough Agents");
        }

        Character plotter = potentialPlotters[UnityEngine.Random.Range(0, potentialPlotters.Count)];

        //Find Entry
        List<PlotEntry> possibleEntries = new List<PlotEntry>();
        possibleEntries.AddRange(currentSchemeType.PossibleEntries);
        possibleEntries.RemoveAll(x => x.AreRequirementsMet(CurrentCharacter, plotTarget) == null);
        if(possibleEntries.Count == 0)
        {
            Debug.LogError("BOT HAS NO POSSIBLE ENTRY!");

            return null;
        }
        PlotEntry randomEntry = possibleEntries[UnityEngine.Random.Range(0, possibleEntries.Count)];

        //Find Method
        PlotMethod randomMethod = currentSchemeType.PossibleMethods[UnityEngine.Random.Range(0, currentSchemeType.PossibleMethods.Count)];


        //Gather Participants
        List<Character> participants = new List<Character>();
        List<Character> potentialParticipants = CurrentCharacter.CharactersInCommand.FindAll(
            x => x.IsAgent
            && x.Age > 15
            && !participants.Contains(x)
            && x.PrisonLocation == null
            && !x.IsPuppetOf(factionHead.CurrentFaction)
            && !x.IsDead 
            && (x.CurrentTaskEntity == null || x.CurrentTaskEntity.CurrentTask.Cancelable));

        potentialParticipants.OrderByDescending(x => x.GetBonus(randomMethod.OffenseSkill).Value);

        for (int i=0;i<randomMethod.MinimumParticipants;i++)
        {
            if(potentialParticipants.Count == 0)
            {
                return new FailReason("Not Enough Agents");
            }

            participants.Add(potentialParticipants[0]);
            potentialParticipants.RemoveAt(0);
        }

        for (int i = randomMethod.MinimumParticipants; i < randomMethod.MaximumParticipants; i++)
        {
            if (potentialParticipants.Count == 0)
            {
                break;
            }

            participants.Add(potentialParticipants[0]);
            potentialParticipants.RemoveAt(0);
        }



        //Gather target participants
        List<Character> targetParticipants = new List<Character>();
        if (plotTarget.GetType() == typeof(PortraitUI) || plotTarget.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetCharacter = ((PortraitUI)plotTarget).CurrentCharacter;

            if (targetCharacter.PrisonLocation == null && targetCharacter.Age >= 15 && !targetCharacter.IsInTrouble)
            {
                targetParticipants.Add(targetCharacter);
            }

            targetParticipants.AddRange(targetCharacter.GuardsInCommand.FindAll(x =>
           x.PrisonLocation == null
           && !x.IsPuppetOf(CurrentCharacter.CurrentFaction)
           && !x.IsInTrouble));
        }
        else if (plotTarget.GetType() == typeof(LocationEntity))
        {
            LocationEntity location = ((LocationEntity)plotTarget);

            if (location.OwnerCharacter != null)
            {
                targetParticipants.AddRange(CORE.Instance.Characters.FindAll(x =>
                    !targetParticipants.Contains(x)
                    && x.PrisonLocation == null
                    && !x.IsPuppetOf(CurrentCharacter.CurrentFaction)
                    && x.CurrentLocation == location
                    && x.Age >= 15
                    && (x.TopEmployer == location.OwnerCharacter.TopEmployer || x.CurrentFaction.name == "Constabulary")));
            }
        }



        PlotData Plot = new PlotData(currentSchemeType.name,CurrentCharacter, plotter, participants, targetParticipants, plotTarget, randomMethod, randomEntry);

        Plot.BaseMethod = currentSchemeType.BaseMethod;

        //if ((CurrentCharacter.Gold - 30) < GOLD_SCARCE_VALUE/2)
        //{
        //    return new FailReason("Not Enough Gold");
        //}

        //CurrentCharacter.Gold -= 30;

        currentSchemeType.Execute(Plot);


        if(plotTarget.GetType() == typeof(PortraitUI) || plotTarget.GetType() == typeof(PortraitUIEmployee))
        {
            plotTarget.gameObject.SetActive(false);
        }

        return null;
    }

    #endregion
}

public enum AIAgression
{
    Passive,
    Normal,
    Agressive,
}
