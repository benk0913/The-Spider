using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAKnowInformationAboutLocation", menuName = "DataObjects/Dialog/Actions/DDAKnowInformationAboutLocation", order = 2)]
public class DDAKnowInformationAboutLocation : DialogDecisionAction
{
    public List<string> Information = new List<string>();

    public Property LocationProperty;
    public Character ByCharacter;

    public override void Activate()
    {
        LocationEntity location = CORE.Instance.Locations.Find(X => X.CurrentProperty == LocationProperty);

        if(location == null)
        {
            Debug.LogError(this + " LOCATION NOT FOUND " + LocationProperty.name);
            return;
        }

        Character byCharacter;

        byCharacter = CORE.Instance.Characters.Find(y => y.name == ByCharacter.name);

        if (byCharacter == null)
        {
            Debug.LogError(this + " CHARACTER NOT FOUND " + ByCharacter.name);
            return;
        }


        Information.ForEach(x => location.Known.Know(x, byCharacter, false));
    }
}
