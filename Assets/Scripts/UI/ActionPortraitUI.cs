using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionPortraitUI : AgentInteractable, IPointerClickHandler
{
    [SerializeField]
    Image Icon;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    List<PlayerAction> PossiblePlayerActions = new List<PlayerAction>();

    public LongTermTaskEntity CurrentEntity;

    public void SetAction(LongTermTaskEntity entity)
    {
        CurrentEntity = entity;

        Icon.sprite = entity.CurrentTask.Icon;

        TooltipTarget.Text = "<size=24><u>" + entity.CurrentTask.name + "</u></size> ";

        TooltipTarget.Text += "\n <size=18>" + entity.CurrentTask.Description + "</size>";

        TooltipTarget.Text += "<size=16>";

        if (entity.CurrentTask.DefaultResults.Length > 0)
        {
            TooltipTarget.Text += "\n \n <u>Default Results:</u>";

            foreach (AgentAction result in entity.CurrentTask.DefaultResults)
            {
                if (result == null)
                {
                    continue;
                }

                TooltipTarget.Text += "\n" + result.name;

                if (result.Challenge.Type == null)
                {
                    continue;
                }

                float playerBonus = entity.CurrentCharacter.GetBonus(result.Challenge.Type).Value;
                float challengeBonus = result.Challenge.ChallengeValue;
                float rarity = result.Challenge.RarityValue;

                float precentNormalized = challengeBonus / (playerBonus + rarity);

                TooltipTarget.Text += "\n <color=yellow>" + result.name + "</color> - <color=green>" + result.Challenge.Type.name
                    + "</color> <color=yellow> (" + Mathf.RoundToInt(100f * precentNormalized) + "%) </color>";
            }
        }

        if (entity.CurrentTask.PossibleResults.Length > 0)
        {
            TooltipTarget.Text += "\n \n <u>Possible Results:</u>";

            foreach (AgentAction result in entity.CurrentTask.PossibleResults)
            {
                if (result == null)
                {
                    continue;
                }

                float playerBonus = entity.CurrentCharacter.GetBonus(result.Challenge.Type).Value;
                float challengeBonus = result.Challenge.ChallengeValue;
                float rarity = result.Challenge.RarityValue;

                float precentNormalized = challengeBonus / (playerBonus + rarity);

                TooltipTarget.Text += "\n <color=yellow>" + result.name + "</color> - <color=green>" + result.Challenge.Type.name
                    + "</color> <color=yellow> (" + Mathf.RoundToInt(100f * precentNormalized) + "%) </color>";
            }
        }

        TooltipTarget.Text += "</size>";
    }



    public override List<PlayerAction> GetPossiblePlayerActions()
    {
        return PossiblePlayerActions;
    }

    public void OnRightClick()
    {
        ShowActionMenu();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ShowActionMenu();
        }
    }
}
