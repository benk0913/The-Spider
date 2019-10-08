using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class StatsViewUI : MonoBehaviour
{
    public static StatsViewUI Instance;

    [SerializeField]
    TextMeshProUGUI GoldText;
    
    [SerializeField]
    TextMeshProUGUI ReputationText;

    [SerializeField]
    TextMeshProUGUI TurnText;

    [SerializeField]
    TextMeshProUGUI WeekText;

    [SerializeField]
    UnityEvent OnGoldChanged;

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
        GameClock.Instance.OnTurnPassed.AddListener(OnTurnPassed);
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
            RefreshRep();
        }
    }

    void RefreshGold()
    {
        if(GoldText.text != CORE.PC.Gold.ToString()+"c")
        {
            GoldText.text = CORE.PC.Gold.ToString() + "c";
            OnGoldChanged?.Invoke();
        }
    }

    void RefreshRep()
    {
        ReputationText.text = "None";
    }
}
