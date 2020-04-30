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


    [SerializeField]
    GameObject HideButton;

    public bool CantHide = false;

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
        if (!CantHide && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            Hide();
        }
    }

    public void Show(string message, Action acceptCallback, bool cantHide = false)
    {
        CantHide = cantHide;

        HideButton.gameObject.SetActive(!CantHide);

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
        AcceptAction?.Invoke();
        Hide();
    }
}
