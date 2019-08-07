using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class CORE : MonoBehaviour
{
    public static CORE Instance;

    [SerializeField]
    public GameDB Database;

    [SerializeField]
    public Canvas MainCanvas;

    public List<Character> Characters = new List<Character>();

    public List<LocationEntity> Locations = new List<LocationEntity>();

    public static Character PC;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        PC = Instantiate(Database.PlayerCharacter);
        PC.name = Database.PlayerCharacter.name;
        Characters.Add(PC);
        PC.Initialize();

        foreach(Character character in Database.PresetCharacters)
        {
            Character tempCharacter = Instantiate(character);
            tempCharacter.Initialize();
            tempCharacter.name = character.name;

            Characters.Add(tempCharacter);
        }
    }

    #region Events

    public Dictionary<string, UnityEvent> DynamicEvents = new Dictionary<string, UnityEvent>();

    public void SubscribeToEvent(string eventKey, UnityAction action)
    {
        if(!DynamicEvents.ContainsKey(eventKey))
        {
            DynamicEvents.Add(eventKey,new UnityEvent());
        }

        DynamicEvents[eventKey].AddListener(action);
    }

    public void UnsubscribeFromEvent(string eventKey, UnityAction action)
    {
        if (!DynamicEvents.ContainsKey(eventKey))
        {
            Debug.LogError("EVENT " + eventKey + " does not exist!");
            return;
        }

        DynamicEvents[eventKey].RemoveListener(action);
    }

    public void InvokeEvent(string eventKey)
    {
        if (!DynamicEvents.ContainsKey(eventKey))
        {
            DynamicEvents.Add(eventKey, new UnityEvent());
        }

        DynamicEvents[eventKey].Invoke();
    }

    #endregion

    #region Misc

    public void ShowHoverMessage(string content, Sprite icon, Transform targetTransform)
    {
        HoverPanelUI hoverPanel = ResourcesLoader.Instance.GetRecycledObject("HoverPanelUI").GetComponent<HoverPanelUI>();
        hoverPanel.transform.SetParent(CORE.Instance.MainCanvas.transform);
        hoverPanel.transform.SetAsFirstSibling();
        hoverPanel.Show(targetTransform, content, icon);
    }

    #endregion

    #region Characters

    public Character GenerateCharacter(int isFemale = -1, int minAge = 0, int maxAge = 150)
    {
        Character character = Instantiate(Database.HumanReference);

        character.Initialize();

        character.Randomize();

        if(isFemale >= 0)
        {
            character.Gender = (GenderType)isFemale;
        }

        character.Age = Random.Range(minAge, maxAge);


        Characters.Add(character);

        return character;
    }

    public Character GetCharacter(string sName)
    {
        for(int i=0;i<Characters.Count;i++)
        {
            if(Characters[i].name == sName)
            {
                return Characters[i];
            }
        }

        return null;
    }

    #endregion

    #region Locations

    public LocationEntity GetRandomLocationWithTrait(PropertyTrait trait)
    {
        Locations = Locations.OrderBy(x => Random.value).ToList();

        for(int i=0;i<Locations.Count;i++)
        {
            if(Locations[i].CurrentProperty.Traits.Contains(trait))
            {
                return Locations[i];
            }
        }

        return null;
    }

    public LocationEntity GetLocationOfProperty(Property property)
    {
        foreach(LocationEntity location in Locations)
        {
            if(location.CurrentProperty == property)
            {
                return location;
            }
        }

        return null;
    }

    #endregion
}
