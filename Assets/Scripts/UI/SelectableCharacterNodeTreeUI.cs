using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SelectableCharacterNodeTreeUI : CharacterNodeTreeUI
{
    Action<Character> LocalOnSelect;

    public UnityEvent OnSelected;

    AgentAction RelevantAction = null;
    AgentInteractable RelevantTarget = null;

    public void Show()
    {
        ShowCharactersHirarchy(CORE.PC);
    }

    public void SetSelectableCharacters(Character topCharacter, Action<Character> onSelect = null, AgentAction agentAction = null, AgentInteractable relevantTarget = null)
    {
        RelevantAction = agentAction;
        RelevantTarget = relevantTarget;

        LocalOnSelect = onSelect;
        ShowCharactersHirarchy(topCharacter);
    }

    protected override IEnumerator SetCharacters(CharacterNodeTreeUIInstance node)
    {
        Transform nodeRoot = node.nodeObject.transform.GetChild(0).GetChild(0);
        nodeRoot.GetComponent<PortraitUI>().SetCharacter(node.CurrentCharacter);

        if (LocalOnSelect != null)
        {
            Button tempButton = nodeRoot.GetComponent<Button>();
            tempButton.onClick.RemoveAllListeners();

            FailReason potentialReason = null;

            if (node.CurrentCharacter == CORE.PC)
            {
                nodeRoot.GetComponent<CanvasGroup>().alpha = 0.5f;

                tempButton.onClick.AddListener(() =>
                {
                    GlobalMessagePrompterUI.Instance.Show(CORE.PC.name + " doesn't take part in tasks such as this.", 1f, Color.red);
                });
            }
            else if (!node.CurrentCharacter.IsAgent)
            {
                nodeRoot.GetComponent<CanvasGroup>().alpha = 0.5f;

                tempButton.onClick.AddListener(() =>
                {
                    GlobalMessagePrompterUI.Instance.Show("This character is not an agent.", 1f, Color.red);
                });
            }
            else if(RelevantAction != null && !RelevantAction.CanDoAction(node.CurrentCharacter.TopEmployer,node.CurrentCharacter, RelevantTarget, out potentialReason))
            {
                nodeRoot.GetComponent<CanvasGroup>().alpha = 0.5f;

                tempButton.onClick.AddListener(() =>
                {
                    GlobalMessagePrompterUI.Instance.Show(node.CurrentCharacter.name+" can not do this"+(potentialReason != null?(", "+potentialReason.Key) : ".") , 1f, Color.red);
                });
            }
            else
            {
                nodeRoot.GetComponent<CanvasGroup>().alpha = 1f;

                tempButton.onClick.AddListener(() =>
                {
                    LocalOnSelect.Invoke(node.CurrentCharacter);
                    OnSelected.Invoke();
                });
            }

            
        }

        yield return 0;

        for (int i = 0; i < node.Children.Count; i++)
        {
            yield return StartCoroutine(SetCharacters((CharacterNodeTreeUIInstance) node.Children[i]));
        }
    }
}

