using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDACharacterGainsTrait", menuName = "DataObjects/Dialog/Actions/DDACharacterGainsTrait", order = 2)]
public class DDACharacterGainsTrait : DialogDecisionAction
{
    [SerializeField]
    Trait TraitToGain;

    [SerializeField]
    Character TargetCharacter;

    public bool RandomCharacter;

    public override void Activate()
    {
        Character actor;

        if (RandomCharacter)
        {
            actor = CORE.Instance.Characters.Find(X => !X.IsDisabled && !X.HiddenFromCharacterWindows && X.TopEmployer != X && !X.Traits.Contains(TraitToGain));
            if(actor == null)
            {
                return;
            }
            actor.AddTrait(TraitToGain);
            return;
        }

        actor = CORE.Instance.Characters.Find(x => x.name == TargetCharacter.name);
        actor.AddTrait(TraitToGain);
    }
}
