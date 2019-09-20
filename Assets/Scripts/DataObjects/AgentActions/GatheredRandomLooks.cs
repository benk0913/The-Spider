using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GatheredRandomLooks", menuName = "DataObjects/AgentActions/Spying/GatheredRandomLooks", order = 2)]
public class GatheredRandomLooks : AgentAction
{
    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        
        LocationEntity location = (LocationEntity)target;
        
        
        
    }
}
