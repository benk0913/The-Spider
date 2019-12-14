using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QOSchemeWin", menuName = "DataObjects/Quests/QuestObjectives/QOSchemeWin", order = 2)]
public class QOSchemeWin : QuestObjective
{
    public SchemeType Scheme;

    [System.NonSerialized]
    public LocationEntity TargetLocation;

    [System.NonSerialized]
    public Character TargetCharacter;

    public Property RandomTargetProperty;
    public Trait[] RandomTargetTraits;
    public int RandomCharacterMinAge;
    public int RandomCharacterMaxAge;
    public int RandomCharacterGender = -1;
    public bool RandomCharacterGenerateNew = false;

    public override QuestObjective CreateClone()
    {
        QOSchemeWin clone = (QOSchemeWin) base.CreateClone();
        
        if(clone.RandomTarget)
        {
            if (Scheme.TargetIsLocation)
            {
                List<LocationEntity> relevantLocations = CORE.Instance.Locations.FindAll(x => x.CurrentProperty == RandomTargetProperty);

                foreach (Trait trait in RandomTargetTraits)
                {
                    relevantLocations.RemoveAll(x => !x.Traits.Contains(trait));
                }

                clone.TargetLocation = relevantLocations[Random.Range(0, relevantLocations.Count)];
            }
            else
            {
                if (RandomCharacterGenerateNew)
                {
                    clone.TargetCharacter = CORE.Instance.GenerateCharacter(
                        RandomCharacterGender, 
                        RandomCharacterMinAge, 
                        RandomCharacterMaxAge, 
                        CORE.Instance.GetRandomLocation());
                }
                else
                {
                    List<Character> relevantCharacters = CORE.Instance.Characters.FindAll(x =>
                    x.Age > RandomCharacterMinAge
                    && x.Age < RandomCharacterMaxAge
                    && (RandomCharacterGender != -1 && (int)x.Gender == RandomCharacterGender));

                    foreach (Trait trait in RandomTargetTraits)
                    {
                        relevantCharacters.RemoveAll(x => !x.Traits.Contains(trait));
                    }

                    clone.TargetCharacter = relevantCharacters[Random.Range(0, relevantCharacters.Count)];
                }
            }
        }

        return clone;
    }

    bool valid = false;
    bool subscribed = false;

    public override bool Validate()
    {
        if (!subscribed)
        {
            CORE.Instance.OnSchemeWin.AddListener(OnSchemeWin);
        }

        if (valid)
        {
            subscribed = false;
            CORE.Instance.OnSchemeWin.RemoveListener(OnSchemeWin);
            return true;
        }

        return false;
    }

    void OnSchemeWin(SchemeType schemeType, LocationEntity targetLocation = null, Character targetCharacter = null)
    {
        if(schemeType.GetType() != Scheme.GetType())
        {
            return;
        }

        if(schemeType.TargetIsLocation)
        {
            if (targetLocation != this.TargetLocation)
            {
                return;
            }
        }
        else
        {
            if (targetCharacter != this.TargetCharacter)
            {
                return;
            }
        }
        
        valid = true;
    }

}