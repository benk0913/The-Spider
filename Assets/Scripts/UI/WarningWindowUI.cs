﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarningWindowUI : MonoBehaviour
{
    public static WarningWindowUI Instance;

    Action AcceptAction;

    [SerializeField]
    TextMeshProUGUI Description;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    public void Show(string message, Action acceptCallback)
    {
        if (MapViewManager.Instance != null && MouseLook.Instance != null && MouseLook.Instance.isAbleToLookaround)
        {
            MapViewManager.Instance.ForceInteractWithMap();
        }

        this.gameObject.SetActive(true);

        Description.text = message;
        AcceptAction = acceptCallback;
    }

    public void Accept()
    {
        AcceptAction.Invoke();
        Hide();
    }
}
