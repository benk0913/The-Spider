using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LockedBoxEntity : MonoBehaviour
{
    public string ExpectedResult = "";
    public DialogDecisionAction OnUnlock = null;

    [SerializeField]
    public TMP_InputField.ContentType ContentType;

    public int CharLimit = 4;

    public string Message;

    public string OnWrongMessage;

    public void Interact()
    {
        KeyInputWindowUI.Instance.Show(Message,
            ExpectedResult,
            () =>
            {
                OnUnlock?.Activate();
            },
            () =>
            {
                GlobalMessagePrompterUI.Instance.Show(OnWrongMessage, 1f, Color.red);
            },
            CharLimit,
            ContentType);
    }
}
