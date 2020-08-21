using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AgentActionLogUI : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI TurnsLeft;

    [SerializeField]
    protected PortraitUI CharacterPortrait;

    [SerializeField]
    protected ActionPortraitUI CurrentAction;

    [SerializeField]
    protected PortraitUI TargetCharacterPortrait;

    [SerializeField]
    protected LocationPortraitUI TargetLocationPortrait;

    [SerializeField]
    GameObject Arrows;

    public virtual void SetInfo(Character character, LongTermTaskEntity currentTask = null)
    {
        if(currentTask == null)
        {
            CharacterPortrait.SetCharacter(character);
            TargetCharacterPortrait.gameObject.SetActive(false);
            TargetLocationPortrait.gameObject.SetActive(false);
            CurrentAction.gameObject.SetActive(false);
            Arrows.gameObject.SetActive(false);
            TurnsLeft.text = "";
            return;
        }

        Arrows.gameObject.SetActive(true);

        CharacterPortrait.SetCharacter(character);

        CurrentAction.gameObject.SetActive(true);
        CurrentAction.SetAction(currentTask);

        if (currentTask.TargetCharacter != null)
        {
            TargetCharacterPortrait.gameObject.SetActive(true);
            TargetCharacterPortrait.SetCharacter(currentTask.TargetCharacter);
        }
        else
        {
            TargetCharacterPortrait.gameObject.SetActive(false);
        }

        if (currentTask.CurrentTargetLocation != null)
        {
            TargetLocationPortrait.gameObject.SetActive(true);
            TargetLocationPortrait.SetLocation(currentTask.CurrentTargetLocation);
        }
        else
        {
            TargetLocationPortrait.gameObject.SetActive(false);
        }

        if (currentTask.TurnsLeft > 1)
        {
            TurnsLeft.text = currentTask.CurrentTask.TurnsToComplete - currentTask.TurnsLeft + "/" + currentTask.CurrentTask.TurnsToComplete;
        }
        else
        {
            TurnsLeft.text = "";
        }
    }

}
