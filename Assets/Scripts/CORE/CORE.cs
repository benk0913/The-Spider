using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CORE : MonoBehaviour
{
    public static CORE Instance;

    [SerializeField]
    public GameDB Database;

    [SerializeField]
    public Canvas MainCanvas;

    public List<Character> Characters = new List<Character>();

    public static Character PC;

    private void Awake()
    {
        Instance = this;
        Initialize();
    }

    void Initialize()
    {
        PC = Instantiate(Database.PlayerCharacter);
        Characters.Add(PC);

        Characters.AddRange(Database.PresetCharacters);
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

    public Character GenerateCharacter(int isFemale = -1, int minAge = 0, int maxAge = 150)
    {
        Character character = Instantiate(Database.HumanReference);

        character.Randomize();

        if(isFemale >= 0)
        {
            character.Gender = (GenderType)isFemale;
        }

        character.Age = Random.Range(minAge, maxAge);

        Characters.Add(character);

        return character;
    }
}
