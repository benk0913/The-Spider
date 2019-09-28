using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SimpleScenario", menuName = "DataObjects/Dialog/Scenario/SimpleScenario", order = 2)]
public class SimpleScenario : ScriptableObject
{
    public ScenarioInstance DefaultScenario;

    public virtual void TriggerScenario(Character actor, LocationEntity location, Character target)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("Actor", actor);
        parameters.Add("ActorName", actor.name);
        parameters.Add("Location", location);
        parameters.Add("LocationName", location.CurrentProperty.name);
        parameters.Add("Target", target);
        parameters.Add("TargetName", target.name);

        TriggerScenario(parameters);
    }

    public virtual void TriggerScenario(Dictionary<string, object> parameters)
    {
        DialogWindowUI.Instance.StartNewDialog(DefaultScenario.DialogPieces, parameters);
    }

}

[System.Serializable]
public class ScenarioInstance
{
    public List<DialogPiece> DialogPieces = new List<DialogPiece>();
}
