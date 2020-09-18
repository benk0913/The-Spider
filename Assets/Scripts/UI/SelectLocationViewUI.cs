using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectLocationViewUI : MonoBehaviour
{
    public static SelectLocationViewUI Instance;

    [SerializeField]
    protected Transform Container;

    [SerializeField]
    public string PortraitPrefab = "SelectableLocationPortraitUI";

    [SerializeField]
    public TextMeshProUGUI TitleText;

    Action<LocationEntity> CurrentonSelect;

    Action CurrentOnCancel;

    Predicate<LocationEntity> CurrentFilter;


    string CurrentTitle;

    public string CurrentSortKey;


    public GameObject ZoomableContainer;

    public float MinScale = 0.5f;
    public float MaxScale = 1f;



    protected virtual void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (MouseLook.Instance == null) return;

        MouseLook.Instance.CurrentWindow = null;
    }

    public void Cancel()
    {
        CurrentOnCancel?.Invoke();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }


        if (Input.GetAxis("Mouse ScrollWheel") > 0f && ZoomableContainer.transform.localScale.x < MaxScale)
        {
            ZoomableContainer.transform.localScale += Vector3.one * 2f * Time.deltaTime;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && ZoomableContainer.transform.localScale.x > MinScale)
        {
            ZoomableContainer.transform.localScale -= Vector3.one * 2f * Time.deltaTime;
        }
    }

    public virtual void Show(Action<LocationEntity> onSelect = null, Predicate<LocationEntity> filter = null, string title = "Select Location:", LocationEntity topLocation = null, Action OnCancel = null)
    {
        MouseLook.Instance.CurrentWindow = this.gameObject;

        this.gameObject.SetActive(true);

        CurrentTitle = title;
        CurrentonSelect = onSelect;
        CurrentOnCancel = OnCancel;
        CurrentFilter = filter;

        Refresh();
    }

    void Refresh()
    {
        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        if (PopulateGridRoutine != null)
        {
            return;
        }

        PopulateGridRoutine = StartCoroutine(PopulateGrid(CurrentonSelect, CurrentFilter));
        TitleText.text = CurrentTitle;
    }

    protected Coroutine PopulateGridRoutine;
    protected virtual IEnumerator PopulateGrid(Action<LocationEntity> onSelect = null, Predicate<LocationEntity> filter = null)
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }

        yield return 0;

        List<LocationEntity> locations = CORE.Instance.Locations.FindAll(filter != null? filter : CommonFilter);

        locations = SortCharacters(CurrentSortKey, locations);

        yield return 0;

        foreach (LocationEntity location in locations)
        {
            GameObject selectableLocation = ResourcesLoader.Instance.GetRecycledObject(PortraitPrefab);

            selectableLocation.transform.SetParent(Container, false);
            selectableLocation.transform.localScale = Vector3.one;

            if (onSelect != null)
            {
                Button tempButton = selectableLocation.GetComponent<Button>();
                tempButton.onClick.RemoveAllListeners();
                tempButton.onClick.AddListener(() =>
                {
                    onSelect.Invoke(location);
                    this.gameObject.SetActive(false);
                });
            }

            selectableLocation.transform.GetComponentInChildren<LocationPortraitUI>().SetLocation(location);
            yield return 0;
        }

        PopulateGridRoutine = null;
    }

    public virtual bool CommonFilter(LocationEntity character)
    {
        return true;
    }

    public void SetSortingKey(string sortingKey)
    {
        CurrentSortKey = sortingKey;
        Refresh();
    }

    public List<LocationEntity> SortCharacters(string byKey, List<LocationEntity> locations)
    {

        switch(byKey)
        {
            case "Rank":
                {
                    return locations.OrderBy(x => x.Level).ToList();
                }
        }

        //Default Abc...
        return locations.OrderBy(x => x.name).ToList();
    }
}
