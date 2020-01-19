using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter
{
    public LetterPreset Preset;
    public Dictionary<string, object> Parameters;

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
            
            if(Preset.Encryption != null)
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
}
