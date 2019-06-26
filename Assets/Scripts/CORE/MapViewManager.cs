﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapViewManager : MonoBehaviour
{
    public static MapViewManager Instance;

    public LocationEntity SelectedLocation;

    internal void PurchasePlot(PurchasablePlotUI purchasablePlotUI)
    {
    }

    public Character SelectedCharacter;

    private void Awake()
    {
        Instance = this;
    }

    public void Show()
    {

    }

    public void Hide()
    {
        DeselectCurrentCharacter();
        DeselectCurrentLocation();
    }

    public void SelectLocation(LocationEntity location)
    {
        if(SelectedLocation != null)
        {
            DeselectCurrentLocation();
        }

        SelectedLocation = location;
        SelectedLocation.Select();
    }

    public void DeselectCurrentLocation()
    {
        if(SelectedLocation == null)
        {
            return;
        }

        SelectedLocation.Deselect();
    }

    public void SelectCharacter(Character character)
    {
        if (SelectedCharacter != null)
        {
            DeselectCurrentCharacter();
        }

        if (character.Employer == null)
        {
            return;
        }

        if (character.TopEmployer != CORE.Instance.Database.PlayerCharacter)
        {
            return;
        }

        SelectedCharacter = character;
        SelectedPanelUI.Instance.SetSelected(character);
    }

    public void DeselectCurrentCharacter()
    {
        SelectedCharacter = null;
        SelectedPanelUI.Instance.Deselect();
    }
}
