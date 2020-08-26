using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAEmployeesRelation", menuName = "DataObjects/Dialog/Actions/DDAEmployeesRelation", order = 2)]
public class DDAEmployeesRelation : DialogDecisionAction
{
    public Property TheProperty;
    public Faction OfFaction;
    public string ModifierMessage = "Likes you for some reason...";
    public int ModifierValue = 1;
    public int ModifierTurnLength = 5;

    public override void Activate()
    {

        LocationEntity targetLocation = null;
        if (TheProperty != null && OfFaction != null)
        {
            targetLocation = CORE.Instance.Locations.Find(X => X.CurrentProperty == TheProperty && X.FactionInControl != null && X.FactionInControl.name == OfFaction.name);
        }
        else if(DialogWindowUI.Instance.GetDialogParameter("Location") != null)
        {
            targetLocation = (LocationEntity)DialogWindowUI.Instance.GetDialogParameter("Location");
        }

        if (targetLocation == null)
        {
            Debug.LogError("NO RELEVANT LOCATION FOR " + this.name);
            return;
        }

        if(targetLocation.EmployeesCharacters.Count == 0)
        {
            return;
        }

        List<Character> targetCHars = new List<Character>();
        targetCHars.AddRange( targetLocation.EmployeesCharacters);

        if(targetLocation.OwnerCharacter != null)
            targetCHars.Add(targetLocation.OwnerCharacter);

        foreach(Character character in targetCHars)
        {
            character.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(new RelationsModifier(ModifierMessage, ModifierValue), ModifierTurnLength, CORE.PC));
        }
    }
}
