using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextInputWindowUI : MonoBehaviour
{
    public static TextInputWindowUI Instance;

    Action<string> AcceptAction;

    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    TMP_InputField InputField;

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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            Hide();
        }
    }

    public void Show(Action<string> acceptCallback, string title = "Type It Down", string defaultText = "Enter Text...")
    {
        if (MapViewManager.Instance != null && MouseLook.Instance != null && MouseLook.Instance.isAbleToLookaround)
        {
            MapViewManager.Instance.ForceInteractWithMap();
        }

        this.gameObject.SetActive(true);

        Title.text = title;
        InputField.text = defaultText;
        AcceptAction = acceptCallback;
    }

    public void Accept()
    {
        AcceptAction.Invoke(InputField.text);
        Hide();
    }
}
