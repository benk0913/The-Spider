using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QRGainLetter", menuName = "DataObjects/Quests/Rewards/QRGainLetterSe", order = 2)]
public class QRGainLetter : QuestReward
{
    [SerializeField]
    public LetterPreset Letter;

    public override void Claim(Character byCharacter)
    {
        base.Claim(byCharacter);

        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("Letter_From", this.Letter.PresetSender);
        parameters.Add("Letter_To", CORE.PC);

        Letter letter = new Letter(this.Letter, parameters);

        LetterDispenserEntity.Instance.DispenseLetter(letter);
    }
}