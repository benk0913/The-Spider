using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAAddQuest", menuName = "DataObjects/Dialog/Actions/DDAAddQuest", order = 2)]
public class DDAAddQuest : DialogDecisionAction
{
    [SerializeField]
    Quest Quest;

    public override void Activate()
    {
        Quest questInstance = Quest.CreateClone();
        questInstance.ForCharacter = CORE.PC;
        QuestsPanelUI.Instance.AddNewExistingQuest(questInstance);
    }
}
