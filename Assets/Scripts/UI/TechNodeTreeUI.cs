using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TechNodeTreeUI : NodeTreeUI
{

    public void Show()
    {
        this.gameObject.SetActive(true);
        ShowTechHirarchy(CORE.Instance.TechTree);
    }

    public virtual void ShowTechHirarchy(TechTreeItem rootItem)
    {
        TechNodeTreeUIInstance rootNode = GenerateTechNode(null, rootItem);

        GenerateTree(rootNode);
    }

    protected virtual TechNodeTreeUIInstance GenerateTechNode(TechNodeTreeUIInstance parent, TechTreeItem item)
    {
        TechNodeTreeUIInstance node = new TechNodeTreeUIInstance();
        node.Item = item;
        node.Parent = parent;

        foreach (TechTreeItem techItem in node.Item.Children)
        {
            node.Children.Add(GenerateTechNode(node, techItem));
            
        }

        return node;
    }



    protected override IEnumerator GenerateTreeRoutine(NodeTreeUIInstance origin)
    {
        yield return StartCoroutine(base.GenerateTreeRoutine(origin));

        yield return 0;

        yield return StartCoroutine(SetItemsUI((TechNodeTreeUIInstance)origin));

    }

    protected virtual IEnumerator SetItemsUI(TechNodeTreeUIInstance node)
    {
        node.nodeObject.transform.GetChild(0).GetChild(0).GetComponent<TechTreeItemUI>().SetItem(node.Item);

        yield return 0;

        for (int i = 0; i < node.Children.Count; i++)
        {
            yield return StartCoroutine(SetItemsUI((TechNodeTreeUIInstance) node.Children[i]));
        }
    }
}

public class TechNodeTreeUIInstance : NodeTreeUIInstance
{
    public TechTreeItem Item;
}
