using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsOfLocationUI : MonoBehaviour
{

    [SerializeField]
    WorldPositionLerperUI Lerper;

    [SerializeField]
    TextMeshProUGUI GoldText;

    [SerializeField]
    TextMeshProUGUI RumorsText;

    [SerializeField]
    TextMeshProUGUI ConnectionsText;

    [SerializeField]
    TextMeshProUGUI ProgressionText;

    LocationEntity CurrentLocation;

    [SerializeField]
    Animator PopAnimer;

    private void Start()
    {
        if (!MapViewManager.Instance.MapElementsContainer.gameObject.activeInHierarchy)
        {
            Hide();
        }

        CORE.Instance.SubscribeToEvent("ShowMap", Show);

        CORE.Instance.SubscribeToEvent("HideMap", Hide);
    }

    private void OnDestroy()
    {
        CORE.Instance.UnsubscribeFromEvent("ShowMap", Show);
        CORE.Instance.UnsubscribeFromEvent("HideMap", Hide);
    }

    public void Show()
    {
        if(!MapViewManager.Instance.MapElementsContainer.gameObject.activeInHierarchy)
        {
            return;
        }

        if (CurrentLocation.OwnerCharacter == null || CurrentLocation.OwnerCharacter.TopEmployer != CORE.PC)
        {
            return;
        }

        this.gameObject.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void SetInfo(LocationEntity location)
    {
        CurrentLocation = location;

        Lerper.SetTransform(location.transform);

        Refresh();
    }

    float previousGold;
    float previousRumors;
    float previousConnections;
    float previousProgress;

    public void Refresh()
    {
        if(CurrentLocation == null)
        {
            return;
        }

        int currentGold = CurrentLocation.GoldGenerated;
        int currentRumors = CurrentLocation.RumorsGenerated;
        int currentConnections = CurrentLocation.ConnectionsGenerated;
        int currentProgression = CurrentLocation.ProgressionGenerated;

        if (currentGold != previousGold || currentRumors != previousRumors || currentConnections != previousConnections || currentProgression != previousProgress)
        {
            PopAnimer.SetTrigger("Pop");
        }

        previousGold = currentGold;
        previousRumors = currentRumors;
        previousConnections = currentConnections;
        previousProgress = currentProgression;



        GoldText.text = currentGold.ToString();
        RumorsText.text = currentRumors.ToString();
        ConnectionsText.text = currentConnections.ToString();
        ProgressionText.text = currentProgression.ToString();

        GoldText.transform.parent.gameObject.SetActive(CurrentLocation.CurrentAction.GoldGenerated > 0);
        RumorsText.transform.parent.gameObject.SetActive(CurrentLocation.CurrentAction.RumorsGenerated> 0);
        ConnectionsText.transform.parent.gameObject.SetActive(CurrentLocation.CurrentAction.ConnectionsGenerated> 0);
        ProgressionText.transform.parent.gameObject.SetActive(CurrentLocation.CurrentAction.ProgressGenerated> 0);
    }

}
