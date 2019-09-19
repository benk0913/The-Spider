using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knowledge
{
    public List<KnowledgeInstance> Items = new List<KnowledgeInstance>();

    public Knowledge()
    {
        Items.Add(new KnowledgeInstance("Name", "The name of the person.", false));
        Items.Add(new KnowledgeInstance("Personality", "The traits which make this person unique.", false));
        Items.Add(new KnowledgeInstance("Relations","This person's relations with you.", false));
        Items.Add(new KnowledgeInstance("Skills", "This persons unique skills.", false));
        Items.Add(new KnowledgeInstance("WorkLocation", "Where this person works.", false));
        Items.Add(new KnowledgeInstance("HomeLocation", "Where this person lives.", false));

        Items.Add(new KnowledgeInstance("Appearance", "How this person looks", false));
        Items.Add(new KnowledgeInstance("CurrentLocation", "Where this person currently is.", false));

        Items.Add(new KnowledgeInstance("Gold", "How much gold is in this person's possession.", false));
        //TODO DeepSecrets
    }

    public void KnowAllBasic()
    {
        foreach(KnowledgeInstance item in Items)
        {
            item.IsKnown = true;
        }
    }
}

public class KnowledgeInstance
{
    public KnowledgeInstance(string key, string description, bool isKnwon)
    {
        this.Key = key;
        this.Description = description;
        this.IsKnown = isKnwon;
    }

    public string Key;
    public string Description;
    public bool IsKnown;
    
}
