using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[System.Serializable]
public class KnowledgeRumor : ISaveFileCompatible
{
    public string RelevantKey;
    public string Description;

    public void FromJSON(JSONNode node)
    {
        RelevantKey = node["RelevantKey"];
        Description = node["Description"];
    }

    public void ImplementIDs()
    {
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["RelevantKey"] = RelevantKey;
        node["Description"] = Description;

        return node;
    }
}
