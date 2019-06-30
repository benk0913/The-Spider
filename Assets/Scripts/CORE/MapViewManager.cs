﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapViewManager : MonoBehaviour
{
    public static MapViewManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        CORE.Instance.SubscribeToEvent("AttemptBuild", AttemptBuild);
        CORE.Instance.SubscribeToEvent("HideBuild", HideBuild);
    }

    void AttemptBuild()
    {
    }

    void HideBuild()
    {
    }

    public void ShowMap()
    {
        CORE.Instance.InvokeEvent("ShowMap");
    }

    public void HideMap()
    {
        SelectedPanelUI.Instance.Deselect();
        CORE.Instance.InvokeEvent("HideMap");
    }

    public void Deselect()
    {
        SelectedPanelUI.Instance.Deselect();
    }
}
