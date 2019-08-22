using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using SimpleJSON;
using System.IO;

public class CORE : MonoBehaviour
{
    public static CORE Instance;

    [SerializeField]
    public GameDB Database;

    [SerializeField]
    public Canvas MainCanvas;

    public List<Character> Characters = new List<Character>();

    public List<LocationEntity> Locations = new List<LocationEntity>();

    public List<LongTermTaskEntity> LongTermTasks = new List<LongTermTaskEntity>();

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

    public Character GetCharacterByID(string ID)
    {
        for(int i=0;i<Characters.Count;i++)
        {
            if(ID == Characters[i].ID)
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

    public LocationEntity GetClosestLocationWithTrait(PropertyTrait trait, LocationEntity targetLocation)
    {
        List<LocationEntity> LocationsWithTrait = new List<LocationEntity>();
        for (int i = 0; i < Locations.Count; i++)
        {
            if (Locations[i].CurrentProperty.Traits.Contains(trait))
            {
                LocationsWithTrait.Add(Locations[i]);
            }
        }

        LocationEntity foundLocation = null;
        if (LocationsWithTrait.Count > 0)
        {
            float minDistance = Mathf.Infinity;
            
            foreach(LocationEntity location in LocationsWithTrait)
            {
                float dist = Vector3.Distance(location.transform.position, targetLocation.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    foundLocation = location;
                }
            }
        }

        return foundLocation;
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

    public LocationEntity GetLocationByID(string ID)
    {
        for (int i = 0; i < Locations.Count; i++)
        {
            if (ID == Locations[i].ID)
            {
                return Locations[i];
            }
        }

        return null;
    }

    #endregion

    #region Long Term Tasks

    public LongTermTaskEntity GetLongTermTaskByID(string ID)
    {
        for (int i = 0; i < LongTermTasks.Count; i++)
        {
            if (ID == LongTermTasks[i].ID)
            {
                return LongTermTasks[i];
            }
        }

        return null;
    }

    #endregion



    #region Saving & Loading

    List<SaveFile> SaveFiles = new List<SaveFile>();

    public void SaveGame()
    {
        ReadAllSaveFiles();

        JSONClass savefile = new JSONClass();
        savefile["Name"] = "Save" + SaveFiles.Count;

        //savefile["CurrentDialog"] = SM.Dialog.CurrentDialog.Key;

    
        string ePath = Application.dataPath + "/Saves/" + savefile["Name"] + ".json";
        JSONNode tempNode = (JSONNode)savefile;
        File.WriteAllText(ePath, tempNode.ToString());

        ReadAllSaveFiles();
    }

    public void LoadGame(SaveFile file)
    {
        //SM.Game.NewGame();

        //SM.Game.CurrentDayPhase = (DayPhase)file.Content["DayPhase"].AsInt;
    }

    public void ReadAllSaveFiles()
    {
        SaveFiles.Clear();

        if (!Directory.Exists(Application.dataPath + "/Saves"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Saves");
        }
        string[] tempSaveFiles = Directory.GetFiles(Application.dataPath + "/Saves");

        if (tempSaveFiles.Length > 0)
        {
            for (int i = 0; i < tempSaveFiles.Length; i++)
            {
                if (tempSaveFiles[i].Contains("Save") && !tempSaveFiles[i].Contains(".meta"))
                {
                    string content = File.ReadAllText(tempSaveFiles[i]);
                    SaveFiles.Add(new SaveFile(content));
                }
            }
        }
    }

    #endregion
}

public class SaveFile
{
    public SaveFile(string content)
    {
        this.Content = JSON.Parse(content);
        this.Name = this.Content["Name"].Value;
    }

    public string Name;
    public JSONNode Content;
}