using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePromptEntity : MonoBehaviour
{

    public string Message;

    public float Length;

    public Color MessageColor;
    
    public void ShowMessage()
    {
        GlobalMessagePrompterUI.Instance.Show(Message, Length, MessageColor);
    }

    public void ShowMessage(string message)
    {
        ShowMessage(message,1f,Color.yellow);
    }

    public void ShowMessage(string message, float time, Color clr)
    {
        this.Message = message;
        this.MessageColor = clr;
        this.Length = time;
        ShowMessage();
    }
}
