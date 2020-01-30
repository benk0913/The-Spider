using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnLoadingWindowUI : MonoBehaviour
{
    public static TurnLoadingWindowUI Instance;

    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    Image Filler;

    private void Awake()
    {
        Instance = this;
    }

    public void SetLoadingTitle(string title)
    {
        Title.text = title;
    }

    public void SetProgress(float progress)
    {
        Filler.fillAmount = progress;
    }
}
