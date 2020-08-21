using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class NodeTreeUI : MonoBehaviour
{
    [SerializeField]
    protected Transform RootContainer;

    [SerializeField]
    protected GameObject NodePrefab;

    [SerializeField]
    protected float NodeSpacingX = 1f;

    [SerializeField]
    protected float NodeSpacingY = 1f;

    public bool DEBUG = false;
    private void OnEnable()
    {
        if (DEBUG)
        {
            GenerateRandom();
        }
    }

    void GenerateRandom()
    {
        NodeTreeUIInstance parent = GenerateRandomSubnode(null, 5);
        GenerateTree(parent);
    }

    NodeTreeUIInstance GenerateRandomSubnode(NodeTreeUIInstance parent = null, int depth = 0)
    {
        NodeTreeUIInstance node = new NodeTreeUIInstance();

        node.Parent = parent;

        depth--;

        if(depth <= 0)
        {
            return node;
        }

        int randomChildcount = Random.Range(0, 5);

        for(int i=0;i<randomChildcount;i++)
        {
            node.Children.Add(GenerateRandomSubnode(node, depth));
        }

        return node;
    }

    public virtual void GenerateTree(NodeTreeUIInstance origin)
    {
        StopAllCoroutines();
        StartCoroutine(GenerateTreeRoutine(origin));
    }

    protected virtual void ClearBranch(Transform root)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Destroy(root.GetChild(i).gameObject);
        }
    }

    protected virtual IEnumerator GenerateTreeRoutine(NodeTreeUIInstance origin)
    {
        ClearBranch(RootContainer);

        //yield return 0;

        yield return StartCoroutine(GenerateNode(origin));

        yield return 0;

        LayoutRebuilder.ForceRebuildLayoutImmediate(origin.nodeObject.GetComponent<RectTransform>());


        yield return 0;

        yield return 0;

        LayoutRebuilder.ForceRebuildLayoutImmediate(origin.nodeObject.GetComponent<RectTransform>());

        //yield return StartCoroutine(GenerateLines(origin));

        //yield return StartCoroutine(GenerateLines(origin));

        //CORE.Instance.DelayedInvokation(0.5F, () => { GenerateLinesInstant(origin);  });

    }

    protected virtual IEnumerator GenerateNode(NodeTreeUIInstance node)
    {
        GameObject tempObj = Instantiate(NodePrefab);
        if (node.Parent == null)
        {
            tempObj.transform.SetParent(RootContainer, false);
        }
        else
        {
            tempObj.transform.SetParent(node.Parent.nodeObject.transform.GetChild(1).transform, false);
        }
        tempObj.transform.localScale = Vector3.one;

        node.nodeObject = tempObj;

        //yield return 0;

        for (int i=0;i<node.Children.Count;i++)
        {
            yield return StartCoroutine(GenerateNode(node.Children[i]));
        }
    }

    protected void GenerateLinesInstant(NodeTreeUIInstance node)
    {
        if (node.Parent != null)
        {
            List<Vector2> linePoints = new List<Vector2>();

            Vector2 localPortrait = node.nodeObject.transform.InverseTransformPoint(node.nodeObject.transform.GetChild(0).position);

            Vector2 parentPortrait = node.nodeObject.transform.InverseTransformPoint(node.Parent.nodeObject.transform.GetChild(0).position);
            parentPortrait = new Vector2(parentPortrait.x, parentPortrait.y - 140);

            Vector2 midpointA = Vector2.Lerp(localPortrait, new Vector2(localPortrait.x, parentPortrait.y), 0.5f);
            Vector2 midpointB = Vector2.Lerp(parentPortrait, new Vector2(localPortrait.x, parentPortrait.y), 0.5f);

            //linePoints.Add(localPortrait);
            //linePoints.Add(midpointA);
            //linePoints.Add(midpointB);
            //linePoints.Add(parentPortrait);

            for (float i = 0f; i < 1f; i += 0.1f)
            {
                linePoints.Add(Util.SplineLerpY(localPortrait, parentPortrait, 120f, i));
            }

            node.nodeObject.GetComponent<UILineRenderer>().Points = linePoints.ToArray();
        }

        //yield return 0;

        for (int i = 0; i < node.Children.Count; i++)
        {
            GenerateLinesInstant(node.Children[i]);
        }
    }

    protected virtual IEnumerator GenerateLines(NodeTreeUIInstance node)
    {
        if (node.Parent != null)
        {
            List<Vector2> linePoints = new List<Vector2>();

            Vector2 localPortrait = node.nodeObject.transform.InverseTransformPoint(node.nodeObject.transform.GetChild(0).position);
            
            Vector2 parentPortrait = node.nodeObject.transform.InverseTransformPoint(node.Parent.nodeObject.transform.GetChild(0).position);
            parentPortrait = new Vector2(parentPortrait.x, parentPortrait.y - 140);

            Vector2 midpointA = Vector2.Lerp(localPortrait, new Vector2(localPortrait.x, parentPortrait.y), 0.5f);
            Vector2 midpointB = Vector2.Lerp(parentPortrait, new Vector2(localPortrait.x, parentPortrait.y), 0.5f);

            //linePoints.Add(localPortrait);
            //linePoints.Add(midpointA);
            //linePoints.Add(midpointB);
            //linePoints.Add(parentPortrait);

            for(float i=0f;i<1f;i+=0.1f)
            {
                linePoints.Add(Util.SplineLerpX(localPortrait, parentPortrait, 120f, i));
            }

            node.nodeObject.GetComponent<UILineRenderer>().Points = linePoints.ToArray();
        }

        //yield return 0;

        for (int i = 0; i < node.Children.Count; i++)
        {
            yield return StartCoroutine(GenerateLines(node.Children[i]));
        }
    }


}

public class NodeTreeUIInstance
{
    public List<NodeTreeUIInstance> Children = new List<NodeTreeUIInstance>();
    public NodeTreeUIInstance Parent;
    public GameObject nodeObject;
}
