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

    private void Start()
    {
        GameStorage.Instance.OnMoneyChanged.AddListener(RefreshGold);
        GameStorage.Instance.OnReputationChanged.AddListener(RefreshRep);

        RefreshGold();
        RefreshRep();
    }

    void RefreshGold()
    {
        GoldText.text = GameStorage.Instance.Gold.ToString()+"c";
    }

    void RefreshRep()
    {
        ReputationText.text = GameStorage.Instance.Reputation.ToString();
    }
}
