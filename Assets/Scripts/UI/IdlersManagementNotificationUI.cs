using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlersManagementNotificationUI : CharacterManagementNotificationUI
{
    public override bool CommonFilter(Character character)
    {
        return character != CORE.PC && character.TopEmployer == CORE.PC  && character.CurrentTaskEntity == null;
    }
}
