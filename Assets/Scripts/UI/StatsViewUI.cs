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

    private void OnEnable()
    {
        StartCoroutine(UpdateState());
    }

    IEnumerator UpdateState()
    {
        yield return new WaitForSeconds(1f);

        RefreshGold();
        RefreshRep();
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
