using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PromotionControlPanelUI : MonoBehaviour
{

    public TextMeshProUGUI PercentageText;

    public PortraitUI CharPortrait;

    public Image PrecentageFill;

    Character CurrentCharacter;

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("PassTimeComplete", Refresh);
        CORE.Instance.SubscribeToEvent("CreditStolen", Refresh);
        DefaultView();
    }

    void Refresh()
    {
        List<Character> potentialThreats = CORE.PC.PropertiesInCommand[0].EmployeesCharacters.OrderByDescending(x => x.CProgress).ToList();
        if(potentialThreats == null || potentialThreats.Count == 0)
        {
            DefaultView();
            return;
        }

        CharPortrait.SetCharacter(potentialThreats[0]);
        float percentage = (potentialThreats[0].CProgress / 50f);
        PercentageText.text = Mathf.RoundToInt(percentage*100f)+"%";
        PrecentageFill.fillAmount = percentage;
    }

    void DefaultView()
    {
        CharPortrait.SetCharacter(null);
        PercentageText.text = "--";
        PrecentageFill.fillAmount = 0f;
    }

    public void ShowPromotionTree()
    {
        AgentPromotionWindowUI.Instance.Show(
            null,
            x => { return x.TopEmployer == CORE.PC && x.TopEmployer != x; },
            "Constabulary");
    }
}
