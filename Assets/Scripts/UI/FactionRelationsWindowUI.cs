using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionRelationsWindowUI : MonoBehaviour
{
    public static FactionRelationsWindowUI Instance;

    public Faction CurrentFaction;

    [SerializeField]
    Transform RelationsContainer;

    [SerializeField]
    FactionPortraitUI CurrentFactionPortrait;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (MouseLook.Instance == null) return;

        MouseLook.Instance.CurrentWindow = null;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Show(Faction ofFaction)
    {
        CurrentFaction = ofFaction;
        this.gameObject.SetActive(true);

        RefreshUI();
    }

    void RefreshUI()
    {
        CurrentFactionPortrait.SetInfo(CurrentFaction);

        ClearContainer();

        foreach(Faction faction in CORE.Instance.Factions)
        {
            if(faction == CurrentFaction)
            {
                continue;
            }

            GameObject itemObj = ResourcesLoader.Instance.GetRecycledObject("FactionRelationItemUI");
            itemObj.transform.SetParent(RelationsContainer, false);
            itemObj.GetComponent<FactionRelationItemUI>().SetInfo(CurrentFaction, faction);
        }
    }

    public void ClearContainer()
    {
        while (RelationsContainer.childCount > 0)
        {
            RelationsContainer.GetChild(0).gameObject.SetActive(false);
            RelationsContainer.GetChild(0).SetParent(transform);
        }
    }
}
