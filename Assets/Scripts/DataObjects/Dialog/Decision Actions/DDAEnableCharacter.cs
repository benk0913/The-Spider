using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAEnableCharacter", menuName = "DataObjects/Dialog/Actions/DDAEnableCharacter", order = 2)]
public class DDAEnableCharacter : DialogDecisionAction
{
    public string CharacterID;

    public bool ActiveState = true;

    public override void Activate()
    {
        Character character = CORE.Instance.Characters.Find(x => x.ID == CharacterID);

        
        if(character == null)
        {
            Debug.LogError("Couldn't find character! "+ CharacterID);
            return;
        }

        if(ActiveState)
        {
            character.EnableCharacter();
        }
        else
        {
            character.DisableCharacter();
        }
    }
}
