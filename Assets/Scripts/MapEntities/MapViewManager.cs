using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapViewManager : MonoBehaviour
{
    public static MapViewManager Instance;

    [SerializeField]
    public Transform MapElementsContainer;

    [SerializeField]
    public Camera Cam;

    [SerializeField]
    public TopdownCameraMovement TopDownCamera;

    [SerializeField]
    InteractableEntity MapInteractionEntity;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        CORE.Instance.SubscribeToEvent("AttemptBuild", AttemptBuild);
        CORE.Instance.SubscribeToEvent("HideBuild", HideBuild);
    }

    void AttemptBuild()
    {
    }

    void HideBuild()
    {
    }

    public void ShowMap()
    {
        CORE.Instance.InvokeEvent("ShowMap");
    }

    public void HideMap()
    {
        SelectedPanelUI.Instance.Deselect();
        RightClickDropDownPanelUI.Instance.Hide();
        CORE.Instance.InvokeEvent("HideMap");
    }

    public void Deselect()
    {
        SelectedPanelUI.Instance.Deselect();
        RightClickDropDownPanelUI.Instance.Hide();
    }

    public void FocusOnEntity(Transform entityTransform)
    {
        TopDownCamera.ViewTarget(entityTransform);
    }

    public void ForceInteractWithMap()
    {
        if (MapViewManager.Instance.MapElementsContainer.gameObject.activeInHierarchy)
        {
            return;
        }

        MapInteractionEntity.Interact();
    }
}
