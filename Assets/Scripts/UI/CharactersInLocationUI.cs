using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersInLocationUI : MonoBehaviour
{
    [SerializeField]
    Transform Container;

    [SerializeField]
    WorldPositionLerperUI Lerper;

    public List<Character> Characters = new List<Character>();


    private void Start()
    {
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

        this.gameObject.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    LocationEntity currentLocation;
    public void SetInfo(LocationEntity location)
    {
        currentLocation = location;

        Lerper.SetTransform(location.transform);

        Refresh();
    }

    public void AddCharacter(Character character)
    {
        if (!character.IsKnown("CurrentLocation", CORE.PC) 
            || character.IsDead 
            || !character.IsAgent)
        { 
            return;
        }

        if(Characters.Contains(character))
        {
            return;
        }
        Characters.Add(character);

        PortraitUI portrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI").GetComponent<PortraitUI>();
        portrait.transform.SetParent(Container, false);
        portrait.transform.localScale = Vector3.one;
        portrait.SetCharacter(character);
    }

    public void RemoveCharacter(Character character)
    {
        if(Characters.Contains(character))
        {
            Characters.Remove(character);
        }

        for(int i=0;i<Container.childCount;i++)
        {
            if(Container.GetChild(i).GetComponent<PortraitUI>().CurrentCharacter == character)
            {
                Container.GetChild(i).gameObject.SetActive(false);
                Container.GetChild(i).SetParent(transform);
                return;
            }
        }
    }

    void Refresh()
    {
        ClearContainer();
        Characters.Clear();

        foreach (Character character in currentLocation.CharactersInLocation)
        {
            AddCharacter(character);
        }
    }

    void ClearContainer()
    {
        while(Container.childCount > 0)
        {
            Container.GetChild(0).gameObject.SetActive(false);
            Container.GetChild(0).SetParent(transform);
        }
    }
}
