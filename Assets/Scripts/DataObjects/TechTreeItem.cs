using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[CreateAssetMenu(fileName = "TechTreeItem", menuName = "DataObjects/TechTree/Item", order = 2)]
public class TechTreeItem : ScriptableObject, ISaveFileCompatible
{
    [TextArea(4,6)]
    public string Description;

    public Sprite Icon;

    public int Price;

    public List<TechTreeItem> Children;
    public TechTreeItem Parent;
    public List<TechTreeItem> SubParents = new List<TechTreeItem>(); //Additional techs required for this one

    public List<Faction> FactionsLocked = new List<Faction>();

    public Color BoxColor;

    public bool IsHidden = false;
    public bool IsUnresearchable = false;
    public bool IsResearched;
    public bool IsResearchable
    {
        get
        {
            if(Parent != null && !Parent.IsResearched)
            {
                return false;
            }

            if(IsUnresearchable)
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
        for(int i=0;i< cloneInst.Children.Count;i++)
        {
            cloneInst.Children[i] = cloneInst.Children[i].Clone();
            cloneInst.Children[i].Parent = cloneInst;
        }

        return cloneInst;
    }

    public TechTreeItem Find(Predicate<TechTreeItem> predicate)
    {
        if(predicate(this))
        {
            return this;
        }

        foreach(TechTreeItem item in Children)
        {
            if(item.Find(predicate))
            {
                return item.Find(predicate);
            }
        }

        return null;
    }

    public List<TechTreeItem> FindAll(Predicate<TechTreeItem> predicate = null)
    {
        List<TechTreeItem> Elements = new List<TechTreeItem>();

        if(predicate == null)
        {
            Elements.Add(this);
        }
        else if (predicate(this))
        {
            Elements.Add(this);
        }

        foreach (TechTreeItem item in Children)
        {
            Elements.AddRange(item.FindAll(predicate));
        }

        return Elements;
    }

    public void FromJSON(JSONNode node)
    {
        IsResearched = bool.Parse(node["IsResearched"]);
        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].FromJSON(node["Children"][i]);
        }

    }

    public void ImplementIDs()
    {
        
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["IsResearched"] = this.IsResearched.ToString();
        for(int i=0;i<Children.Count;i++)
        {
            node["Children"][i] = Children[i].ToJSON();
        }

        return node;
    }
}
