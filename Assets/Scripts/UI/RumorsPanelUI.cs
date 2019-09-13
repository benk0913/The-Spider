using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumorsPanelUI : MonoBehaviour
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
            rumorPanel.GetComponent<RumorHeadlineUI>().SetInfo(randomRumor);
        }
    }

    public void Load()
    {
        //TODO LoadState
    }

    public void Save()
    {
        //TODO SaveState
    }


}
