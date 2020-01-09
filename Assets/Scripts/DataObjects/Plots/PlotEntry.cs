using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plot Entry", menuName = "DataObjects/Plots/Plot Entry", order = 2)]
public class PlotEntry : ScriptableObject
{
    [TextArea(3,6)]
    public string Description;
    public Sprite Icon;
    public Item[] ItemsRequired;
    public Trait[] TraitsRequired;
    public BonusType Skill;
    public int BonusToSkill;
    public List<PlotType> PlotTypeRequired;

    public FailReason AreRequirementsMet(Character requester, AgentInteractable target)
    {
        if(target.GetType() == typeof(LocationEntity))
        {
            LocationEntity locationTarget = (LocationEntity)target;

            if(!PlotTypeRequired.Contains(locationTarget.CurrentProperty.PlotType))
            {
                return new FailReason("Unavailable for " + locationTarget.CurrentProperty.PlotType + " plot type");
            }

            foreach (Trait trait in TraitsRequired)
            {
                if (locationTarget.Traits.Find(x => x.name == trait.name) == null)
                {
                    return new FailReason("Target Requires The Trait: " + trait.name);
                }
            }
        }
        else if (target.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            Character characterTarget = ((PortraitUI)target).CurrentCharacter;

            foreach (Trait trait in TraitsRequired)
            {
                if (characterTarget.Traits.Find(x => x.name == trait.name) == null)
                {
                    return new FailReason("Target Requires The Trait: " + trait.name);
                }
            }
        }

        foreach (Item item in ItemsRequired)
        {
            if (requester.GetItem(item.name) == null)
            {
                return new FailReason("Requires The Item: " + item.name);
            }
        }

        return null;
    }
}
