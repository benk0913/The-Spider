using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trait", menuName = "DataObjects/Trait", order = 2)]
public class Trait : ScriptableObject
{
    [TextArea(2, 3)]
    public string Description;

    public Sprite icon;

    public List<Bonus> Bonuses = new List<Bonus>();

    public Trait NextTrait;

    public Trait PreviousTrait;

    public Trait[] OppositeTraits;

    public RelationsModifier[] RelationModifiers;

    public float DropChance = 0f;

    public int MinAge = 0;

    public string KnowledgeRumor = "\"What an interesting person!\"";
}

[System.Serializable]
public class Bonus
{
    public BonusType Type;
    public float Value;
}

[System.Serializable]
public class BonusChallenge
{
    public BonusType Type;
    public int ChallengeValue;
    public int RarityValue = 0;
    public bool InvertedChance = false;
}