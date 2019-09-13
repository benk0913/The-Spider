using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class RumorsPanelUI : MonoBehaviour, ISaveFileCompatible
{
    public static RumorsPanelUI Instance;

    public List<Rumor> AllAvailableRumors = new List<Rumor>();
    public List<Rumor> VisibleRumors = new List<Rumor>();
    public List<Rumor> ArchivedRumors = new List<Rumor>();

    [SerializeField]
    Transform RumorsContainer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameClock.Instance.OnWeekPassed.AddListener(OnWeekPassed);
    }

    public void OnWeekPassed()
    {
        if(GameClock.Instance.CurrentWeek >= CORE.Instance.Database.Timeline.Length)
        {
            return;
        }

        AllAvailableRumors.InsertRange(0, CORE.Instance.Database.Timeline[GameClock.Instance.CurrentWeek].Rumors);
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

            GameObject rumorPanel = ResourcesLoader.Instance.GetRecycledObject("RumorHeadlineUI");
            rumorPanel.transform.SetParent(RumorsContainer, false);
            rumorPanel.GetComponent<RumorHeadlineUI>().SetInfo(randomRumor, this);
        }
    }

    public void Archive(Rumor rumor)
    {
        ArchivedRumors.Add(rumor);
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


    //TODO DEBUG

    public bool DEBUG;

    private void Update()
    {
        if (DEBUG)
        {
            GainRumors(1);
            DEBUG = false;
        }
    }
}
