using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllCharactersWindowUI : SelectCharacterViewUI
{
    public static AllCharactersWindowUI Instance;
    public GridType CurrentType;


    protected override void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void ChangeType(int toType)
    {
        CurrentType = (GridType)toType;
        Refresh();
    }

    void Refresh()
    {
        switch (CurrentType)
        {
            case GridType.All:
                {
                    base.Show(null, (Character charInQuestion) =>
                    {
                        return charInQuestion.Known.GetIsAnythingKnown(CORE.PC) && charInQuestion != CORE.PC;
                    });
                    break;
                }
            case GridType.Agents:
                {
                    base.Show(null, (Character charInQuestion) =>
                    {
                        return charInQuestion.Known.GetIsAnythingKnown(CORE.PC)
                        && charInQuestion.TopEmployer == CORE.PC
                        && charInQuestion != CORE.PC;
                    });
                    break;
                }
            case GridType.Important:
                {
                    base.Show(null, (Character charInQuestion) =>
                    {
                        return charInQuestion.Known.GetIsAnythingKnown(CORE.PC)
                        && charInQuestion.isImportant
                        && charInQuestion != CORE.PC;
                    });
                    break;
                }
        }
    }

    public void Show()
    {
        Show(null, null);
    }

    public override void Show(Action<Character> onSelect = null, Predicate<Character> filter = null, string title = "Characters View", Character topCharacter = null)
    {
        this.gameObject.SetActive(true);
        CORE.Instance.SubscribeToEvent("PassTimeComplete", Refresh);

        Refresh();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        CORE.Instance.UnsubscribeFromEvent("PassTimeComplete", Refresh);
    }

    [System.Serializable]
    public enum GridType
    {
        All,Agents,Important
    }
}
