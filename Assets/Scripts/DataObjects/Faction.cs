using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[CreateAssetMenu(fileName = "Faction", menuName = "DataObjects/Faction", order = 2)]
public class Faction : ScriptableObject, ISaveFileCompatible
{
    [TextArea(4, 6)]
    public string Description;
    public Sprite Icon;

    public Material WaxMaterial;
    public Color FactionColor;
    public string RoomScene;
    public Character FactionHead;
    public GameObject FactionSelectionPrefab;

    public FactionAI AI;

    public Property[] FactionProperties;

    public LetterPreset[] StartingLetters;

    public AgentAction[] AgentActionsOnLocation;
    public AgentAction[] AgentActionsOnAgent;
    public PlayerAction[] PlayerActionsOnLocation;
    public PlayerAction[] PlayerActionsOnAgent;

    public bool isLockedByDefault = true;
    public bool isLocked =  false;
    public bool isPlayable = true;
    public bool isAlwaysKnown = false;

    public int GoldGeneratedPerDay;
    public int ConnectionsGeneratedPerDay;
    public int RumorsGeneratedPerDay;
    public int ProgressGeneratedPerDay;
    public int ReputationGeneratedPerDay;

    public List<Quest> Goals = new List<Quest>();

    public FactionKnowledge Known;

    public Faction Clone()
    {
        Faction newClone = Instantiate(this);

        newClone.name = this.name;
        newClone.Known = new FactionKnowledge(newClone);

        return newClone;
    }

    public void FromJSON(JSONNode node)
    {
        foreach (KnowledgeInstance item in Known.Items)
        {
            if (string.IsNullOrEmpty(node["Knowledge"][item.Key]))
            {
                continue;
            }

            List<string> IDs = new List<string>();
            for (int i = 0; i < node["Knowledge"][item.Key].Count; i++)
            {
                IDs.Add(node["Knowledge"][item.Key][i]);
            }

            knowledgeCharacterIDs.Add(node["Knowledge"][item.Key], IDs);
        }
    }

    public Dictionary<string, List<string>> knowledgeCharacterIDs = new Dictionary<string, List<string>>();

    public void ImplementIDs()
    {
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
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["Key"] = this.name;
        foreach (KnowledgeInstance item in Known.Items)
        {
            for (int i = 0; i < item.KnownByCharacters.Count; i++)
            {
                node["Knowledge"][item.Key][i] = item.KnownByCharacters[i].ID;
            }
        }

        return node;
    }
}
