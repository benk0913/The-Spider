using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationLanguage", menuName = "DataObjects/LocalizationLanguage", order = 2)]
public class LocalizationLanguage : ScriptableObject
{
    public string LanguageKey;
    public List<TranslationInstance> Instances = new List<TranslationInstance>();
    public Dictionary<string, string> Translations = new Dictionary<string, string>();
    public List<string> StringsToIgnore = new List<string>();

    public string Translate(string message)
    {
        if(Translations.ContainsKey(message))
        {
            return Translations[message];
        }

        return message;
    }

    public void Initialize()
    {
        Translations.Clear();
        foreach (TranslationInstance instance in Instances)
        {
            Translations.Add(instance.Key, instance.Value);
        }
    }
}

[System.Serializable]
public class TranslationInstance
{
    public string Key;
    public string Value;

    public TranslationInstance(string key,string value)
    {
        this.Key = key;
        this.Value = value;
    }
}
