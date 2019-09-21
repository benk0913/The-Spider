﻿using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class RumorsPanelUI : MonoBehaviour, ISaveFileCompatible
{
    public static RumorsPanelUI Instance;

    public List<Rumor> AllAvailableRumors = new List<Rumor>();
    public List<Rumor> VisibleRumors = new List<Rumor>();
    public List<Rumor> ArchivedRumors = new List<Rumor>();

    [SerializeField]
    Transform RumorsContainer;

    [SerializeField]
    Button visibleRumorsButton;

    [SerializeField]
    Button archivedRumorsButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameClock.Instance.OnWeekPassed.AddListener(OnWeekPassed);
    }

    void OnEnable()
    {
        ShowVisible();
    }

    public void OnWeekPassed()
    {
        if(GameClock.Instance.CurrentWeek < CORE.Instance.Database.Timeline.Length)
        {
            AllAvailableRumors.InsertRange(0, CORE.Instance.Database.Timeline[GameClock.Instance.CurrentWeek].Rumors);
        }

        int rumorsToGenerate = 0;
        foreach (LocationEntity location in CORE.Instance.Locations)
        {
            
            if(location.OwnerCharacter != null && location.OwnerCharacter.TopEmployer == CORE.PC && location.CurrentProperty.Traits.Contains(CORE.Instance.Database.RumorsHubTrait))
            {
                CORE.Instance.ShowHoverMessage("Generated Rumors", null
                    ,location.transform);
                rumorsToGenerate += location.Level;
            }
        }

        GainRumors(rumorsToGenerate);
    }

    public void GainRumors(int amount)
    {
        for(int i=0;i<amount;i++)
        {
            if(AllAvailableRumors.Count == 0)
            {
                return;
            }

            Rumor randomRumor = AllAvailableRumors[Random.Range(0, AllAvailableRumors.Count)];

            VisibleRumors.Add(randomRumor);
            AllAvailableRumors.Remove(randomRumor);

            AddRumorToContainer(randomRumor);
        }
    }

    public void Archive(Rumor rumor)
    {
        VisibleRumors.Remove(rumor);
        ArchivedRumors.Add(rumor);
    }

    public void ShowVisible()
    {
        ClearContainer();

        visibleRumorsButton.interactable = false;
        archivedRumorsButton.interactable = true;

        foreach (Rumor rumor in VisibleRumors)
        {
            AddRumorToContainer(rumor);
        }
    }

    public void ShowArchived()
    {
        ClearContainer();

        visibleRumorsButton.interactable = true;
        archivedRumorsButton.interactable = false;

        foreach (Rumor rumor in ArchivedRumors)
        {
            AddRumorToContainer(rumor, false);
        }
    }

    void AddRumorToContainer(Rumor rumor, bool canArchive = true)
    {
        GameObject rumorPanel = ResourcesLoader.Instance.GetRecycledObject("RumorHeadlineUI");
        rumorPanel.transform.SetParent(RumorsContainer, false);
        rumorPanel.GetComponent<RumorHeadlineUI>().SetInfo(rumor, this, canArchive);
    }

    void ClearContainer()
    {
        while(RumorsContainer.childCount > 0)
        {
            RumorsContainer.GetChild(0).gameObject.SetActive(false);
            RumorsContainer.GetChild(0).SetParent(transform);
        }
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        for (int i = 0; i < AllAvailableRumors.Count; i++)
        {
            node["AllAvailableRumors"][i] = AllAvailableRumors[i].name;
        }

        for (int i = 0; i < VisibleRumors.Count; i++)
        {
            node["VisibleRumors"][i] = VisibleRumors[i].name;
        }

        for (int i = 0; i < ArchivedRumors.Count; i++)
        {
            node["ArchivedRumors"][i] = ArchivedRumors[i].name;
        }

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        AllAvailableRumors.Clear();
        VisibleRumors.Clear();
        ArchivedRumors.Clear();

        for (int i=0;i<node["AllAvailableRumors"].Count;i++)
        {
            AllAvailableRumors.Add(CORE.Instance.Database.GetRumor(node["AllAvailableRumors"][i]));
        }

        for (int i = 0; i < node["VisibleRumors"].Count; i++)
        {
            VisibleRumors.Add(CORE.Instance.Database.GetRumor(node["VisibleRumors"][i]));
        }

        for (int i = 0; i < node["ArchivedRumors"].Count; i++)
        {
            ArchivedRumors.Add(CORE.Instance.Database.GetRumor(node["ArchivedRumors"][i]));
        }

        foreach(Rumor rumor in VisibleRumors)
        {
            GameObject rumorPanel = ResourcesLoader.Instance.GetRecycledObject("RumorHeadlineUI");
            rumorPanel.transform.SetParent(RumorsContainer, false);
            rumorPanel.GetComponent<RumorHeadlineUI>().SetInfo(rumor, this);
        }
    }

    public void ImplementIDs()
    {
        throw new System.NotImplementedException();
    }
}
