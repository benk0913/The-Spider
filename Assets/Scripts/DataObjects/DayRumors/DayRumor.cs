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
                    LocationEntity property = CORE.PC.PropertiesInCommand.Find(x => x.OwnerCharacter != CORE.PC && x.CurrentProperty.PropertyLevels.Count > x.Level);

                    if (property == null)
                    {
                        break;
                    }

                    Character agent = property.OwnerCharacter;

                    popup = new PopupData(Preset, new List<Character> { agent }, new List<Character> { CORE.PC });

                    break;
                }
            case DayRumorType.MorePeople:
                {
                    LocationEntity property = CORE.PC.PropertiesInCommand.Find(x => x.OwnerCharacter != CORE.PC && x.EmployeesCharacters.Count < x.CurrentProperty.PropertyLevels[x.Level-1].MaxEmployees);

                    if (property == null)
                    {
                        break;
                    }

                    Character agent = property.OwnerCharacter;

                    popup = new PopupData(Preset, new List<Character> { agent }, new List<Character> { CORE.PC });

                    break;
                }
            case DayRumorType.MoreGuards:
                {
                    LocationEntity property = CORE.PC.PropertiesInCommand.Find(x => x.OwnerCharacter != CORE.PC && x.GuardsCharacters.Count < x.CurrentProperty.PropertyLevels[x.Level - 1].MaxGuards);

                    if (property == null)
                    {
                        break;
                    }

                    Character agent = property.OwnerCharacter;

                    popup = new PopupData(Preset, new List<Character> { agent }, new List<Character> { CORE.PC });

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
                    && X.PropertiesInCommand.Count > minToBetray);

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
                    && X.GetRelationsWith(CORE.PC) < -7);

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
                    && X.GetRelationsWith(CORE.PC) > 9);

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

                    Character tellingAgent = CORE.PC.RandomAgent;
                    Character targetAgent = CORE.PC.CharactersInCommand.Find(X => X.IsAgent
                    && X != tellingAgent
                    && X.CProgress > 35);

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
        MoreGuards
    }
}
