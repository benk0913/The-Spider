using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plot Method", menuName = "DataObjects/Plots/Plot Method", order = 2)]
public class PlotMethod : ScriptableObject
{
    [TextArea(3,6)]
    public string Description;
    public Sprite Icon;
    public Item[] ItemsRequired;
    public BonusType OffenseSkill;
    public BonusType DefenceSkill;
    public int MinimumSkillRequired;
    public int MinimumParticipants;
    public int MaximumParticipants;



    public FailReason AreRequirementsMet(Character[] Participants)
    {
        if (Participants.Length < MinimumParticipants)
        {
            return new FailReason("Not Enough Participants",MinimumParticipants);
        }

        if (Participants.Length > MaximumParticipants)
        {
            return new FailReason("Too Much Participants",MaximumParticipants);
        }

        foreach(Character character in Participants)
        {
            if (character.GetBonus(OffenseSkill).Value < MinimumSkillRequired)
            {
                return new FailReason(character.name + " Is Not Skilled Enough.",MinimumSkillRequired);
            }

            foreach (Item item in ItemsRequired)
            {
                if (character.TopEmployer.GetItem(item.name) == null)
                {
                    return new FailReason(" Requires: " + item.name+" for each participant");
                }
            }
        }


        return null;
    }

}
