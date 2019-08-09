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
}