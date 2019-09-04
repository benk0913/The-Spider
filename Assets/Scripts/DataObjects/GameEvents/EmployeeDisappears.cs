using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmployeeDisappears", menuName = "DataObjects/GameEvents/Property/EmployeeDisappears", order = 2)]
public class EmployeeDisappears : GameEvent
{
    public override void Execute(Dictionary<string, object> parameters = null, bool sendLetter = true)
    {
        LocationEntity location = (LocationEntity)parameters["Location"];
        
        if (location.EmployeesCharacters.Count <= 0)
        {
            return;
        }

        Character character = location.EmployeesCharacters[Random.Range(0, location.EmployeesCharacters.Count)];
        character.StopWorkingFor(location);
        parameters.Add("Target_Name", character.name);

        base.Execute(parameters, sendLetter);

    }
}
