using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "DataObjects/GameEvents/GameEvent", order = 2)]
public class GameEvent : ScriptableObject
{

    [TextArea(2, 3)]
    public string Description;

    public Sprite Icon;

    public float Chance;

    public LetterPreset LetterMessage;

    public virtual void Execute(Dictionary<string, object> parameters = null, bool sendLetter = true)
    {
        if (LetterMessage != null)
        {
            Dictionary<string, object> letterParameters = parameters;

            letterParameters.Add("Letter_From", (Character)parameters["From"]);
            letterParameters.Add("Letter_To", (Character)parameters["To"]);

            LetterDispenserEntity.Instance.DispenseLetter(new Letter(LetterMessage, letterParameters));
        }
    }

    public virtual bool RollChance()
    {
        float roll = Random.Range(0f, 1f);
        if (roll < Chance)
        {
            return true;
        }

        return false;
    }
}
