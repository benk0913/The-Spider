using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
    TextMeshProUGUI TurnText;

    [SerializeField]
    TextMeshProUGUI WeekText;

    [SerializeField]
    UnityEvent OnGoldChanged;

    [SerializeField]
    UnityEvent OnConnectionsChanged;

    [SerializeField]
    UnityEvent OnRumorsChanged;

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
        }
    }


    void RefreshGold()
    {
        if (GoldText.text != CORE.PC.Gold.ToString() + "c")
        {
            GoldText.text = CORE.PC.Gold.ToString() + "c";
            OnGoldChanged?.Invoke();
        }
    }

    void RefreshConnections()
    {
        if (ConnectionsText.text != CORE.PC.Connections.ToString())
        {
            ConnectionsText.text = CORE.PC.Connections.ToString();
            OnConnectionsChanged?.Invoke();
        }
    }

    void RefreshRumors()
    {
        if (RumorsText.text != CORE.PC.Rumors.ToString())
        {
            RumorsText.text = CORE.PC.Rumors.ToString();
            OnRumorsChanged?.Invoke();
        }
    }
}
