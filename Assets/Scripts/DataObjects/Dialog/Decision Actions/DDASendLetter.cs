using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDASendLetter", menuName = "DataObjects/Dialog/Actions/DDASendLetter", order = 2)]
public class DDASendLetter : DialogDecisionAction
{
    [SerializeField]
    LetterPreset Letter;

    public override void Activate()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("Letter_From", this.Letter.PresetSender);
        parameters.Add("Letter_To", CORE.PC);

        Letter letter = new Letter(this.Letter, parameters);

        LetterDispenserEntity.Instance.DispenseLetter(letter);
    }
}
