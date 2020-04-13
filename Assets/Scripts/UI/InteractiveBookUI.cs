using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractiveBookUI : MonoBehaviour
{
    public TextMeshProUGUI HowToPlayText;

    private void Start()
    {
        HowToPlayText.text = CORE.PC.CurrentFaction.HowToPlayDescription;
    }
}
