using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpyAroundComplete", menuName = "DataObjects/TechTree/Item", order = 2)]
public class TechTreeItem : ScriptableObject
{
    [TextArea(4,6)]
    public string Description;

    public Sprite Icon;

    public int Price;

    public List<TechTreeItem> Children;
    public TechTreeItem Parent;
    public List<TechTreeItem> SubParents = new List<TechTreeItem>(); //Additional techs required for this one

    public List<Faction> FactionsLocked = new List<Faction>();

    public bool IsResearched;
    public bool IsResearchable
    {
        get
        {
            if(!Parent.IsResearched)
            {
                return false;
            }

            foreach(TechTreeItem subparent in SubParents)
            {
                if(!subparent.IsResearched)
                {
                    return false;
                }
            }

            return true;
        }
    }
    /// <summary>
    /// Cloning will only be done downards, this clone will not have a cloned parent, so it is recommended to clone the tree from the top.
    /// </summary>
    /// <returns></returns>
    public TechTreeItem Clone()
    {
        TechTreeItem cloneInst = Instantiate(this);
        cloneInst.name = this.name;
        for(int i=0;i<Children.Count;i++)
        {
            Children[i] = Children[i].Clone();
        }

        return cloneInst;
    }
}
