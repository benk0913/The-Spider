using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knowledge
{
    public List<KnowledgeInstance> Items = new List<KnowledgeInstance>();

    public Character CurrentCharacter;

    public KnowledgeRumor GetRandomKnowledgeRumor()
    {
        List<KnowledgeInstance> PossibleKeys = new List<KnowledgeInstance>();
        PossibleKeys.AddRange(Items);
        PossibleKeys.RemoveAll(x => x.KnownByCharacters.Contains(CORE.PC));

        if(PossibleKeys.Count == 0)
        {
            return null;
        }

        string key = PossibleKeys[Random.Range(0, PossibleKeys.Count)].Key;

        KnowledgeRumor rumor = new KnowledgeRumor();
        rumor.RelevantKey = key;

        List<string> possibilities = new List<string>();

        switch (key)
        {
            case "Name":
                {
                    possibilities.Add("\"Who? " +CurrentCharacter.name+ "?\"");
                    possibilities.Add("\"Once heard of " + CurrentCharacter.name + "...\"");
                    possibilities.Add("\"The name is " + CurrentCharacter.name + "...\"");
                    possibilities.Add("\"Yeah! I know " + CurrentCharacter.name + "...\"");
                    
                    break;
                }
            case "Appearance":
                {
                    possibilities.AddRange(CurrentCharacter.SkinColor.PossibleRumors);
                    possibilities.AddRange(CurrentCharacter.HairColor.PossibleRumors);

                    break;
                }
            case "Personality":
                {
                    if(CurrentCharacter.Traits.Count <= 2)
                    {
                        possibilities.Add("\"This person is as bland as a rock\"");
                        possibilities.Add("\"Most boring person I've ever met...\"");
                        break;
                    }
                    
                    possibilities.Add(CurrentCharacter.Traits[Random.Range(0, CurrentCharacter.Traits.Count)].KnowledgeRumor);
                    

                    break;
                }
            case "CurrentLocation":
                {
                    possibilities.AddRange(CurrentCharacter.CurrentLocation.CurrentProperty.VisitRumors);
                    break;
                }
            case "HomeLocation":
                {
                    possibilities.Add("Known to reside in - \""+CurrentCharacter.HomeLocation.Name+"\"");
                    break;
                }
            case "WorkLocation":
                {
                    if(CurrentCharacter.WorkLocation == null)
                    {
                        possibilities.Add("\"Seems unemployed to me...\"");
                        break;
                    }

                    possibilities.AddRange(CurrentCharacter.WorkLocation.CurrentProperty.WorkRumors);
                    break;
                }
            case "Faction":
                {
                    possibilities.AddRange(CurrentCharacter.CurrentFaction.FactionRumors);
                    break;
                }
        }

        if (possibilities.Count > 0)
        {
            rumor.Description = possibilities[Random.Range(0, possibilities.Count)];
        }
        else
        {
            rumor.Description = key;
        }

        return rumor;
    }

    public bool GetIsAnythingKnown(Character byCharacter)
    {
        foreach(KnowledgeInstance item in Items)
        {
            if(item.Key == "Faction")
            {
                continue;
            }

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
        BaseParams();
    }

    public virtual void BaseParams()
    {
        Items.Add(new KnowledgeInstance("Name", "The name of the person.",1, ResourcesLoader.Instance.GetSprite("idea_letter")));
        Items.Add(new KnowledgeInstance("Personality", "The traits which make this person unique.",3, ResourcesLoader.Instance.GetSprite("seduce")));
        Items.Add(new KnowledgeInstance("WorkLocation", "Where this person works.",3, ResourcesLoader.Instance.GetSprite("Dockworker")));
        Items.Add(new KnowledgeInstance("HomeLocation", "Where this person lives.",3, ResourcesLoader.Instance.GetSprite("district3")));

        Items.Add(new KnowledgeInstance("Appearance", "How this person looks",2, ResourcesLoader.Instance.GetSprite("clothingTexture")));
        Items.Add(new KnowledgeInstance("CurrentLocation", "Where this person currently is.",2, ResourcesLoader.Instance.GetSprite("DiscoverMap")));

        Items.Add(new KnowledgeInstance("Faction", "Who is this person working for?",3, ResourcesLoader.Instance.GetSprite("standoff")));
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
        foreach (Faction faction in CORE.Instance.Factions)
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
        foreach (Faction faction in CORE.Instance.Factions)
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
                InformationLogUI.Instance.AddInformationGathered(instance.Key, CurrentCharacter);
            }

            instance.KnownByCharacters.Add(byCharacter);
        }
        else
        {
            return;
        }


        if (key == "CurrentLocation")
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

        if(key == "Faction")
        {
            if (CurrentCharacter.CurrentFaction != null)
            {
                CORE.Instance.Factions.Find(x => x.name == CurrentCharacter.CurrentFaction.name).Known.Know("Existance", byCharacter, false);
            }

            foreach(LocationEntity location in CurrentCharacter.PropertiesOwned)
            {
                foreach(Character employee in location.EmployeesCharacters)
                {
                    employee.Known.Know("Faction", byCharacter, false);
                }
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

    public bool IsKnown(string key, Character forCharacter)
    {
        return GetKnowledgeInstance(key).KnownByCharacters.Contains(forCharacter);
    }
}

public class LocationKnowledge : Knowledge
{
    public LocationEntity CurrentLocation;

    public LocationKnowledge(LocationEntity location)
    {
        CurrentLocation = location;
    }

    public override void BaseParams()
    {
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
            if (byCharacter == CORE.PC)
            {
                AudioControl.Instance.PlayInPosition("location_reveal", CurrentLocation.transform.position);
            }

            CurrentLocation.RefreshState();
        }
    }
}

public class FactionKnowledge : Knowledge
{
    public Faction CurrentFaction;

    public FactionKnowledge(Faction faction)
    {
        CurrentFaction = faction;
    }
    
    public override void BaseParams()
    {
        Items.Add(new KnowledgeInstance("Existance", "The Existance Of This Faction"));
    }


    public override void Know(string key, Character byCharacter, bool notify = true)
    {
        KnowledgeInstance instance = GetKnowledgeInstance(key);

        if (!instance.IsKnownByCharacter(byCharacter))
        {
            instance.KnownByCharacters.Add(byCharacter);
        }
    }
}

public class KnowledgeInstance
{
    public KnowledgeInstance(string key, string description, int scoreMax = 1, Sprite icon = null)
    {
        this.Icon = icon;
        this.Key = key;
        this.Description = description;
        this.ScoreMax = scoreMax;
    }

    public Sprite Icon;
    public string Key;
    public string Description;
    public List<Character> KnownByCharacters = new List<Character>();
    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;

            if(_score >= ScoreMax)
            {
                _score = 0;
                if(!KnownByCharacters.Contains(CORE.PC))
                {
                    KnownByCharacters.Add(CORE.PC);
                }
            }
        }
    }
    public int _score;
    public int ScoreMax;


    public bool IsKnownByCharacter(Character character)
    {
        return KnownByCharacters.Contains(character);
    }
}
