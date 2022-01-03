using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DayRumor", menuName = "DataObjects/DayRumors/DayRumor", order = 2)]
public class DayRumor : ScriptableObject
{
    public PopupDataPreset Preset;

    public DayRumorType RumorType;

    public PopupData GetPopup()
    {
        PopupData popup = null;

        switch (RumorType)
        {
            case DayRumorType.MissingPrisoner:
                {
                    LocationEntity property = CORE.PC.PropertiesInCommand.Find(x => x.PrisonersCharacters.Count > 0);

                    if (property == null)
                    {
                        break;
                    }

                    if (property.PrisonersCharacters.Count <= 0)
                    {
                        break;
                    }

                    Character agent = CORE.PC.RandomAgent;

                    Character prisoner = property.PrisonersCharacters[Random.Range(0, property.PrisonersCharacters.Count)];

                    if (agent == null)
                    {
                        break;
                    }

                    popup = new PopupData(Preset, new List<Character> { agent }, new List<Character> { prisoner });

                    break;
                }
            case DayRumorType.InvestmentRequest:
                {
                    LocationEntity property = CORE.PC.PropertiesInCommand.Find(x => x.OwnerCharacter != CORE.PC && x != CORE.PC && x.CurrentProperty.PropertyLevels.Count > x.Level && !x.IsUpgrading);

                    if (property == null)
                    {
                        break;
                    }

                    if (property.OwnerCharacter == null)
                    {
                        break;
                    }

                    Character agent = property.OwnerCharacter;

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("LocationName", property.Name);
                    parameters.Add("DistrictName", property.NearestDistrict == null ? "Glassden" : property.NearestDistrict.Name);

                    popup = new PopupData(Preset, new List<Character> { agent }, new List<Character> { CORE.PC },null,parameters);

                    break;
                }
            case DayRumorType.MorePeople:
                {
                    LocationEntity property = CORE.PC.PropertiesInCommand.Find(x => x.OwnerCharacter != CORE.PC && x != CORE.PC && x.EmployeesCharacters.Count < x.CurrentProperty.PropertyLevels[x.Level-1].MaxEmployees);

                    if (property == null)
                    {
                        break;
                    }

                    Character agent = property.OwnerCharacter;

                    if(agent == null)
                    {
                        break;
                    }

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("LocationName", property.Name);
                    parameters.Add("DistrictName", property.NearestDistrict == null ? "Glassden" : property.NearestDistrict.Name);

                    popup = new PopupData(Preset, new List<Character> { agent }, new List<Character> { CORE.PC },null, parameters);

                    break;
                }
            case DayRumorType.MoreGuards:
                {
                    if(!CORE.Instance.TechTree.Find(X=>X.name == "Street Gangs").IsResearched)
                    {
                        break;
                    }

                    LocationEntity property = CORE.PC.PropertiesInCommand.Find(x => 
                    x.OwnerCharacter != CORE.PC && x != CORE.PC && x.GuardsCharacters.Count < x.CurrentProperty.PropertyLevels[x.Level - 1].MaxGuards);

                    if (property == null)
                    {
                        break;
                    }

                    Character agent = property.OwnerCharacter;

                    if(agent == null)
                    {
                        break;
                    }

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("LocationName", property.Name);
                    parameters.Add("DistrictName", property.NearestDistrict == null? "Glassden" : property.NearestDistrict.Name);

                    popup = new PopupData(Preset, new List<Character> { agent }, new List<Character> { CORE.PC }, null, parameters);

                    break;
                }
            case DayRumorType.AgentTooPowerful:
                {
                    if (CORE.PC.CurrentFaction.HasPromotionSystem)
                    {
                        break;
                    }

                    int minToBetray = CORE.PC.PropertiesInCommand.Count / 2;

                    Character tellingAgent = CORE.PC.RandomAgent;
                    Character targetAgent = CORE.PC.CharactersInCommand.Find(X => X.IsAgent
                    && X != tellingAgent
                    && X.PropertiesInCommand.Count > minToBetray
                    && X != CORE.PC);

                    if (tellingAgent == null)
                    {
                        break;
                    }

                    if (targetAgent == null)
                    {
                        break;
                    }

                    popup = new PopupData(Preset, new List<Character> { tellingAgent }, new List<Character> { targetAgent });

                    break;
                }
            case DayRumorType.AgentHatesPlayer:
                {
                    Character tellingAgent = CORE.PC.RandomAgent;
                    Character targetAgent = CORE.PC.CharactersInCommand.Find(X => X.IsAgent
                    && X != tellingAgent
                    && X.GetRelationsWith(CORE.PC) < -7
                    && X != CORE.PC);

                    if (tellingAgent == null)
                    {
                        break;
                    }

                    if (targetAgent == null)
                    {
                        break;
                    }

                    popup = new PopupData(Preset, new List<Character> { tellingAgent }, new List<Character> { targetAgent });

                    break;
                }
            case DayRumorType.AgentLikesPlayer:
                {

                    Character tellingAgent = CORE.PC.RandomAgent;
                    Character targetAgent = CORE.PC.CharactersInCommand.Find(X => X.IsAgent
                    && X != tellingAgent
                    && X.GetRelationsWith(CORE.PC) > 9
                    && X != CORE.PC);

                    if (tellingAgent == null)
                    {
                        break;
                    }

                    if (targetAgent == null)
                    {
                        break;
                    }

                    popup = new PopupData(Preset, new List<Character> { tellingAgent }, new List<Character> { targetAgent });

                    break;
                }
            case DayRumorType.PromotionSoon:
                {
                    if(!CORE.PC.CurrentFaction.HasPromotionSystem)
                    {
                        break;
                    }

                    Character targetAgent = CORE.PC.CharactersInCommand.Find(X => X.IsAgent
                    && X != CORE.PC
                    && X.CProgress > 35);

                    if (targetAgent == null || targetAgent.Employer == null)
                    {
                        break;
                    }

                    popup = new PopupData(Preset, new List<Character> { targetAgent.Employer }, new List<Character> { targetAgent });

                    break;
                }
            case DayRumorType.AgentPrisoner:
                {
                    Character targetAgent = CORE.PC.CharactersInCommand.Find(X => X.IsAgent
                    && X != CORE.PC
                    && X.PrisonLocation != null);

                    if (targetAgent == null)
                    {
                        break;
                    }

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("LocationName", targetAgent.PrisonLocation.Name);
                    parameters.Add("DistrictName", targetAgent.PrisonLocation.NearestDistrict == null ? "Glassden" : targetAgent.PrisonLocation.NearestDistrict.Name);

                    popup = new PopupData(Preset, new List<Character> { targetAgent.Employer }, new List<Character> { targetAgent },null,parameters);

                    break;
                }
        }

        return popup;
    }

    public enum DayRumorType
    {
        MissingPrisoner,
        InvestmentRequest,
        AgentTooPowerful,
        AgentHatesPlayer,
        AgentLikesPlayer,
        PromotionSoon,
        MorePeople,
        MoreGuards,
        AgentPrisoner
    }
}
