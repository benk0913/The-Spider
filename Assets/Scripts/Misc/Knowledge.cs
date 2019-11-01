﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knowledge
{
    public List<KnowledgeInstance> Items = new List<KnowledgeInstance>();

    public Character CurrentCharacter;

    public bool IsSomethingKnown
    {
        get
        {
            foreach(KnowledgeInstance item in Items)
            {
                if(item.IsKnown)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public int KnownCount
    {
        get
        {
            int count = 0;
            foreach(KnowledgeInstance instance in Items)
            {
                if (instance.IsKnown)
                    count++;
            }

            return count;
        }
    }

    public Knowledge(Character ofCharacter)
    {
        CurrentCharacter = ofCharacter;

        Items.Add(new KnowledgeInstance("Name", "The name of the person.", false));
        Items.Add(new KnowledgeInstance("Personality", "The traits which make this person unique.", false));
        Items.Add(new KnowledgeInstance("WorkLocation", "Where this person works.", false));
        Items.Add(new KnowledgeInstance("HomeLocation", "Where this person lives.", false));

        Items.Add(new KnowledgeInstance("Appearance", "How this person looks", false));
        Items.Add(new KnowledgeInstance("CurrentLocation", "Where this person currently is.", false));

        Items.Add(new KnowledgeInstance("Gold", "How much gold is in this person's possession.", false));
        //TODO DeepSecrets
    }

    public void KnowAllBasic()
    {
        foreach (KnowledgeInstance item in Items)
        {
            Know(item.Key, false);
        }
    }

    public void Know(string key, bool notify = true)
    {
        KnowledgeInstance instance = GetKnowledgeInstance(key);

        if (!instance.IsKnown)
        {
            if (notify)
            {
                if (CurrentCharacter.CurrentLocation != null)
                {
                    CORE.Instance.SplineAnimationObject(
                        "PaperCollectedWorld",
                        CurrentCharacter.CurrentLocation.transform,
                        InformationLogUI.Instance.Notification.transform,
                        null,
                        false);
                }

                InformationLogUI.Instance.AddInformationGathered(instance.Key, CurrentCharacter);
            }

            instance.IsKnown = true;
        }
        

        if(key == "CurrentLocation" && CurrentCharacter.CurrentTaskEntity != null)
        {
            CurrentCharacter.CurrentTaskEntity.CurrentTargetLocation.RefreshTasks();
        }
    }

    public void Forget(string key)
    {
        GetKnowledgeInstance(key).IsKnown = false;
    }

    public KnowledgeInstance GetKnowledgeInstance(string key)
    {
        foreach(KnowledgeInstance instance in Items)
        {
            if(instance.Key == key)
            {
                return instance;
            }
        }

        return null;
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
