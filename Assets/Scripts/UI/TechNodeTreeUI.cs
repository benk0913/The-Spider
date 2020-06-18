using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TechNodeTreeUI : NodeTreeUI
{
    public static TechNodeTreeUI Instance;

    public TechNodeTreeUIInstance CurrentRoot;

    [SerializeField]
    ScrollRect CurrentScrollRect;

    [SerializeField]
    GameObject LoadingPanel;

    [SerializeField]
    GameObject FirstNode;

    [SerializeField]
    CanvasGroup CG;

    public float MinScale = 0.5f;
    public float MaxScale = 1f;

    public bool Initialized = false;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("GameLoadComplete", () => { Initialized = false; });
    }

    public void Hide()
    {
        if (AudioControl.Instance != null)
        {
            AudioControl.Instance.StopSound("soundscape_research_tech");
            AudioControl.Instance.UnmuteMusic();
        }

        CG.alpha = 0f;
        CG.interactable = false;
        CG.blocksRaycasts = false;

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0f && FirstNode.transform.localScale.x < MaxScale)
        {
            FirstNode.transform.localScale += Vector3.one *2f* Time.deltaTime;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && FirstNode.transform.localScale.x > MinScale)
        {
            FirstNode.transform.localScale -= Vector3.one * 2f * Time.deltaTime;
        }
    }

    public void Reveal()
    {
        CG.alpha = 1f;
        CG.interactable = true;
        CG.blocksRaycasts = true;
    }

    public void Show()
    {

        AudioControl.Instance.Play("soundscape_research_tech", true);
        AudioControl.Instance.MuteMusic();

        Reveal();

        if (Initialized)
        {
            return;
        }

        ShowTechHirarchy(CORE.Instance.TechTree);
        FirstNode.transform.localScale = Vector3.one * MinScale;

        Initialized = true;
    }

    public virtual void ShowTechHirarchy(TechTreeItem rootItem)
    {
        CurrentRoot = GenerateTechNode(null, rootItem);

        GenerateTree(CurrentRoot);
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

    public void RefreshNodes()
    {
        if(this.CG.interactable == false)
        {
            return;
        }

        TechTreeItemUI[] items = GetComponentsInChildren<TechTreeItemUI>();

        foreach(TechTreeItemUI item in items)
        {
            item.RefreshUI();
        }
    }


    protected override IEnumerator GenerateTreeRoutine(NodeTreeUIInstance origin)
    {
        LoadingPanel.SetActive(true);

        yield return StartCoroutine(base.GenerateTreeRoutine(origin));

        yield return StartCoroutine(SetItemsUI((TechNodeTreeUIInstance)origin));

        yield return 0;

        CurrentScrollRect.horizontalNormalizedPosition = 0f;
        CurrentScrollRect.verticalNormalizedPosition   = 1f;

        LoadingPanel.SetActive(false);
    }

    protected virtual IEnumerator SetItemsUI(TechNodeTreeUIInstance node)
    {
        node.nodeObject.transform.GetChild(0).GetChild(0).GetComponent<TechTreeItemUI>().SetItem(node.Item);

        if (node.Item.IsHidden || node.Item.FactionsHidden.Find(x=>x.name == CORE.PlayerFaction.name) != null)
        {
            node.nodeObject.transform.GetChild(1).GetComponent<Image>().color = Color.clear;
        }
        else
        {
            node.nodeObject.transform.GetChild(1).GetComponent<Image>().color = node.Item.BoxColor;
        }

        for (int i = 0; i < node.Children.Count; i++)
        {
            yield return StartCoroutine(SetItemsUI((TechNodeTreeUIInstance) node.Children[i]));
        }
    }

    public virtual TechNodeTreeUIInstance FindNode(TechTreeItem tech)
    {
        return FindNode(CurrentRoot, tech);
    }

    protected virtual TechNodeTreeUIInstance FindNode(TechNodeTreeUIInstance node, TechTreeItem tech)
    {
        if(node.Item.name == tech.name)
        {
            return node;
        }

        for(int i=0;i<node.Children.Count;i++)
        {
            TechNodeTreeUIInstance potentialNode = FindNode((TechNodeTreeUIInstance)node.Children[i], tech);

            if(potentialNode != null)
            {
                return potentialNode;
            }
        }

        return null;
    }
}

public class TechNodeTreeUIInstance : NodeTreeUIInstance
{
    public TechTreeItem Item;
}
