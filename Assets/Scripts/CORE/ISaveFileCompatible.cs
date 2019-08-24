using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveFileCompatible
{
    JSONNode ToJSON();

    void FromJSON(JSONNode node);

    void ImplementIDs();
}
