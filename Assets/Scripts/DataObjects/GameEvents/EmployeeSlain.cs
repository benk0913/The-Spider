using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmployeeSlain", menuName = "DataObjects/GameEvents/Property/EmployeeSlain", order = 2)]
public class EmployeeSlain : GameEvent
{
    public override void Execute(Dictionary<string, object> parameters = null, bool sendLetter = true)
    {
        LocationEntity location = (LocationEntity)parameters["Location"];
        
        if (location.EmployeesCharacters.Count < 0)
        {
            return;
        }


        Character character = location.EmployeesCharacters[Random.Range(0, location.EmployeesCharacters.Count)];

        CORE.Instance.Database.GetEventAction("Death").Execute(CORE.Instance.Database.GOD, character, character.CurrentLocation);

        parameters.Add("Target_Name", character.name);

        base.Execute(parameters, sendLetter);

    }
}
