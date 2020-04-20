using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class VictoryWindowUI : MonoBehaviour
{
    [SerializeField]
    Transform AvatarContainer;

    [SerializeField]
    Transform StatsContainer;

    [SerializeField]
    GameObject StatsPanel;

    public static VictoryWindowUI Instance;

    private void Awake()
    {
        Instance = this;

        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);

        ClearContainer(AvatarContainer);

        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject(CORE.PC.CurrentFaction.FactionSelectionPrefab);
        tempObj.transform.SetParent(AvatarContainer, false);
        tempObj.transform.localScale = Vector3.one;
        tempObj.transform.position = AvatarContainer.position;
    }

    public void ShowStats()
    {
        StatsPanel.gameObject.SetActive(true);
        ClearContainer(StatsContainer);

        List<LocationEntity> Properties = CORE.PC.PropertiesInCommand;
        List<Character> Agents = CORE.PC.CharactersInCommand;
        EndGameStatUI tempStat;

        //EndGameStatsVisual
        foreach (EndGameParameter parameter in CORE.PlayerFaction.EndGameUniqueParameters)
        {
            tempStat = ResourcesLoader.Instance.GetRecycledObject("EndGameStatsVisual").GetComponent<EndGameStatUI>();
            tempStat.Show(
                parameter.Title,
                parameter.Description,
                parameter.GetIcon(),
                parameter.GetValue());
            tempStat.transform.SetParent(StatsContainer, false);
        }

        tempStat = ResourcesLoader.Instance.GetRecycledObject("EndGameStatPortrait").GetComponent<EndGameStatUI>();
        Agents = Agents.OrderBy(x => x.PropertiesInCommand).ToList();
        tempStat.Show(
            "Most Powerful Agent",
            "The Agent With Most Properties Under Its Control.",
            null,
            Agents[0].PropertiesInCommand.Count.ToString(),
            Agents[0]);
        tempStat.transform.SetParent(StatsContainer, false);

        tempStat = ResourcesLoader.Instance.GetRecycledObject("EndGameStatPortrait").GetComponent<EndGameStatUI>();
        Agents = Agents.OrderBy(x=> x.TotalBonusScore).ToList();
        tempStat.Show(
            "Most Professional Agent",
            "The Agent With Most Skill Points In Total.",
            null,
            Agents[0].TotalBonusScore.ToString(),
            Agents[0]);
        tempStat.transform.SetParent(StatsContainer, false);

        tempStat = ResourcesLoader.Instance.GetRecycledObject("EndGameStatsParameter").GetComponent<EndGameStatUI>();
        tempStat.Show(
            "Days Passed",
            "The Number Of Business Joints And Properties Under Your Command.", 
            ResourcesLoader.Instance.GetSprite("hourglass"),
            GameClock.Instance.CurrentDay.ToString());
        tempStat.transform.SetParent(StatsContainer, false);

        tempStat = ResourcesLoader.Instance.GetRecycledObject("EndGameStatsParameter").GetComponent<EndGameStatUI>();
        tempStat.Show(
            "Properties",
            "The Number Of Business Joints And Properties Under Your Command.",
            ResourcesLoader.Instance.GetSprite("house"),
            Properties.Count.ToString());
        tempStat.transform.SetParent(StatsContainer, false);

        tempStat = ResourcesLoader.Instance.GetRecycledObject("EndGameStatsParameter").GetComponent<EndGameStatUI>();
        int totalGold = 0;
        Properties.ForEach(x => totalGold += x.CurrentAction.GoldGenerated);
        tempStat.Show(
            "Gold Per Turn", "The amount of gold generated each turn.",
            ResourcesLoader.Instance.GetSprite("receive_money"),
            totalGold.ToString());
        tempStat.transform.SetParent(StatsContainer, false);

        tempStat = ResourcesLoader.Instance.GetRecycledObject("EndGameStatsParameter").GetComponent<EndGameStatUI>();
        int totalConnections = 0;
        Properties.ForEach(x => totalConnections += x.CurrentAction.ConnectionsGenerated);
        tempStat.Show(
            "Connections Per Turn", "The number of connections generated each turn.",
            ResourcesLoader.Instance.GetSprite("connections"),
            totalConnections.ToString());
        tempStat.transform.SetParent(StatsContainer, false);

        tempStat = ResourcesLoader.Instance.GetRecycledObject("EndGameStatsParameter").GetComponent<EndGameStatUI>();
        int totalRumors = 0;
        Properties.ForEach(x => totalRumors += x.CurrentAction.RumorsGenerated);
        tempStat.Show(
            "Rumors Per Turn", "The number of rumours generated each turn.",
            ResourcesLoader.Instance.GetSprite("earIcon"),
            totalRumors.ToString());
        tempStat.transform.SetParent(StatsContainer, false);

        tempStat = ResourcesLoader.Instance.GetRecycledObject("EndGameStatsParameter").GetComponent<EndGameStatUI>();
        int totalProgression = 0;
        Properties.ForEach(x => totalProgression += x.CurrentAction.ProgressGenerated);
        tempStat.Show(
            "Progression Per Turn", "The number of progression points generated each turn.",
            ResourcesLoader.Instance.GetSprite("scroll-unfurled"),
            totalProgression.ToString());
        tempStat.transform.SetParent(StatsContainer, false);




    }

    void ClearContainer(Transform container)
    {
        while (container.childCount > 0)
        {
            container.transform.GetChild(0).gameObject.SetActive(false);
            container.transform.GetChild(0).SetParent(transform);
        }
    }

    public void Confirm()
    {
        MapViewManager.Instance.MapFocusView.Deactivate();
        this.gameObject.SetActive(false);
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        WarningWindowUI.Instance.Show("Go, nobody will miss you...", Application.Quit);
    }
}
