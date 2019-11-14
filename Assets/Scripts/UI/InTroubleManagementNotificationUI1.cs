using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InTroubleManagementNotificationUI1 : CharacterManagementNotificationUI
{
    public override bool CommonFilter(Character character)
    {
        return character.TopEmployer == CORE.PC
            && character.CurrentTaskEntity != null
            &&
            (character.CurrentTaskEntity.CurrentTask.name   == "Being Hanged"
            || character.CurrentTaskEntity.CurrentTask.name == "Being Interrogated"
            || character.CurrentTaskEntity.CurrentTask.name == "Locked In Prison");
    }
}
