using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class PromotionCharacterNodeTreeUI : SelectableCharacterNodeTreeUI
{
    public override void SetSelectableCharacters(Character topCharacter, Action<Character> onSelect = null, AgentAction agentAction = null, AgentInteractable relevantTarget = null)
    {
        ShowCharactersHirarchy(topCharacter);
    }

    protected override IEnumerator SetCharacters(CharacterNodeTreeUIInstance node)
    {
        Transform nodeRoot = node.nodeObject.transform.GetChild(0).GetChild(0);
        nodeRoot.GetComponent<PortraitUI>().SetCharacter(node.CurrentCharacter);

        float percentage = (node.CurrentCharacter.CProgress / 50f);

        Transform nodeProgressPanel = node.nodeObject.transform.GetChild(0).GetChild(1);

        if (node.CurrentCharacter == CORE.PC)
        {
            nodeRoot.GetComponent<CanvasGroup>().alpha = 0.5f;
            nodeProgressPanel.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 0f;
            nodeProgressPanel.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "--";
        }
        else
        {
            nodeRoot.GetComponent<CanvasGroup>().alpha = 1f;
            nodeProgressPanel.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = percentage;
            nodeProgressPanel.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(percentage * 100f) + "%";
        }

        yield return 0;

        for (int i = 0; i < node.Children.Count; i++)
        {
            yield return StartCoroutine(SetCharacters((CharacterNodeTreeUIInstance) node.Children[i]));
        }
    }
}

