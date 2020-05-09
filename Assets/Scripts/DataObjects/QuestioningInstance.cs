using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestioningInstance", menuName = "DataObjects/QuestioningInstance", order = 2)]
public class QuestioningInstance : ScriptableObject, ISaveFileCompatible
{
    public string Title;

    public LetterPreset CompleteLetter;

    public bool InstantFailure = false;

    public void FromJSON(JSONNode node)
    {
        Title = node["Title"];
        CompleteLetter = CORE.Instance.Database.PresetLetters.Find(x=>x.name == node["CompleteDialog"]);
        InstantFailure = bool.Parse(node["InstantFailure"]);
    }

    public void ImplementIDs()
    {
        
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["Title"] = Title;
        node["CompleteDialog"] = CompleteLetter.name;
        node["InstantFailure"] = InstantFailure.ToString();

        return node;
    }

    public QuestioningInstance Clone()
    {
        QuestioningInstance clone = Instantiate(this);
        clone.name = this.name;

        return clone;
    }
}
