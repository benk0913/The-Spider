using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAAddQuestioningInstance", menuName = "DataObjects/Dialog/Actions/DDAAddQuestioningInstance", order = 2)]
public class DDAAddQuestioningInstance : DialogDecisionAction
{
    [SerializeField]
    Character TargetCharacter;

    [SerializeField]
    public QuestioningInstance QInstance;

    public override void Activate()
    {
        CORE.Instance.Characters.Find(x => x.name == TargetCharacter.name).CurrentQuestioningInstance = QInstance;
    }
}
