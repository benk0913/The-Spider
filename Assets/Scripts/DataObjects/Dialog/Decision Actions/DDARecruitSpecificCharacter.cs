using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDARecruitSpecificCharacter", menuName = "DataObjects/Dialog/Actions/DDARecruitSpecificCharacter", order = 2)]
public class DDARecruitSpecificCharacter : DialogDecisionAction
{
    [SerializeField]
    Character SpecificCharacter;

    [SerializeField]
    Character ForTheCharacter;

    public override void Activate()
    {
        
        WarningWindowUI.Instance.Show(SpecificCharacter.name + " has joined your faction, and is now a controllable agent.",()=>
        {
            Character character = CORE.Instance.Characters.Find(x => x.name == SpecificCharacter.name);

            if (character == null)
            {
                Debug.LogError("Couldn't find character " + SpecificCharacter.name);
                return;
            }

            Character forCharacter = CORE.Instance.Characters.Find(x => x.name == ForTheCharacter.name);

            if (forCharacter == null)
            {
                Debug.LogError("Couldn't find character " + ForTheCharacter.name);
                return;
            }

            character.StartWorkingFor(forCharacter.PropertiesOwned[0]);
        });
    }
}
