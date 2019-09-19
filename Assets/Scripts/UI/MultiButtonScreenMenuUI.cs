using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiButtonScreenMenuUI : MonoBehaviour
{
    public List<MultiButtonScreenInstance> ButtonScreens = new List<MultiButtonScreenInstance>();

    private void OnEnable()
    {
        for(int i=0;i<ButtonScreens.Count;i++)
        {
            AddButtonListener(ButtonScreens[i].SelectionButton, ButtonScreens[i]);
        }

        Invoke("Init", 0.1f);
    }

    private void Init()
    {
        Select(ButtonScreens[0]);
    }

    public void OnDisable()
    {
        foreach (MultiButtonScreenInstance inst in ButtonScreens)
        {
            inst.Screen.gameObject.SetActive(true);
        }
    }

    void AddButtonListener(Button button, MultiButtonScreenInstance instance)
    {
        button.onClick.AddListener(() => { Select(instance); });
    }

    void Select(MultiButtonScreenInstance instance)
    {
        foreach(MultiButtonScreenInstance inst in ButtonScreens)
        {
            if(inst != instance)
            {
                inst.Screen.gameObject.SetActive(false);
                inst.SelectionButton.interactable = true;
            }
        }

        instance.Screen.gameObject.SetActive(true);
        instance.SelectionButton.interactable = false;
    }
}

[System.Serializable]
public class MultiButtonScreenInstance
{
    public Button SelectionButton;
    public GameObject Screen;
}
