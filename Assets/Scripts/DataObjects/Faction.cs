using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Faction", menuName = "DataObjects/Faction", order = 2)]
public class Faction : ScriptableObject, ISaveFileCompatible
{
    [TextArea(4, 6)]
    public string Description;

    [TextArea(4, 6)]
    public string HowToPlayDescription;

    public Sprite Icon;

    public Material WaxMaterial;
    public Color FactionColor;
    public string RoomScene;
    public Character FactionHead;
    public GameObject FactionSelectionPrefab;
    public Sprite BriefBGSprite;

    public FactionAI AI;

    public Property[] FactionProperties;

    public LetterPreset[] StartingLetters;

    public AgentAction[] AgentActionsOnLocation;
    public AgentAction[] AgentActionsOnAgent;
    public PlayerAction[] PlayerActionsOnLocation;
    public PlayerAction[] PlayerActionsOnAgent;

    public bool isLockedByDefault = true;
    public string UnlockDescription;
    public bool isLocked = false;
    public bool isPlayable = true;
    public bool isAlwaysKnown = false;
    public bool HasPromotionSystem = false;
    public bool IsCustomziable = false;

    public int GoldGeneratedPerDay;
    public int ConnectionsGeneratedPerDay;
    public int RumorsGeneratedPerDay;
    public int ProgressGeneratedPerDay;
    public int ReputationGeneratedPerDay;

    public int RecommendedPropertyCap = 999;

    public Faction ClonedFrom = null;

    public string LeaveRoomDescription;

    public VideoClip IntroCutscene;

    public List<Character> Members
    {
        get
        {
            List<Character> members = CORE.Instance.Characters.FindAll(X => X.CurrentFaction.name == this.name);
            
            if(members == null)
            {
                return new List<Character>();
            }

            return members;
        }
    }

    public List<Quest> Goals = new List<Quest>();

    public FactionKnowledge Known;

    public FactionRelations Relations;

    public List<string> FactionRumors = new List<string>();

    public List<EndGameParameter> EndGameUniqueParameters = new List<EndGameParameter>();

    public List<TimelineInstance> Timeline = new List<TimelineInstance>();

    public Faction Clone()
    {
        Faction newClone = Instantiate(this);

        newClone.name = this.name;
        newClone.Known = new FactionKnowledge(newClone);
        newClone.Relations = new FactionRelations(newClone);
        newClone.ClonedFrom = this;
        newClone.GoldGeneratedPerDay = this.GoldGeneratedPerDay;
        newClone.RumorsGeneratedPerDay = this.RumorsGeneratedPerDay;
        newClone.ConnectionsGeneratedPerDay = this.ConnectionsGeneratedPerDay;
        newClone.ProgressGeneratedPerDay = this.ProgressGeneratedPerDay;
        newClone.HasPromotionSystem = this.HasPromotionSystem;

        return newClone;
    }

    public bool HasPsychoEffect = false;

    public void FromJSON(JSONNode node)
    {
        if (!string.IsNullOrEmpty(node["FactionHead"].Value))
        {
            factionHeadID = node["FactionHead"].Value;
        }
        else
        {
            if (ClonedFrom != null && ClonedFrom.FactionHead != null)
            {
                Character cloneCharacter = CORE.Instance.Characters.Find(X => X.name == ClonedFrom.FactionHead.name);

                if (cloneCharacter != null)
                {
                    factionHeadID = cloneCharacter.ID;
                }
                else
                {
                    factionHeadID = CORE.Instance.Database.GOD.ID;
                }
            }
            else
            {
                factionHeadID = CORE.Instance.Database.GOD.ID;
            }
        }
        

        foreach (KnowledgeInstance item in Known.Items)
        {
            if (string.IsNullOrEmpty(node["Knowledge"][item.Key].ToString()))
            {
                continue;
            }

            List<string> IDs = new List<string>();
            for (int i = 0; i < node["Knowledge"][item.Key].Count; i++)
            {
                IDs.Add(node["Knowledge"][item.Key][i].Value);
            }

            knowledgeCharacterIDs.Add(item.Key, IDs);
        }

        if (Relations == null)
        {
            Relations = new FactionRelations(this);
        }

        Relations.FromJSON(node["Relations"]);
    }

    public Dictionary<string, List<string>> knowledgeCharacterIDs = new Dictionary<string, List<string>>();
    public string factionHeadID;

    public void ImplementIDs()
    {
        if (CORE.PC.CurrentFaction.name == this.name) // patch
        {
            FactionHead = CORE.PC;
        }
        else
        {
            FactionHead = CORE.Instance.Characters.Find(x => x.ID == factionHeadID);
            if (FactionHead == null)
            {
                FactionHead = CORE.Instance.Database.GOD;
            }
        }

        foreach (string key in knowledgeCharacterIDs.Keys)
        {
            for (int i = 0; i < knowledgeCharacterIDs[key].Count; i++)
            {
                Character character = CORE.Instance.GetCharacterByID(knowledgeCharacterIDs[key][i]);

                if (character == null)
                {
                    continue;
                }

                Known.Know(key, character);
            }
        }

        Relations.ImplementIDs();
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["Key"] = this.name;
        node["ClonedFrom"] = ClonedFrom == null ? "" : ClonedFrom.name;

        
        if (CORE.PC.CurrentFaction.name == this.name) // patch
        {
            node["FactionHead"] = CORE.PC.ID;
        }
        else
        {
            node["FactionHead"] = FactionHead != null ? FactionHead.ID : "GOD";
        }

        foreach (KnowledgeInstance item in Known.Items)
        {
            for (int i = 0; i < item.KnownByCharacters.Count; i++)
            {
                node["Knowledge"][item.Key][i] = item.KnownByCharacters[i].ID;
            }
        }

        node["Relations"] = Relations.ToJSON(this);

        return node;
    }

    public void OnTurnPassed()
    {
        TimelineInstance instance = Timeline.Find(x => x.Turn == GameClock.Instance.CurrentTurn);

        if (instance == null)
        {
            return;
        }

        if (instance.PlayerOnly && CORE.PlayerFaction.name != this.name)
        {
            return;
        }

        foreach (DialogDecisionAction action in instance.Actions)
        {
            action.Activate();
        }
    }

    public void DissolveFaction()
    {
        WarningWindowUI.Instance.Show("The faction " + this.name + " has been destroyed!", () => { });

        List<Character> members = Members;

        foreach(Character character in members)
        {
            character.CurrentFaction = CORE.Instance.Database.DefaultFaction;
        }
    }


}

public class FactionRelations : ISaveFileCompatible
{
    public Faction OfFaction;

    public List<FactionRelationInstance> Relations = new List<FactionRelationInstance>();

    public FactionRelations(Faction ofFaction)
    {
        this.OfFaction = ofFaction;
    }

    public FactionRelationInstance GetRelations(Faction withFaction)
    {
        FactionRelationInstance relationInstance = Relations.Find(x => x.WithFaction == withFaction);

        if (relationInstance == null)
        {
            relationInstance = new FactionRelationInstance(this.OfFaction, withFaction);
            Relations.Add(relationInstance);
        }

        return relationInstance;
    }

    public void FromJSON(JSONNode node)
    {
        ofFactionName = node["OfFaction"];

        Relations.Clear();
        for(int i=0;i<node["Relations"].Count;i++)
        {
            FactionRelationInstance relationInstance = new FactionRelationInstance();
            relationInstance.ofFactionName = ofFactionName;
            relationInstance.FromJSON(node["Relations"][i]);
            Relations.Add(relationInstance);
        }
    }

    public string ofFactionName;

    public void ImplementIDs()
    {
        if (!string.IsNullOrEmpty(ofFactionName))
        {
            this.OfFaction = CORE.Instance.Factions.Find(x => x.name == ofFactionName);
        }

        foreach (FactionRelationInstance relation in Relations)
        {
            relation.ImplementIDs();
        }
    }

    public JSONNode ToJSON(Faction backupFaction = null)
    {
        JSONClass node = new JSONClass();

        if (OfFaction == null)
        {
            Debug.LogError("No OfFaction in factions relations....");
            if (backupFaction != null)
            {
                node["OfFaction"] = backupFaction.name;
            }
            else
            {
                node["OfFaction"] = "Faction Of The Past "+Random.Range(1, 999).ToString();
            }
        }
        else
        {
            node["OfFaction"] = OfFaction.name;
        }

        for (int i=0;i<Relations.Count;i++)
        {
            node["Relations"][i] = Relations[i].ToJSON();
        }

        return node;
    }

    public JSONNode ToJSON()
    {
        return ToJSON(null);
    }
}

public class FactionRelationInstance : ISaveFileCompatible
{
    public const int MAX_RELATION = 10;
    public const int MIN_RELATION = -10;

    public Faction OfFaction;
    public Faction WithFaction;

    //USE THIS
    public int TotalValue
    {
        get
        {
            return System.Math.Max(System.Math.Min(Value + PassiveValue, MAX_RELATION), MIN_RELATION);
        }
        set
        {
            int previousValue = TotalValue;
            this.Value = value;

            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(OfFaction.name + " - " + WithFaction.name + " Relation Changed: " + previousValue + " to " + TotalValue, ResourcesLoader.Instance.GetSprite("Indifferent")));
        }
    }

    public int Value
    {
        get
        {
            return _value;
        }
        set
        {
            if (value > _value && TotalValue >= MAX_RELATION)
            {
                return;
            }
            else if (value < _value && TotalValue <= MIN_RELATION)
            {
                return;
            }

            _value = value;
        }
    }
    int _value;

    public int PassiveValue
    {
        get
        {
            int value = 0;

            foreach(string key in RelationModifiers.Keys)
            {
                value += RelationModifiers[key];
            }

            return value;
        }
    }

    public Dictionary<string, int> RelationModifiers
    {
        get
        {
            Dictionary<string, int> modifiers = new Dictionary<string, int>();

            if (OfFaction.FactionHead == null || WithFaction.FactionHead == null)
            {
                return modifiers;
            }

            Character ofFactionHead = CORE.Instance.Characters.Find(x => x.name == OfFaction.FactionHead.name);
            Character withFactionHead = CORE.Instance.Characters.Find(x => x.name == WithFaction.FactionHead.name);

            if (ofFactionHead != null && withFactionHead != null)
            {
                if (ofFactionHead.PropertiesInCommand.Count > withFactionHead.PropertiesInCommand.Count)
                {
                    modifiers.Add("Small Threat", 1);
                }
                else
                {
                    modifiers.Add("Big Threat", -1);
                }

                if (withFactionHead.Reputation > 0)
                {
                    modifiers.Add("Good Reputation", 2);
                }
                else if (withFactionHead.Reputation < 0)
                {
                    modifiers.Add("Bad Reputation", -2);
                }
            }
            

            return modifiers;
        }
    }

    public FactionRelationInstance(Faction ofFaction = null, Faction withFaction = null)
    {
        this.OfFaction = ofFaction;
        this.WithFaction = withFaction;
        this.Value = 0;
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();
        node["OfFaction"] = OfFaction != null ? OfFaction.name : CORE.Instance.Database.DefaultFaction.name;
        node["WithFaction"] = WithFaction != null? WithFaction.name : CORE.Instance.Database.DefaultFaction.name;
        node["Value"] = _value.ToString();

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        this.ofFactionName = node["OfFaction"].Value;
        this.withFactionName = node["WithFaction"].Value;
        this._value = int.Parse(node["Value"].Value);
    }

    public string withFactionName;
    public string ofFactionName;

    public void ImplementIDs()
    {
        if(!string.IsNullOrEmpty(withFactionName))
        {
            this.WithFaction = CORE.Instance.Factions.Find(x => x.name == withFactionName);
        }

        if (!string.IsNullOrEmpty(ofFactionName))
        {
            this.OfFaction = CORE.Instance.Factions.Find(x => x.name == ofFactionName);
        }
    }
}
