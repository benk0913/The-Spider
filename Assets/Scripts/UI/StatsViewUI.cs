using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsViewUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI GoldText;
    
    [SerializeField]
    TextMeshProUGUI ReputationText;

    [SerializeField]
    TextMeshProUGUI TurnText;

    [SerializeField]
    TextMeshProUGUI WeekText;

    private void OnEnable()
    {
        StartCoroutine(UpdateState());
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
        GoldText.text = CORE.PC.Gold.ToString()+"c";
    }

    void RefreshRep()
    {
        ReputationText.text = "None";
    }
}
