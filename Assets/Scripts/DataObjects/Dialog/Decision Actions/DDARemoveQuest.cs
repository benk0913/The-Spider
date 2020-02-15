using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDARemoveQuest", menuName = "DataObjects/Dialog/Actions/DDARemoveQuest", order = 2)]
public class DDARemoveQuest : DialogDecisionAction
{
    [SerializeField]
    Quest Quest;

    public override void Activate()
    {
        LetterDispenserEntity.Instance.Envelopes.
            FindAll(x => x.PresetLetter != null && x.PresetLetter.QuestAttachment != null && x.PresetLetter.QuestAttachment.name == Quest.name)
            .ForEach((x) => 
            {
                Destroy(x.gameObject);
                LetterDispenserEntity.Instance.DisposeLetter(x);
            });

        LettersPanelUI.Instance.ArchivedLetters.
            FindAll(x => x.CurrentLetter != null && x.CurrentLetter.Preset!=null && x.CurrentLetter.Preset.QuestAttachment != null && x.CurrentLetter.Preset.QuestAttachment.name == Quest.name).
            ForEach((x) => 
        {
            LettersPanelUI.Instance.LetterSelected(x);
            LettersPanelUI.Instance.RemoveSelectedLetter();
        });

        QuestsPanelUI.Instance.RemoveExistingQuest(Quest);
    }
}
