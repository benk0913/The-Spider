using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CORE : MonoBehaviour
{
    public static CORE Instance;

    [SerializeField]
    public GameDB Database;

    public List<Character> Characters = new List<Character>();

    public static Character PC
    {
        get
        {
            return Instance.Database.PlayerCharacter;
        }
    }

    private void Awake()
    {
        Instance = this;
        Initialize();
    }

    void Initialize()
    {
        Characters.Add(PC);
    }

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

    public Character GenerateCharacter(int isFemale = -1, bool isChild = false)
    {
        Character character = Instantiate(Database.HumanReference);

        character.Randomize();

        if(isFemale >= 0)
        {
            character.Gender = (GenderType)isFemale;
        }

        if (isChild)
        {
            character.Age = Random.Range(0,15);
        }

        Characters.Add(character);

        return character;
    }
}
