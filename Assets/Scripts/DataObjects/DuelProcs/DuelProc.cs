using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DuelProc", menuName = "DataObjects/DuelProcs/DuelProc", order = 2)]
public class DuelProc : ScriptableObject
{
    public bool isGoodForDefenders
    {
        get
        {
            return isGoodFor == DuelTeam.Defenders
                || (isGoodFor == DuelTeam.Player && !PlottingDuelUI.Instance.IsPlayerAttacker)
                || (isGoodFor == DuelTeam.NotPlayer && PlottingDuelUI.Instance.IsPlayerAttacker);
        }
    }

    [SerializeField]
    public DuelTeam isGoodFor;

    public DuelProcType Type;

    [TextArea(3,6)]
    public string Description;

    public Sprite Icon;

    public float Chance = 1f;

    public bool Repeatable;
    
    public virtual IEnumerator Execute()
    {
        yield return 0;
    }

    public virtual bool RollChance()
    {
        return Random.Range(0f, 1f) < Chance;
    }

    public virtual bool PassedConditions()
    {
        return true;
    }
}

[System.Serializable]
public enum DuelTeam
{
    Attackers,
    Defenders,
    Player,
    NotPlayer
}
