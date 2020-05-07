using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class KeyBindingPieceUI : MonoBehaviour {

    [SerializeField]
    TextMeshProUGUI m_txtTitle;

    [SerializeField]
    TextMeshProUGUI m_txtKey;

    [SerializeField]
    Image m_Image;

    [SerializeField]
    public Button m_btn;

    public KeyCode CurrentKey;

    public bool isWaitingForKey = false;
    Event keyEvent;
    Color initColor;

    public void SetInfo(string title, KeyCode key)
    {
        CurrentKey = key;
        m_txtTitle.text = title;
        m_txtKey.text = CurrentKey.ToString();
    }

    public void OnClick()
    {
        SetBinding();
    }

    protected void SetBinding()
    {
        StopAllCoroutines();
        isWaitingForKey = true;
        initColor = m_Image.color;
        m_Image.color = Color.green;
    }

    public void CloseBinding()
    {
        isWaitingForKey = false;
        m_Image.color = initColor;
    }

    void OnGUI()
    {
        if(isWaitingForKey)
        {
            keyEvent = Event.current;
            if (keyEvent != null && keyEvent.isKey)
            {
                if(keyEvent.keyCode != KeyCode.Escape)
                {
                    for(int i=0;i< InputMap.Map.Keys.Count;i++)
                    {
                        if(InputMap.Map[InputMap.Map.Keys.ElementAt(i)] == keyEvent.keyCode)
                        {
                            InputMap.Map[InputMap.Map.Keys.ElementAt(i)] = InputMap.Map[m_txtTitle.text];
                        }
                    }
                    InputMap.Map[m_txtTitle.text] = keyEvent.keyCode;
                    InputMap.SaveMap();
                    //SetInfo(m_txtTitle.text, keyEvent.keyCode);
                    KeyBindingWindowUI.Instance.Open();
                }

                CloseBinding();

            }
        }
    }
}
