using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationHoverUI : MonoBehaviour
{
    public static LocationHoverUI Instance;

    LocationEntity CurrentLocation;

    [SerializeField]
    WorldPositionLerperUI Lerper;

    [SerializeField]
    TextMeshProUGUI LabelText;

    [SerializeField]
    Image BubbleImage;

    [SerializeField]
    LocationPortraitUI Portrait;

    public bool IsUnknown;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(LocationEntity location, bool isUnknown = false)
    {
        this.IsUnknown = isUnknown;

        this.gameObject.SetActive(true);

        CurrentLocation = location;

        Lerper.SetTransform(location.transform);

        RefreshUI();
    }

    public void Hide()
    {
        Lerper.SetTransform(null);
        this.gameObject.SetActive(false);
    }

    public void RefreshUI()
    {
        if (IsUnknown)
        {
            Portrait.SetLocation(null);

            LabelText.text = "Unknown...";

            LabelText.color = Color.black;
            BubbleImage.color = CORE.Instance.Database.DefaultFaction.FactionColor;

            return;
        }

        Portrait.SetLocation(CurrentLocation);
        
        LabelText.text = CurrentLocation.Name;

        if (CurrentLocation.OwnerCharacter != null && CurrentLocation.OwnerCharacter.IsKnown("Faction", CORE.PC))
        {
            LabelText.color = Color.white;
            BubbleImage.color = CurrentLocation.OwnerCharacter.CurrentFaction.FactionColor;
        }
        else
        {
            LabelText.color = Color.black;
            BubbleImage.color = CORE.Instance.Database.DefaultFaction.FactionColor;
        }
    }
}
