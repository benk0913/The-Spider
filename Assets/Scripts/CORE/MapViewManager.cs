using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapViewManager : MonoBehaviour
{
    public void ShowMap()
    {
        CORE.Instance.InvokeEvent("ShowMap");
    }

    public void HideMap()
    {
        SelectedPanelUI.Instance.Deselect();
        CORE.Instance.InvokeEvent("HideMap");
    }

    public void Deselect()
    {
        SelectedPanelUI.Instance.Deselect();
    }
}
