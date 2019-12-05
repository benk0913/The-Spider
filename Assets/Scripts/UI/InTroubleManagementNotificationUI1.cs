using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InTroubleManagementNotificationUI1 : CharacterManagementNotificationUI
{
    public override bool CommonFilter(Character character)
    {
        return character.TopEmployer == CORE.PC
            && character.CurrentTaskEntity != null
            && character.IsInTrouble;
    }
}
