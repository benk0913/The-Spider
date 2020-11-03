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

    public override void Activate()
    {
        Character actor = CORE.Instance.Characters.Find(x => x.name == TargetCharacter.name);
        actor.AddTrait(TraitToGain);
    }
}
