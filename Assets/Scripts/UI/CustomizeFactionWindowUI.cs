using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizeFactionWindowUI : MonoBehaviour
{
    public static  CustomizeFactionWindowUI Instance;

    public TMP_InputField NameField;
    
    Faction CurrentFaction;

    public List<Sprite> FactionIcons = new List<Sprite>();

    public FactionPortraitUI FactionPortrait;

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void SetFaction(Faction faction)
    {
        CurrentFaction = faction;
        this.gameObject.SetActive(true);

        RefreshUI();
    }

    void RefreshUI()
    {
        if(CurrentFaction == null)
        {
            return;
        }

        NameField.text = CurrentFaction.name;

        FactionPortrait.SetInfo(CurrentFaction);

        FactionInfoUI.Instance.Show(CurrentFaction);
    }

    public void SetName()
    {
        CurrentFaction.name = NameField.text;
        RefreshUI();
    }

    public void SetFactionIcon()
    {
        PickCustomImageUI.Instance.Show((Sprite selectedSprite)=>
        {
            CurrentFaction.Icon = selectedSprite;
            RefreshUI();
        },FactionIcons,"Pick "+CurrentFaction.name+"'s Symbol:");
    }


}
