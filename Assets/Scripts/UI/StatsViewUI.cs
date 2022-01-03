using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StatsViewUI : MonoBehaviour
{
    public static StatsViewUI Instance;

    [SerializeField]
    public TextMeshProUGUI GoldText;
    
    [SerializeField]
    public TextMeshProUGUI ConnectionsText;

    [SerializeField]
    public TextMeshProUGUI RumorsText;

    [SerializeField]
    public TextMeshProUGUI ReputationText;

    [SerializeField]
    public TextMeshProUGUI ProgressText;

    [SerializeField]
    public Image ReputationIcon;

    [SerializeField]
    TooltipTargetUI ReputationTooltip;

    [SerializeField]
    TextMeshProUGUI TurnText;

    [SerializeField]
    TextMeshProUGUI WeekText;

    [SerializeField]
    UnityEvent OnGoldChanged;

    [SerializeField]
    UnityEvent OnConnectionsChanged;

    [SerializeField]
    UnityEvent OnRumorsChanged;

    [SerializeField]
    UnityEvent OnProgressChanged;

    [SerializeField]
    UnityEvent OnReputationChanged;

    private void OnEnable()
    {
        StartCoroutine(UpdateState());
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("PassTimeComplete", OnTurnPassed);
        CORE.Instance.SubscribeToEvent("GameLoadComplete", OnTurnPassed);
    }

    void OnTurnPassed()
    {
        TurnText.text = GameClock.Instance.CurrentTurn.ToString();
        WeekText.text = GameClock.Instance.CurrentWeek.ToString();
    }

    IEnumerator UpdateState()
    {
        yield return 0;
        
        while (CORE.Instance.isLoading)
        {
            yield return 0;
        }

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            RefreshGold();
            RefreshConnections();
            RefreshRumors();
            RefreshProgress();
            RefreshReputation();
        }
    }


    public void RefreshGold()
    {
        if (GoldText.text != CORE.PC.CGold.ToString() + "c")
        {
            AudioControl.Instance.PlayInPosition("resource_collect", transform.position);
            GoldText.text = CORE.PC.CGold.ToString() + "c";
            OnGoldChanged?.Invoke();
        }
    }

    public void RefreshConnections()
    {
        if (ConnectionsText.text != CORE.PC.CConnections.ToString())
        {
            AudioControl.Instance.PlayInPosition("resource_collect", transform.position);
            ConnectionsText.text = CORE.PC.CConnections.ToString();
            OnConnectionsChanged?.Invoke();
        }
    }

    public void RefreshRumors()
    {
        if (RumorsText.text != CORE.PC.CRumors.ToString())
        {
            AudioControl.Instance.PlayInPosition("resource_collect", transform.position);
            RumorsText.text = CORE.PC.CRumors.ToString();
            OnRumorsChanged?.Invoke();
        }
    }

    public void RefreshProgress()
    {
        if (ProgressText.text != CORE.PC.CProgress.ToString())
        {
            AudioControl.Instance.PlayInPosition("resource_collect", transform.position);
            ProgressText.text  = System.String.Format("{0:n0}", CORE.PC.CProgress);
            OnProgressChanged?.Invoke();
        }
    }

    public void RefreshReputation()
    {
        if(CORE.PC.Reputation < 0)
        {
            TutorialScreenUI.Instance.Show("BadReputation");
        }

        ReputationInstance instance = CORE.Instance.Database.GetReputationType(CORE.PC.Reputation);

        if (instance.name != ReputationText.text)
        {
            List<TooltipBonus> bonuses = new List<TooltipBonus>();

            foreach(ReputationInstance repInst in CORE.Instance.Database.ReputationTypes)
            {
                Sprite icon = ResourcesLoader.Instance.GetSprite(repInst == instance ? "pointing" : "circle");
                bonuses.Add(new TooltipBonus(repInst.name, icon));
            }

            string tooltipText = "Your Reputation: " + CORE.PC.Reputation;

            if (instance.AgentRelationModifier != 0)
            {
                tooltipText += "\n Relation Modifier: " + instance.AgentRelationModifier;
            }

            if (instance.RecruitExtraCost != 0)
            {
                tooltipText += "\n Recruitment Cost Penalty: " + instance.RecruitExtraCost;
            }

            ReputationTooltip.SetTooltip(tooltipText, bonuses);

            ReputationText.text = instance.name;
            ReputationText.color = instance.color;
            ReputationIcon.color = instance.color;

            OnReputationChanged?.Invoke();
        }


    }
}
