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
    public bool FromRaven
    {
        get
        {
            if(_fromRaven == -1)
            {
                if(Preset != null)
                {
                    _fromRaven = Preset.FromRaven? 1 : 0;
                }
                else
                {
                    _fromRaven = 0;
                }
            }

            return _fromRaven == 1? true : false;
        }
        set
        {
            _fromRaven = value? 1 : 0;
        }
    }
    int _fromRaven = -1;

    public string Title
    {
        get
        {
            string title = "";
            if (Preset == null)
            {
                if (CORE.Instance.DEBUG)
                {
                    Debug.Log("NO PRESET FOR LETTER");
                }

                title = " -- ";
            }
            else
            {
                title = Preset.Title;
            }

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
            string description = "";

            if (Preset == null)
            {
                if (CORE.Instance.DEBUG)
                {
                    Debug.Log("NO PRESET FOR LETTER");
                }

                description = " -- ";
            }
            else
            {
                description = Preset.Description;
            }

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

        node["FromRaven"] = FromRaven.ToString();

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
                if (tempValue != null && tempValue.GetType() == typeof(string))
                {
                    elementValue = (string)tempValue;
                }
                else if (tempValue != null && tempValue.GetType() == typeof(Character))
                {
                    elementValue = ((Character)tempValue).ID;
                }
                else if (tempValue != null && tempValue.GetType() == typeof(LocationEntity))
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

        FromRaven = bool.Parse(node["FromRaven"]);

        tempParameters = new Dictionary<string, string>();
        for(int i=0;i<node["Parameters"].Count;i++)
        {
            if(node["Parameters"][i]["Value"].Value == null || string.IsNullOrEmpty(node["Parameters"][i]["Value"].Value))
            {
                Debug.LogError("LETTER VALUE IS NULL " + node["Parameters"][i]["Key"].Value);
                tempParameters.Add(node["Parameters"][i]["Key"].Value, "");
                continue;
            }

            if (node["Parameters"][i]["Key"].Value == null || string.IsNullOrEmpty(node["Parameters"][i]["KEY"].Value))
            {
                Debug.LogError("LETTER KEY IS NULL " + node["Parameters"][i]["Value"].Value);
                continue;
            }

            tempParameters.Add(node["Parameters"][i]["Key"].Value, node["Parameters"][i]["Value"].Value);
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
