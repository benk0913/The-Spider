﻿using System.Collections;
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

    public List<LocationEntity> PresetLocations = new List<LocationEntity>();

    public static Character PC;

    public bool LOAD_ON_START_tEST;

    public bool SAVE_FILE_TEST;

    private void Update()
    {
        if(SAVE_FILE_TEST)
        {
            SaveGame();
            SAVE_FILE_TEST = false;
        }
    }

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
        //TODO Remove when save/load complete.

        if(LOAD_ON_START_tEST)
        {
            ReadAllSaveFiles();
            foreach(SaveFile file in SaveFiles)
            {
                Debug.Log(file.Name + " - " + file.Content);
            }

            LoadGame(SaveFiles[SaveFiles.Count - 1]);
            return;
        }

        NewGame();
    }

    void NewGame()
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

        foreach(LocationEntity presetLocation in PresetLocations)
        {
            presetLocation.InitializePreset();
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

    public Character GenerateSimpleCharacter()
    {
        Character character = Instantiate(Database.HumanReference);

        character.Initialize();

        return character;
    }

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

    public LocationEntity GetPresetLocationByID(string locationID)
    {
        foreach(LocationEntity location in PresetLocations)
        {
            if(location.name == locationID)
            {
                return location;
            }
        }

        return null;
    }

    public LocationEntity GenerateNewLocation(Vector3 atPosition, Quaternion atRotation)
    {
        GameObject locationPrefab = ResourcesLoader.Instance.GetRecycledObject("Location");

        locationPrefab.transform.SetParent(MapViewManager.Instance.MapElementsContainer);
        locationPrefab.transform.position = atPosition;
        locationPrefab.transform.rotation = atRotation;

        return locationPrefab.GetComponent<LocationEntity>();
    }

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

    #region Saving & Loading

    List<SaveFile> SaveFiles = new List<SaveFile>();

    public void SaveGame()
    {
        ReadAllSaveFiles();

        JSONClass savefile = new JSONClass();
        savefile["Name"] = "Save" + SaveFiles.Count;

        for(int i=0;i<Characters.Count;i++)
        {
            savefile["Characters"][i] = Characters[i].ToJSON();
        }

        for (int i = 0; i < Locations.Count; i++)
        {
            savefile["Locations"][i] = Locations[i].ToJSON();
        }


        string ePath = Application.dataPath + "/Saves/" + savefile["Name"] + ".json";
        JSONNode tempNode = (JSONNode)savefile;
        File.WriteAllText(ePath, tempNode.ToString());

        ReadAllSaveFiles();
    }

    public void LoadGame(SaveFile file)
    {
        if(LoadGameRoutineInstance != null)
        {
            return;
        }

        LoadGameRoutineInstance = StartCoroutine(LoadGameRoutine(file));
    }

    Coroutine LoadGameRoutineInstance;
    IEnumerator LoadGameRoutine(SaveFile file)
    {
        while(ResourcesLoader.Instance.m_bLoading)
        {
            yield return 0;
        }

        for (int i = 0; i < file.Content["Characters"].Count; i++)
        {
            Character tempChar = GenerateSimpleCharacter();
            tempChar.FromJSON(file.Content["Characters"][i]);
            Characters.Add(tempChar);
        }

        for (int i = 0; i < file.Content["Locations"].Count; i++)
        {
            LocationEntity tempLocation = GetPresetLocationByID(file.Content["Locations"][i]["ID"]);

            if (tempLocation == null)
            {
                tempLocation = GenerateNewLocation(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            }

            tempLocation.FromJSON(file.Content["Locations"][i]);
            Locations.Add(tempLocation);
        }

        foreach (Character character in Characters)
        {
            character.ImplementIDs();
        }

        PC = GetCharacter("You");

        LoadGameRoutineInstance = null;
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