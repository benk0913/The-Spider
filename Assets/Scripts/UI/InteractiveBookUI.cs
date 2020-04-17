using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractiveBookUI : MonoBehaviour
{
    public TextMeshProUGUI HowToPlayText;

    private void Start()
    {
        CORE.Instance.DelayedInvokation(1f, ()=>HowToPlayText.text = CORE.PC.CurrentFaction.HowToPlayDescription);
    }
}
