using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DuelProc", menuName = "DataObjects/DuelProcs/DuelProc", order = 2)]
public class DuelProc : ScriptableObject
{
    public TechTreeItem RequiredTech;
    
    public List<PlotMethod> ForMethodCondition;

    public bool isGoodForDefenders
    {
        get
        {
            return (isGoodFor == DuelTeam.Player && !PlottingDuelUI.Instance.IsPlayerAttacker)
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

    [SerializeField]
    protected GameObject EffectOnTarget;

    public virtual IEnumerator Execute()
    {
        yield return 0;
    }

    public virtual bool RollChance()
    {
        if (CORE.Instance.DEBUG)
        {
            Debug.Log("$$$ - Rolled Chance For - " + this.name);
        }

        return Random.Range(0f, 1f) < Chance;
    }

    public virtual bool PassedConditions()
    {
        if (CORE.Instance.DEBUG)
        {
            Debug.Log("$$$ - Attempted Conditions - " + this.name);
        }

        TechTreeItem techItem = CORE.Instance.TechTree.Find(x => x.name == RequiredTech.name);
        if(!techItem.IsResearched)
        {
            return false;
        }

        if (ForMethodCondition != null && ForMethodCondition.Count > 0 && !ForMethodCondition.Contains(PlottingDuelUI.Instance.CurrentPlot.Method))
        {
            return false;
        }

        return true;
    }

    public void GenerateEffectOnPortrait(PortraitUI portrait)
    {
        if (EffectOnTarget == null)
        {
            return;
        }

        if(portrait == null)
        {
            return;
        }


        GameObject effect = ResourcesLoader.Instance.GetRecycledObject(EffectOnTarget);
        effect.transform.SetParent(portrait.transform.parent);
        effect.transform.localScale = Vector3.one;
        effect.transform.position = portrait.transform.position;
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
