using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmployeeForcedTask", menuName = "DataObjects/GameEvents/Property/EmployeeForcedTask", order = 2)]
public class EmployeeForcedTask : GameEvent
{
    [SerializeField]
    LongTermTask Task;

    public override void Execute(Dictionary<string, object> parameters = null, bool sendLetter = true)
    {
        LocationEntity location = (LocationEntity)parameters["Location"];
        
        if (location.EmployeesCharacters.Count < 0)
        {
            return;
        }


        Character character = location.EmployeesCharacters[Random.Range(0, location.EmployeesCharacters.Count)];
        
        

        parameters.Add("Target_Name", character.name);

        CORE.Instance.GenerateLongTermTask(this.Task, CORE.Instance.Database.GOD, character, (LocationEntity)character.WorkLocation);

        base.Execute(parameters, sendLetter);
    }
}
