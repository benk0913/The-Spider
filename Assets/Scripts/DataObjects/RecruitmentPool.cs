using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;


[CreateAssetMenu(fileName = "RecruitmentPool", menuName = "DataObjects/RecruitmentPool", order = 2)]
public class RecruitmentPool : ScriptableObject, ISaveFileCompatible
{
    public Sprite Icon;

    [TextArea(2,3)]
    public string Description;

    public List<Trait> MustHaveTraits = new List<Trait>();
    public List<Trait> OptionalTraits = new List<Trait>();

    public int PoolCap = 5;

    public List<Character> Characters = new List<Character>();

    public Faction RequiresFaction;
    public TechTreeItem RequiresTech;
    public int MinReputation = -10;

    public int TraitCap = 2;

    public int Cost = 3;


    public RecruitmentPool Clone()
    {
        RecruitmentPool clone = Instantiate(this);
        clone.name = this.name;
        clone.Setup();
        return clone;
    }

    public void Setup()
    {
        while(Characters.Count < PoolCap)
        {
            GenerateCharacter();
        }
    }

    void GenerateCharacter()
    {
        Character temp = CORE.Instance.GenerateCharacter(-1,15,70);

        foreach (Trait mustTrait in MustHaveTraits)
        {
            temp.AddTrait(mustTrait);
        }

        if (temp.Age > 14)
        {
            for (int i = 0; i < TraitCap; i++)
            {
                if (Random.Range(0, 2) == 0)
                {
                    continue;
                }

                Trait optionalTrait = OptionalTraits[Random.Range(0, OptionalTraits.Count)];

                if (temp.Traits.Find(x => x.name == optionalTrait.name) != null)
                {
                    continue;
                }

                temp.AddTrait(optionalTrait);
            }
        }

        Characters.Add(temp);

    }

    public void Remove(Character character)
    {

        Characters.Remove(character);
        GenerateCharacter();
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["Key"] = this.name;

        for(int i=0;i<Characters.Count;i++)
        {
            node["Characters"][i] = Characters[i].ToJSON();
        }

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        Characters.Clear();

        for (int i = 0; i < node["Characters"].Count; i++)
        {
            Character temp = CORE.Instance.GenerateSimpleCharacter();
            temp.FromJSON(node["Characters"][i]);
            Characters.Add(temp);
        }
    }

    public void ImplementIDs()
    {
        
    }
}
