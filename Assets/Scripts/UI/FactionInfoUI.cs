using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FactionInfoUI : MonoBehaviour
{
    public static FactionInfoUI Instance;

    [SerializeField]
    FactionPortraitUI FactionPortrait;

    [SerializeField]
    PortraitUI LeaderPortrait;

    [SerializeField]
    TextMeshProUGUI FactionNameUI;

    [SerializeField]
    TextMeshProUGUI RelationsWithPlayerText;

    [SerializeField]
    Transform PropertiesOwnedContainer;

    [SerializeField]
    Transform KnownInformationContainer;

    Faction CurrentFaction;


    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("HideMap", Hide);
    }
    public void Hide()
    {
        CORE.Instance.UnsubscribeFromEvent("PassTimeComplete", RefreshUI);
        this.gameObject.SetActive(false);
    }

    public void Show(Faction faction)
    {
        faction = CORE.Instance.Factions.Find(x => x.name == faction.name);

        CharacterInfoUI.Instance.Hide();
        LocationInfoUI.Instance.Hide();

        if (faction == null)
        {
            return;
        }

        this.gameObject.SetActive(true);

        CurrentFaction = faction;
        RefreshUI();

        CORE.Instance.SubscribeToEvent("PassTimeComplete", RefreshUI);
    }

    public void RefreshUI()
    {
        if(CurrentFaction == null)
        {
            return;
        }

        FactionPortrait.SetInfo(CurrentFaction);

        if (CurrentFaction.FactionHead != null)
        {
            LeaderPortrait.SetCharacter(CORE.Instance.Characters.Find(x => x.name == CurrentFaction.FactionHead.name));
        }
        else
        {
            LeaderPortrait.SetCharacter(null);
        }

        if (CurrentFaction.Known != null)
        {
            if (CurrentFaction.Known.IsKnown("Existance", CORE.PC))
            {
                FactionNameUI.text = CurrentFaction.name;
            }
            else
            {
                FactionNameUI.text = "Unknown Faction";
            }

            ClearContainer(KnownInformationContainer);
            foreach (KnowledgeInstance kInstance in CurrentFaction.Known.Items)
            {
                KnownInstanceUI uiInstance = ResourcesLoader.Instance.GetRecycledObject("KnownInstanceUI").GetComponent<KnownInstanceUI>();
                uiInstance.SetInfo(kInstance.Key, kInstance.Description, kInstance.IsKnownByCharacter(CORE.PC));
                uiInstance.transform.SetParent(KnownInformationContainer, false);
            }
        }

        ClearContainer(PropertiesOwnedContainer);
        if(CurrentFaction.FactionHead != null)
        {
            Character factionHead = CORE.Instance.Characters.Find(x=>x.name == CurrentFaction.FactionHead.name);
            List<LocationEntity> PropertiesOwned = factionHead.PropertiesInCommand;

            for (int i = 0; i < PropertiesOwned.Count; i++)
            {
                GameObject tempPortrait = ResourcesLoader.Instance.GetRecycledObject("LocationPortraitUI");
                tempPortrait.transform.SetParent(PropertiesOwnedContainer, false);
                tempPortrait.transform.localScale = Vector3.one;
                tempPortrait.GetComponent<LocationPortraitUI>().SetLocation(PropertiesOwned[i]);
            }
        }

        if(CurrentFaction.Relations != null)
        {
            if (CurrentFaction == CORE.PC.CurrentFaction)
            {
                RelationsWithPlayerText.text = "--";
                RelationsWithPlayerText.color = Color.yellow;
            }
            else
            {
                int relation = CurrentFaction.Relations.GetRelations(CORE.PC.CurrentFaction).TotalValue;

                RelationsWithPlayerText.text = relation.ToString();

                if (relation > 3)
                {
                    RelationsWithPlayerText.color = Color.green;
                }
                else if (relation < -3)
                {
                    RelationsWithPlayerText.color = Color.red;
                }
                else
                {
                    RelationsWithPlayerText.color = Color.yellow;
                }
            }
        }
    }

    public void ShowFactionHirarachy()
    {
        if(!CurrentFaction.Known.IsKnown("Existance",CORE.PC))
        {
            GlobalMessagePrompterUI.Instance.Show("This faction is not known to you. (Know of a member first)", 1f, Color.red);
            return;
        }

        Character factionLeader = CORE.Instance.Characters.Find(x => x.name == CurrentFaction.FactionHead.name);

        SelectAgentWindowUI.Instance.Show(
                (Character character) => { CharacterInfoUI.Instance.ShowInfo(character); }
                , x => { return x.TopEmployer == factionLeader && x.TopEmployer != x; }, factionLeader.CurrentFaction.name , factionLeader);

        Hide();
    }

    public void ShowFactionRelations()
    {
        FactionRelationsWindowUI.Instance.Show(CurrentFaction);
    }


    void ClearContainer(Transform container)
    {
        while (container.childCount > 0)
        {
            container.GetChild(0).gameObject.SetActive(false);
            container.GetChild(0).SetParent(transform);
        }
    }

}
