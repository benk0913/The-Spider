using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LocationBasedScenario", menuName = "DataObjects/Dialog/Scenario/LocationBasedScenario", order = 2)]
public class LocationBasedScenario : SimpleScenario
{
    public List<LocationScenarioInstance> LocationScenarios = new List<LocationScenarioInstance>();

    public override void TriggerScenario(Dictionary<string, object> parameters)
    {
        ScenarioInstance instance =  GetInstance((LocationEntity)parameters["Location"]);

        if(instance == null)
        {
            instance = DefaultScenario;
        }

        DialogWindowUI.Instance.StartNewDialog(instance.Piece, parameters);
    }

    public LocationScenarioInstance GetInstance(LocationEntity byLocation)
    {
        foreach(LocationScenarioInstance scenario in LocationScenarios)
        {
            if(scenario.PropertyType == byLocation.CurrentProperty)
            {
                return scenario;
            }
        }

        return null;
    }

}

[System.Serializable]
public class LocationScenarioInstance : ScenarioInstance
{
    public Property PropertyType;
}