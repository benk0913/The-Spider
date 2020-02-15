using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDARemoveCharacter", menuName = "DataObjects/Dialog/Actions/DDARemoveCharacter", order = 2)]
public class DDARemoveCharacter : DialogDecisionAction
{
    [SerializeField]
    Character CharacterToRemove;

    public override void Activate()
    {
        Character character = CORE.Instance.Characters.Find(x => x.name == CharacterToRemove.name);

        if(character == null)
        {
            Debug.LogError("Couldn't find character - " + CharacterToRemove.name);
            return;
        }

        character.Death(false,true);
    }
}
