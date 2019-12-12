using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using SimpleJSON;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CORE : MonoBehaviour
{
    public static CORE Instance;

    public bool DEBUG;

    [SerializeField]
    public GameDB Database;

    [SerializeField]
    public Canvas MainCanvas;

    [SerializeField]
    public EventSystem UIEventSystem;

    public TechTreeItem TechTree;

    public List<Character> Characters = new List<Character>();
    
    public List<LocationEntity> Locations = new List<LocationEntity>();

    public List<LocationEntity> PresetLocations = new List<LocationEntity>();

    public static Character PC;
    public static Faction PlayerFaction;

    public bool TutorialOnStart = false; // TODO - Move this to a pref section when there is one (settings / etc...)

    public List<SplineLerperWorldUI> ActiveLerpers = new List<SplineLerperWorldUI>();

    public bool isLoading
    {
        get
        {
            return LoadingGameRoutine != null;
        }
    }

    public bool FocusViewLocked = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        foreach(Faction faction in Database.Factions)
        {
            if(faction.isLockedByDefault)
            {
                faction.isLocked = bool.Parse(PlayerPrefs.GetString(faction.name + "LOCK", true.ToString()));
            }
        }
    }

    public void NewGame(Faction selectedFaction)
    {
        if(LoadingGameRoutine != null)
        {
            return;
        }

        LoadingGameRoutine = StartCoroutine(NewGameRoutine(selectedFaction));
    }

    Coroutine LoadingGameRoutine;
    IEnumerator NewGameRoutine(Faction selectedFaction)
    {
        while (ResourcesLoader.Instance.m_bLoading)
        {
            yield return 0;
        }

        yield return StartCoroutine(LoadMainScene(selectedFaction));


        if (TutorialOnStart)
        {
            Quest questClone = Database.TutorialQuest.CreateClone();
            questClone.ForCharacter = CORE.PC;
            QuestsPanelUI.Instance.AddNewQuest(questClone);
        }

        foreach(Faction faction in Database.Factions)
        {
            if(faction.FactionHead != null)
            {
                Character factionHead = CORE.Instance.GetCharacter(faction.FactionHead.name);

                foreach (Quest quest in faction.Goals)
                {
                    Quest questClone = quest.CreateClone();
                    questClone.ForCharacter = factionHead;

                    QuestsPanelUI.Instance.AddNewQuest(questClone);
                }
            }
        }

        foreach (Faction faction in Database.Factions)
        {
            if (faction.FactionHead != null)
            {
                Character factionHead = CORE.Instance.GetCharacter(faction.FactionHead.name);

                if (faction.isAlwaysKnown)
                {
                    factionHead.Known.KnowAll("Name");
                    factionHead.Known.KnowAll("Faction");
                }
            }
        }

        LoadingGameRoutine = null;
    }

    IEnumerator LoadMainScene(Faction selectedFaction)
    {
        SceneManager.LoadScene(selectedFaction.RoomScene);
        while (SceneManager.GetActiveScene().name != selectedFaction.RoomScene)
        {
            yield return 0;
        }

        if (MapViewManager.Instance != null)
        {
            MapViewManager.Instance.ShowMap();
            MapViewManager.Instance.MapElementsContainer.gameObject.SetActive(true);
        }

        yield return 0;

        TechTree = Database.TechTreeRoot.Clone();

        PlayerFaction = selectedFaction;

        Character prewarmedPC = Instantiate(selectedFaction.FactionHead);
        prewarmedPC.name = selectedFaction.FactionHead.name;
        PC = prewarmedPC;
        prewarmedPC.Initialize(true);
        Characters.Add(prewarmedPC);

        foreach (Character character in Database.PresetCharacters)
        {
            if(character.name == PC.name)
            {
                continue;
            }

            Character tempCharacter = Instantiate(character);
            tempCharacter.name = character.name;
            tempCharacter.Initialize(true);

            Characters.Add(tempCharacter);
        }

        foreach (LocationEntity presetLocation in PresetLocations)
        {
            presetLocation.InitializePreset();
            CORE.Instance.Locations.Add(presetLocation);
        }

        if (MapViewManager.Instance != null)
        {
            MapViewManager.Instance.HideMap();
            MapViewManager.Instance.MapElementsContainer.gameObject.SetActive(false);
        }


        RoomsManager.Instance.AddCurrentRoom();

        AddListeners();
    }

    void AddListeners()
    {
        GameClock.Instance.OnTurnPassed.AddListener(TurnPassed);
    }

    void RemoveListeners()
    {
        GameClock.Instance.OnTurnPassed.RemoveListener(TurnPassed);
    }

    void WipeCurrentGame()
    {
        foreach(Character character in Characters)
        {
            character.Wipe();
        }

        Characters.Clear();
        Locations.Clear();
        PresetLocations.Clear();

        GameClock.Instance.Wipe();
        RemoveListeners();
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
        if(DEBUG)
        {
            Debug.Log("CORE - Event Invoked " + eventKey);
        }

        if (!DynamicEvents.ContainsKey(eventKey))
        {
            DynamicEvents.Add(eventKey, new UnityEvent());
        }

        DynamicEvents[eventKey].Invoke();
    }

    public void TurnPassed()
    {
        TurnPassedRoutineInstance = StartCoroutine(TurnPassedRoutine());
    }

    public Coroutine TurnPassedRoutineInstance;
    IEnumerator TurnPassedRoutine()
    {
        //AI DECISIONS
        foreach(Faction faction in Database.Factions)
        {
            if(faction.FactionHead == null)
            {
                continue;
            }

            Character factionHead = GetCharacter(faction.FactionHead.name);

            if(factionHead == null || factionHead.AI == null)
            {
                continue;
            }

            if(factionHead.name == CORE.PC.name)
            {
                continue;
            }

            if(factionHead.IsDead)
            {
                continue;
            }

            factionHead.AI.MakeDecision(factionHead);
        }

        yield return 0;

        foreach(LocationEntity location in Locations)
        {
            yield return StartCoroutine(location.TurnPassed());
        }

        foreach (Character character in Characters)
        {
            character.OnTurnPassedAI();
            yield return 0;
        }

        if(GameClock.Instance.CurrentTimeOfDay == GameClock.GameTime.Morning)
        {
            foreach(Faction faction in Database.Factions)
            {
                if(faction.FactionHead == null)
                {
                    continue;
                }

                Character factionHead = GetCharacter(faction.FactionHead.name);

                if(factionHead == null)
                {
                    continue;
                }

                factionHead.Gold += faction.GoldGeneratedPerDay;
                factionHead.Connections += faction.ConnectionsGeneratedPerDay;
                factionHead.Rumors += faction.RumorsGeneratedPerDay;
                factionHead.Progress += faction.ProgressGeneratedPerDay;
                factionHead.Reputation += faction.ReputationGeneratedPerDay;
            }
        }


        TurnPassedRoutineInstance = null;
    }

    #endregion

    #region Misc

    public void ShowPortraitEffect(GameObject effect, Character character, LocationEntity targetLocation)
    {
        GameObject effectObj = ResourcesLoader.Instance.GetRecycledObject(effect);
        effectObj.transform.SetParent(CORE.Instance.MainCanvas.transform);
        effectObj.transform.localScale = Vector3.one;
        effectObj.GetComponent<PortraitUI>().SetCharacter(character);
        effectObj.GetComponent<WorldPositionLerperUI>().SetTransform(targetLocation.transform);
    }

    public void SplineAnimationObject(string prefabKey,Transform startPoint,Transform targetPoint,System.Action OnComplete = null, bool canvasElement = true)
    {
        GameObject prefabObj = ResourcesLoader.Instance.GetRecycledObject(prefabKey);

        if (canvasElement)
        {
            prefabObj.transform.SetParent(MainCanvas.transform);
            prefabObj.transform.localScale = Vector3.one;
        }

        SplineLerperWorldUI lerper = prefabObj.GetComponent<SplineLerperWorldUI>();

        ActiveLerpers.Add(lerper);
        OnComplete += (() => { ActiveLerpers.Remove(lerper); });

        lerper.SetInfo(startPoint ,targetPoint, OnComplete);
    }

    public void ShowHoverMessage(string content, Sprite icon, Transform targetTransform)
    {
        HoverPanelUI hoverPanel = ResourcesLoader.Instance.GetRecycledObject("HoverPanelUI").GetComponent<HoverPanelUI>();
        hoverPanel.transform.SetParent(CORE.Instance.MainCanvas.transform);
        hoverPanel.transform.SetAsFirstSibling();
        hoverPanel.Show(targetTransform, content, icon);
    }

    public void GenerateLongTermTask(LongTermTask task, Character requester, Character character, LocationEntity target, Character targetCharacter = null, int turnsLeft = -1, AgentAction actionPerTurn = null)
    {
        LongTermTaskEntity longTermTask = Instantiate(ResourcesLoader.Instance.GetObject("LongTermTaskEntity")).GetComponent<LongTermTaskEntity>();

        longTermTask.transform.SetParent(MapViewManager.Instance.MapElementsContainer);
        longTermTask.transform.position = target.transform.position;
        longTermTask.SetInfo(task, requester, character, target, targetCharacter, turnsLeft, actionPerTurn);
    }

    public void DelayedInvokation(float time, System.Action action)
    {
        StartCoroutine(DelayedInvokationRoutine(time, action));
    }

    IEnumerator DelayedInvokationRoutine(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);

        action.Invoke();
    }

    #endregion

    #region Characters

    public Character GenerateSimpleCharacter()
    {
        Character character = Instantiate(Database.HumanReference);

        character.Initialize();

        return character;
    }

    public Character GenerateCharacter(int isFemale = -1, int minAge = 0, int maxAge = 150, LocationEntity startLocation = null)
    {
        Character character = GenerateSimpleCharacter();

        character.Randomize();

        if(isFemale >= 0)
        {
            character.Gender = (GenderType)isFemale;
        }

        character.Age = Random.Range(minAge, maxAge);

        if (startLocation != null)
        {
            character.GoToLocation(startLocation);
        }
        else
        {
            character.GoToLocation(GetRandomLocationWithTrait(Database.PublicAreaTrait));            
        }

        character.StartLivingIn(character.CurrentLocation);

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
        GameObject locationPrefab = Instantiate(ResourcesLoader.Instance.GetObject("Location"));

        locationPrefab.transform.SetParent(MapViewManager.Instance.MapElementsContainer);
        locationPrefab.transform.position = atPosition;
        locationPrefab.transform.rotation = atRotation;

        return locationPrefab.GetComponent<LocationEntity>();
    }
    
    public LocationEntity GetRandomLocation()
    {
        return Locations[Random.Range(0, Locations.Count)];
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
        if(targetLocation == null)
        {
            return GetRandomLocationWithTrait(trait);
        }

        List<LocationEntity> LocationsWithTrait = CORE.Instance.Locations.FindAll((LocationEntity location) =>
        {
            return location.Traits.Contains(trait);
        });

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

    public void RestartGame()
    {
        Destroy(this.gameObject);
        SceneManager.LoadScene(0);
    }

    public List<SaveFile> SaveFiles = new List<SaveFile>();

    public void SaveGame()
    {
        ReadAllSaveFiles();

        JSONClass savefile = new JSONClass();
        savefile["Name"] = "Save" + SaveFiles.Count;
        savefile["Date"] = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        savefile["SelectedFaction"] = PlayerFaction.name;
        savefile["PlayerCharacter"] = PC.name;

        savefile["GameClock"] = GameClock.Instance.ToJSON();

        for (int i=0;i<Characters.Count;i++)
        {
            savefile["Characters"][i] = Characters[i].ToJSON();
        }

        for (int i = 0; i < Locations.Count; i++)
        {
            savefile["Locations"][i] = Locations[i].ToJSON();
        }

        savefile["Rumors"] = RumorsPanelUI.Instance.ToJSON();
        savefile["Quests"] = QuestsPanelUI.Instance.ToJSON();
        savefile["Rooms"]  = RoomsManager.Instance.ToJSON();
        savefile["TechTree"] = TechTree.ToJSON();

        string ePath = Application.dataPath + "/Saves/" + savefile["Name"] + ".json";
        JSONNode tempNode = (JSONNode)savefile;
        File.WriteAllText(ePath, tempNode.ToString());

        ReadAllSaveFiles();
    }

    public void LoadGame(SaveFile file = null)
    {
        if(LoadingGameRoutine != null)
        {
            return;
        }

        LoadingGameRoutine = StartCoroutine(LoadGameRoutine(file));
    }


    IEnumerator LoadGameRoutine(SaveFile file = null)
    {
        while (ResourcesLoader.Instance.m_bLoading)
        {
            yield return 0;
        }


        WipeCurrentGame();

        SceneManager.LoadScene("POSTLOADER");
        while (SceneManager.GetActiveScene().name != "POSTLOADER")
        {
            yield return 0;
        }

        yield return StartCoroutine(LoadMainScene(Database.GetFactionByName(file.Content["SelectedFaction"])));

        MapViewManager.Instance.ShowMap();
        MapViewManager.Instance.MapElementsContainer.gameObject.SetActive(true);

        if (file != null)
        {

            GameClock.Instance.FromJSON(file.Content["GameClock"]);

            for (int i = 0; i < file.Content["Characters"].Count; i++)
            {
                Character tempCharacter = GetCharacterByID(file.Content["Characters"][i]["ID"]);

                if (tempCharacter == null)
                {
                    tempCharacter = GenerateSimpleCharacter();
                    Characters.Add(tempCharacter);
                }

                tempCharacter.FromJSON(file.Content["Characters"][i]);

                yield return 0;
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

                yield return 0;
            }

            RumorsPanelUI.Instance.FromJSON(file.Content["Rumors"]);
            QuestsPanelUI.Instance.FromJSON(file.Content["Quests"]);
            RoomsManager.Instance.FromJSON(file.Content["Rooms"]);
            TechTree.FromJSON(file.Content["TechTree"]);
        }
        yield return 0;

        PC = GetCharacter(file.Content["PlayerCharacter"]);

        foreach (Character character in Characters)
        {
            character.ImplementIDs();

            yield return 0;
        }

        foreach(LocationEntity location in Locations)
        {
            location.ImplementIDs();

            yield return 0;
        }

        MapViewManager.Instance.HideMap();
        MapViewManager.Instance.MapElementsContainer.gameObject.SetActive(false);

        LoadingGameRoutine = null;
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
                    SaveFiles.Add(new SaveFile(content, tempSaveFiles[i]));
                }
            }
        }
    }

    public void RemoveSave(SaveFile currentSave)
    {
        File.Delete(currentSave.Path);
        ReadAllSaveFiles();
    }

    #endregion

    #region Character Utils
    
    #endregion
}

public class SaveFile
{
    public SaveFile(string content, string path)
    {
        this.Content = JSON.Parse(content);

        this.Name = this.Content["Name"].Value;
        this.Date = this.Content["Date"].Value;

        this.Path = path;
    }

    public string Name;
    public string Date;
    public string Path;
    public JSONNode Content;
}