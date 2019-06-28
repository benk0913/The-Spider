using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapViewManager : MonoBehaviour
{
    public void ShowMap()
    {

    }

    public void HideMap()
    {
        SelectedPanelUI.Instance.Deselect();
    }

    public void Deselect()
    {
        SelectedPanelUI.Instance.Deselect();
    }
}
