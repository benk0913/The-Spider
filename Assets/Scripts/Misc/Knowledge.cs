using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knowledge
{
    public List<KnowledgeInstance> Items = new List<KnowledgeInstance>();

    public Character CurrentCharacter;

    public bool GetIsAnythingKnown(Character byCharacter)
    {
        foreach(KnowledgeInstance item in Items)
        {
            if(item.IsKnownByCharacter(byCharacter))
            {
                return true;
            }
        }

        return false;
        
    }

    public int GetKnownCount(Character byCharacter)
    {
        int count = 0;
        foreach(KnowledgeInstance instance in Items)
        {
            if (instance.IsKnownByCharacter(byCharacter))
                count++;
        }

        return count;
    }

    public Knowledge(Character ofCharacter = null)
    {
        CurrentCharacter = ofCharacter;

        Items.Add(new KnowledgeInstance("Name", "The name of the person."));
        Items.Add(new KnowledgeInstance("Personality", "The traits which make this person unique."));
        Items.Add(new KnowledgeInstance("WorkLocation", "Where this person works."));
        Items.Add(new KnowledgeInstance("HomeLocation", "Where this person lives."));

        Items.Add(new KnowledgeInstance("Appearance", "How this person looks"));
        Items.Add(new KnowledgeInstance("CurrentLocation", "Where this person currently is."));

        Items.Add(new KnowledgeInstance("Gold", "How much gold is in this person's possession."));
        //TODO DeepSecrets
    }

    public virtual void KnowEverything(Character byCharacter)
    {

        foreach (KnowledgeInstance item in Items)
        {
            Know(item.Key, byCharacter,false);
        }
        
    }

    public virtual void KnowEverythingAll()
    {
        foreach (Faction faction in CORE.Instance.Database.Factions)
        {
            if (faction.FactionHead == null)
            {
                continue;
            }

            Character factionLeader = CORE.Instance.GetCharacter(faction.FactionHead.name);

            if (factionLeader == null)
            {
                continue;
            }

            foreach (KnowledgeInstance item in Items)
            {
                Know(item.Key, factionLeader, false);
            }
        }
    }

    public virtual void KnowAll(string key)
    {
        foreach (Faction faction in CORE.Instance.Database.Factions)
        {
            if (faction.FactionHead == null)
            {
                continue;
            }

            Character factionLeader = CORE.Instance.GetCharacter(faction.FactionHead.name);

            if (factionLeader == null)
            {
                continue;
            }

            Know(key, factionLeader);
        }
    }

    public virtual void Know(string key,Character byCharacter, bool notify = true)
    {
        KnowledgeInstance instance = GetKnowledgeInstance(key);

        if (!instance.KnownByCharacters.Contains(byCharacter))
        {
            if (byCharacter == CORE.PC  && notify)
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

            instance.KnownByCharacters.Add(byCharacter);
        }
        

        if(key == "CurrentLocation")
        {
            if (CurrentCharacter.CurrentLocation != null)
            {
                CurrentCharacter.CurrentLocation.RefreshCharactersInLocationUI();
            }

            if (CurrentCharacter.CurrentTaskEntity != null)
            {
                CurrentCharacter.CurrentTaskEntity.CurrentTargetLocation.RefreshTasks();
            }
        }
    }

    public void Forget(string key, Character byCharacter)
    {
        GetKnowledgeInstance(key).KnownByCharacters.Remove(byCharacter);
    }

    public void ForgetAll(string key)
    {
        GetKnowledgeInstance(key).KnownByCharacters.Clear();
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

public class LocationKnowledge : Knowledge
{
    public LocationEntity CurrentLocation;

    public LocationKnowledge(LocationEntity location)
    {
        CurrentLocation = location;

        Items.Add(new KnowledgeInstance("Existance", "The Existance Of This Place"));
    }

    public override void Know(string key, Character byCharacter, bool notify = true)
    {
        KnowledgeInstance instance = GetKnowledgeInstance(key);

        if (!instance.IsKnownByCharacter(byCharacter))
        {
            instance.KnownByCharacters.Add(byCharacter);
        }


        if (key == "Existance")
        {
            //CurrentLocation
        }
    }
}

public class KnowledgeInstance
{
    public KnowledgeInstance(string key, string description)
    {
        this.Key = key;
        this.Description = description;
    }

    public string Key;
    public string Description;
    public List<Character> KnownByCharacters = new List<Character>();
    
    public bool IsKnownByCharacter(Character character)
    {
        return KnownByCharacters.Contains(character);
    }
}
