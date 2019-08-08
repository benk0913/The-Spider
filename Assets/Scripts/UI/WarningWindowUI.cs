using System;
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

    public void Show(string message, Action acceptCallback)
    {
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
