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

    public List<WarningWindowData> WindowQueue = new List<WarningWindowData>();


    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);

        if (WindowQueue.Count > 0)
        {
            Show(WindowQueue[0].Message, WindowQueue[0].AcceptCallback, WindowQueue[0].CantHide);
            WindowQueue.RemoveAt(0);
        }
    }

    private void Update()
    {
        if (!CantHide && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)))
        {
            Hide();
        }
        else if(Input.GetKeyDown(KeyCode.Return))
        {
            Accept();
        }
    }

    public void Show(string message, Action acceptCallback, bool cantHide = false)
    {
        if(this.gameObject.activeInHierarchy)
        {
            WindowQueue.Add(new WarningWindowData(message, acceptCallback, cantHide));
            return;
        }

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

    public class WarningWindowData
    {
        public string Message;
        public Action AcceptCallback;
        public bool CantHide = false;

        public WarningWindowData(string msg, Action callback, bool canHide = false)
        {
            this.Message = msg;
            this.AcceptCallback = callback;
            this.CantHide = canHide;
        }
    }
}
