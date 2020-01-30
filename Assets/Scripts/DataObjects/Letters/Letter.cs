using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class Letter : ISaveFileCompatible
{
    public LetterPreset Preset;
    public Dictionary<string, object> Parameters;
    public bool IsDeciphered = false;

    public string Title
    {
        get
        {
            string title = Preset.Title;

            if (Parameters != null)
            {
                foreach (string key in Parameters.Keys)
                {
                    if (!title.Contains("{" + key + "}"))
                    {
                        continue;
                    }

                    title = title.Replace("{" + key + "}", (string)Parameters[key]);
                }
            }

            return title;
        }
    }

    public string Content
    {
        get
        {
            string description = Preset.Description;

            if (Parameters != null)
            {
                foreach (string key in Parameters.Keys)
                {
                    if (!description.Contains("{" + key + "}"))
                    {
                        continue;
                    }

                    description = description.Replace("{" + key + "}", (string)Parameters[key]);
                }
            }
            
            if(Preset.Encryption != null && !IsDeciphered)
            {
                return Preset.Encryption.Convert(description);    
            }

            return description;
        }
    }

    public Letter(LetterPreset preset, Dictionary<string, object> parameters = null)
    {
        this.Preset = preset;
        this.Parameters = parameters;
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["IsDeciphered"] = IsDeciphered.ToString();

        if (Preset != null)
        {
            node["Preset"] = Preset.name;
        }

        if (Parameters != null)
        {
            for (int i = 0; i < Parameters.Keys.Count; i++)
            {
                string elementValue = "";

                object tempValue = Parameters[Parameters.Keys.ElementAt(i)];
                if (tempValue.GetType() == typeof(string))
                {
                    elementValue = (string)tempValue;
                }
                else if (tempValue.GetType() == typeof(Character))
                {
                    elementValue = ((Character)tempValue).ID;
                }
                else if (tempValue.GetType() == typeof(LocationEntity))
                {
                    elementValue = ((LocationEntity)tempValue).ID;
                }
                else
                {
                    Debug.LogError("Couldn't find element value!!!!! " + Parameters.Keys.ElementAt(i).ToString());
                }

                if (!string.IsNullOrEmpty(elementValue))
                {
                    node["Parameters"][i]["Key"] = Parameters.Keys.ElementAt(i).ToString();
                    node["Parameters"][i]["Value"] = elementValue;
                }
            }
        }

        return node;
    }


    Dictionary<string, string> tempParameters;

    public void FromJSON(JSONNode node)
    {
        IsDeciphered = bool.Parse(node["IsDeciphered"]);

        Preset = CORE.Instance.Database.PresetLetters.Find(x=>x.name == node["Preset"].Value.ToString());

        tempParameters = new Dictionary<string, string>();
        for(int i=0;i<node["Parameters"].Count;i++)
        {
            tempParameters.Add(node["Parameters"][i]["Key"], node["Parameters"][i]["Value"]);
        }
    }

    public void ImplementIDs()
    {
        if(tempParameters == null || tempParameters.Keys.Count == 0)
        {
            return;
        }

        Dictionary<string, object> resultParameters = new Dictionary<string, object>();
        for (int i = 0; i < tempParameters.Keys.Count; i++)
        {
            string tempValue = tempParameters[tempParameters.Keys.ElementAt(i)];

            Character possibleCharacter = CORE.Instance.Characters.Find(x => x.ID == tempValue);
            if(possibleCharacter != null)
            {
                resultParameters.Add(tempParameters.Keys.ElementAt(i), possibleCharacter);
                continue;
            }

            LocationEntity possibleLocation = CORE.Instance.Locations.Find(x => x.ID == tempValue);
            if (possibleLocation != null)
            {
                resultParameters.Add(tempParameters.Keys.ElementAt(i), possibleLocation);
                continue;
            }

            resultParameters.Add(tempParameters.Keys.ElementAt(i), tempValue);
        }
    }
}
