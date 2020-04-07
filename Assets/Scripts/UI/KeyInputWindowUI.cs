using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyInputWindowUI : MonoBehaviour
{
    public static KeyInputWindowUI Instance;

    [SerializeField]
    TextMeshProUGUI DefaultInput;

    [SerializeField]
    TMP_InputField InputField;

    public Action OnCorrect;
    public Action OnWrong;

    public string ExpectedResult;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Submit();
        }
    }

    public void Show(string defaultMessage, string expectedResult, Action onCorrect = null, Action onIncorrect = null, int charLimit = 999, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard)
    {
        //if (MapViewManager.Instance != null && MouseLook.Instance != null && MouseLook.Instance.isAbleToLookaround)
        //{
        //    MapViewManager.Instance.ForceInteractWithMap();
        //}

        this.gameObject.SetActive(true);

        InputField.text = "";
        DefaultInput.text = defaultMessage;
        OnCorrect = onCorrect;
        OnWrong = onIncorrect;
        InputField.characterLimit = charLimit;
        InputField.contentType = contentType;
        ExpectedResult = expectedResult;
        InputField.Select();
    }

    public void Submit()
    {
        if(InputField.text == ExpectedResult)
        {
            OnCorrect?.Invoke();
        }
        else
        {
            OnWrong?.Invoke();
        }
        
        Hide();
    }
}
