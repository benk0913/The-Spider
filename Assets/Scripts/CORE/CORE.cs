using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using SimpleJSON;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Steamworks;
using UnityEngine.UI;

public class CORE : MonoBehaviour
{
    public static CORE Instance;

    public string FEEDBACK_URL;

    public bool DEBUG;

    [SerializeField]
    public GameDB Database;

    [SerializeField]
    public Canvas MainCanvas;

    [SerializeField]
    public Transform DisposableContainer;

    [SerializeField]
    public EventSystem UIEventSystem;

    [SerializeField]
    public GameObject LoadingPanel;

    [SerializeField]
    AudioListener ListenerOfSound;

    [SerializeField]
    public Image ScreenFader;

    [SerializeField]
    GameObject SavingGamePanel;

    public TechTreeItem TechTree;

    public List<Faction> Factions = new List<Faction>();

    public List<Character> Characters = new List<Character>();
    
    public List<LocationEntity> Locations = new List<LocationEntity>();

    public List<LocationEntity> PresetLocations = new List<LocationEntity>();

    public List<RecruitmentPool> RecruitmentPools = new List<RecruitmentPool>();


    public List<DayRumor> DayRumors = new List<DayRumor>();

    public static Character PC;
    public static Faction PlayerFaction;

    public SessionRulesManager SessionRules;

    public bool TutorialOnStart = false; // TODO - Move this to a pref section when there is one (settings / etc...)

    public List<SplineLerperWorldUI> ActiveLerpers = new List<SplineLerperWorldUI>();

    public int PsychoEffectRate
    {
        get
        {
            return _psychoEffectRate;
        }
        set
        {
            if(value < 0)
            {
                _psychoEffectRate = 0;
            }
            else if( value >= 10)
            {
                _psychoEffectRate = 10;
            }
            _psychoEffectRate = value;

            InvokeEvent("PsychoEffectRefresh");

            if(_psychoEffectRate > 9)
            {
                ShadedViewUI.Instance.Show(2);
            }
            else if (_psychoEffectRate > 6)
            {
                ShadedViewUI.Instance.Show(1);
            }
            else if (_psychoEffectRate > 3)
            {
                ShadedViewUI.Instance.Show(0);
            }
            else
            {
                ShadedViewUI.Instance.Hide();
            }
        }
    }
    int _psychoEffectRate;

    public static SessionStats Stats;

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

        if(PlayerPrefs.GetInt("FirstTimePlaying",1) == 1)
        {
            PlayerPrefs.SetInt("FirstTimePlaying", 0);
            for (int i=0;i< Screen.resolutions.Length;i++)
            {
                if(Screen.resolutions[i].width == 1920 && Screen.resolutions[i].height == 1080)
                {
                    Screen.SetResolution(1920, 1080, true);
                    break;
                }
            }
        }

        Screen.fullScreen = PlayerPrefs.GetInt("WindowOn", 0) == 0;

        DontDestroyOnLoad(this.gameObject);
        Stats = new SessionStats();
        Instance = this;
    }

    private void OnApplicationPause(bool pause)
    {
        if (ListenerOfSound != null)
        {
            ListenerOfSound.enabled = false;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (ListenerOfSound != null)
        {
            ListenerOfSound.enabled = true;
        }
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
            QuestsPanelUI.Instance.AddNewExistingQuest(questClone);
        }

        DelayedInvokation(5f, () => TutorialScreenUI.Instance.Show("FirstGame"));

        LoadingGameRoutine = null;

        DelayedInvokation(1f, ()=> InvokeEvent("NewGameComplete"));
    }

    IEnumerator LoadMainScene(Faction selectedFaction, bool isLoadedFromSave = false)
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

        Factions.Clear();
        foreach (Faction faction in Database.Factions)
        {
            Factions.Add(faction.Clone());
        }

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

            if (tempCharacter.IsAlwaysKnown)
            {
                tempCharacter.Known.KnowEverything(PC);
            }

            Characters.Add(tempCharacter);
        }

        foreach (Faction faction in Factions)
        {
            if (faction.FactionHead != null)
            {
                Character factionHead = CORE.Instance.GetCharacter(faction.FactionHead.name);
                
                if(factionHead != null)
                {
                    factionHead.CurrentFaction = faction;
                    faction.FactionHead = factionHead;
                    faction.factionHeadID = factionHead.ID;
                }

                if (faction.isAlwaysKnown)
                {
                    faction.Known.KnowAll("Existance");
                    factionHead.Known.KnowAll("Name");
                    factionHead.Known.KnowAll("Faction");
                }
            }
        }

        while(PresetLocations.Count == 0)
        {
            yield return 0;
        }

        yield return 0;
        selectedFaction = Factions.Find(x => x.name == selectedFaction.name);

        foreach (LocationEntity presetLocation in PresetLocations)
        {
            presetLocation.InitializePreset();
            CORE.Instance.Locations.Add(presetLocation);
        }

        foreach(Character character in Characters)
        {
            if (character.CurrentLocation == null)
            {
                character.GoToLocation(GetRandomLocationWithTrait(Database.PublicAreaTrait));
            }

            if (character.HomeLocation == null)
            {
                character.StartLivingIn(character.CurrentLocation);
            }
        }

        if (MapViewManager.Instance != null)
        {
            MapViewManager.Instance.HideMap();
            MapViewManager.Instance.MapElementsContainer.gameObject.SetActive(false);
        }


        RoomsManager.Instance.AddCurrentRoom();

        SessionRules = new SessionRulesManager();

        AddListeners();

        if (!isLoadedFromSave)
        {
            foreach (LetterPreset letter in CORE.PC.CurrentFaction.StartingLetters)
            {
                LetterDispenserEntity.Instance.DispenseLetter(new Letter(letter));
            }

            Factions.ForEach((faction) => {
                faction.Goals.ForEach((quest) =>
                {
                    Quest questClone = quest.CreateClone();
                    questClone.ForCharacter = Characters.Find(x => x.name == faction.FactionHead.name);

                    QuestsPanelUI.Instance.AddNewExistingQuest(questClone);
                });
            });
        }

        RecruitmentPools.Clear();
        foreach (RecruitmentPool pool in Database.RecruitmentPools)
        {
            RecruitmentPools.Add(pool.Clone());
        }

        InvokeEvent("MainSceneLoaded");

        AudioListener newListener = Camera.main.GetComponent<AudioListener>();
        if(newListener != null && newListener.enabled)
        {
            ListenerOfSound = newListener;
        }

        PsychoEffectRate = 0;
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

    public ScehemWon OnSchemeWin = new ScehemWon();

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
        if (GameClock.Instance.CurrentTurn % 15 == 0)
        {
            GlobalMessagePrompterUI.Instance.Show("Auto Save", 3f, Color.green);

            SavingGamePanel.SetActive(true);

            yield return new WaitForSeconds(1f);
            CORE.Instance.SaveGame("Auto Save");
            yield return 0;
        }

        try
        {

            //AI DECISIONS
            foreach (Faction faction in CORE.Instance.Factions)
            {
                faction.OnTurnPassed();

                if (faction.FactionHead == null)
                {
                    continue;
                }

                Character factionHead = CORE.Instance.Characters.Find(x => x.name == faction.FactionHead.name);

                if (factionHead == null)
                {
                    continue;
                }

                if (factionHead.AI == null)
                {
                    continue;
                }

                if (factionHead.name == CORE.PC.name)
                {
                    continue;
                }

                if (factionHead.IsDead)
                {
                    continue;
                }

                if (factionHead.CurrentFaction == null)
                {
                    factionHead.CurrentFaction = faction;
                }

                if (factionHead.CurrentFaction.name != faction.name)
                {
                    continue;
                }

                factionHead.AI.MakeDecision(factionHead);
            }
        }
        catch (System.Exception error)
        {
            SendFeedBack(error.Message);
        }

        yield return 0;


        TurnLoadingWindowUI.Instance.SetLoadingTitle("Locations... ");//(" + Locations.Count + ")

        for (int i = 0; i < Locations.Count; i++)
        {
            TurnLoadingWindowUI.Instance.SetProgress(i * 1f / (Locations.Count + Characters.Count) * 1f);

            try
            {
                Locations[i].TurnPassed();
            }
            catch (System.Exception error)
            {
                SendFeedBack(error.Message);
            }

            if (i % 2 == 0)
            {
                yield return 0;
            }
        }

        TurnLoadingWindowUI.Instance.SetLoadingTitle("Characters... ");//("+Characters.Count + ")

        for (int i = 0; i < Characters.Count; i++)
        {

            TurnLoadingWindowUI.Instance.SetProgress((Locations.Count + i) * 1f / (Locations.Count + Characters.Count) * 1f);

            try
            {
                Characters[i].OnTurnPassedAI();
            }
            catch (System.Exception error)
            {
                SendFeedBack(error.Message);
            }

            if (i % 2 == 0)
            {
                yield return 0;
            }
        }

        try
        {
            //if(GameClock.Instance.CurrentTimeOfDay == GameClock.GameTime.Morning)
            //{
            foreach (Faction faction in CORE.Instance.Factions)
            {
                if (faction.FactionHead == null)
                {
                    continue;
                }

                Character factionHead = GetCharacter(faction.FactionHead.name);

                if (factionHead == null)
                {
                    continue;
                }

                factionHead.CGold += faction.GoldGeneratedPerDay;
                factionHead.CConnections += faction.ConnectionsGeneratedPerDay;
                factionHead.CRumors += faction.RumorsGeneratedPerDay;
                factionHead.CProgress += faction.ProgressGeneratedPerDay;
                factionHead.Reputation += faction.ReputationGeneratedPerDay;

                if (faction.FactionHead.name == CORE.PC.name && faction.ReputationGeneratedPerDay != 0)
                {
                    TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("Daily Reputation " + faction.ReputationGeneratedPerDay,
                        ResourcesLoader.Instance.GetSprite("pointing"),
                        CORE.PC));
                }
            }
            //}

            bool shouldRemoveRule = false;
            List<SessionRule> rulesToRemove = new List<SessionRule>();

            foreach (SessionRule rule in SessionRules.Rules)
            {
                rule.PassTurn(out shouldRemoveRule);

                if (shouldRemoveRule)
                {
                    rulesToRemove.Add(rule);
                }
            }

            while (rulesToRemove.Count > 0)
            {
                SessionRules.Rules.Remove(rulesToRemove[0]);
                rulesToRemove.RemoveAt(0);
            }

            foreach (RecruitmentPool pool in RecruitmentPools)
            {
                if (pool.Characters.Count == 0)
                {
                    continue;
                }

                pool.Remove(pool.Characters[Random.Range(0, pool.Characters.Count)]);
            }

        }
        catch (System.Exception error)
        {
            SendFeedBack(error.Message);
        }

        if (GameClock.Instance.CurrentTurn % 3 == 0)
        {
            TurnLoadingWindowUI.Instance.SetLoadingTitle("Whispers...");

            yield return StartCoroutine(DisplayDayRumorRoutine());
        }


        TurnLoadingWindowUI.Instance.SetLoadingTitle("Cults");

        yield return 0;
        
        TechTreeItem cultTech = TechTree.Find(X => X.name == Database.CultTech.name);

        if (cultTech != null)
        {
            if (cultTech.IsResearched)
            {
                int CharactersConverted = 0;

                List<Character> Cultists = new List<Character>();
                List<Character> NonCultists = new List<Character>();

                for (int i = 0; i < Characters.Count; i++)
                {
                    if (i % 10 == 0)
                    {
                        yield return 0;
                    }

                    try
                    {
                        Character X = Characters[i];
                        if (!X.IsDisabled && !X.HiddenFromCharacterWindows && X.TopEmployer != X)
                        {
                            if (X.Traits.Find(T => T == Database.CultistTrait) != null)
                            {
                                Cultists.Add(X);
                            }
                            else
                            {
                                NonCultists.Add(X);
                            }
                        }

                    }
                    catch(System.Exception error)
                    {
                        SendFeedBack(error.Message);
                    }
                }


                if (NonCultists == null)
                {
                    NonCultists = new List<Character>();
                }

                if (NonCultists.Count > 0)
                {
                    if (Cultists.Count == 0)
                    {
                        NonCultists[Random.Range(0, NonCultists.Count)].AddTrait(Database.CultistTrait);
                        NonCultists[Random.Range(0, NonCultists.Count)].AddTrait(Database.CultistTrait);
                        NonCultists[Random.Range(0, NonCultists.Count)].AddTrait(Database.CultistTrait);
                        NonCultists[Random.Range(0, NonCultists.Count)].AddTrait(Database.CultistTrait);
                        NonCultists[Random.Range(0, NonCultists.Count)].AddTrait(Database.CultistTrait);
                        CharactersConverted += 5;
                    }

                    for (int i = 0; i < Cultists.Count; i++)
                    {
                        try
                        {
                            float rnd = Random.Range(0f, 1f);
                            if (rnd < 0.02f)
                            {
                                Character target = NonCultists[Random.Range(0, NonCultists.Count)];
                                target.Traits.Add(Database.CultistTrait);
                                CharactersConverted++;


                                CORE.Instance.ShowPortraitEffect(ResourcesLoader.Instance.GetRecycledObject("PortraitEffectJoinedCult"), target, target.CurrentLocation);
                            }
                            else if (rnd < 0.05f)
                            {
                                Character newChar = CORE.Instance.GenerateSimpleCharacter();
                                newChar.Randomize();
                                Characters.Add(newChar);
                                newChar.Traits.Add(Database.CultistTrait);
                                CharactersConverted++;
                                newChar.GoToLocation(CORE.Instance.GetRandomLocation());

                                CORE.Instance.ShowPortraitEffect(ResourcesLoader.Instance.GetRecycledObject("PortraitEffectJoinedCult"), newChar, newChar.CurrentLocation);
                            }

                        }
                        catch (System.Exception error)
                        {
                            SendFeedBack(error.Message);
                        }

                        if (i % 5 == 0)
                        {
                            yield return 0;
                        }
                    }
                }
                
                if (CharactersConverted > 0)
                {
                    TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(CharactersConverted + " characters have joined the cult this turn...", ResourcesLoader.Instance.GetSprite("therapy"), CORE.PC));
                }

                List<Character> DevotedCultists = new List<Character>();
                TechTreeItem religiousCultTech = TechTree.Find(X => X.name == Database.CultTechReligious.name);

                if (religiousCultTech != null)
                {
                    if (religiousCultTech.IsResearched)
                    {
                        List<Character> cultistsToRemove = new List<Character>();

                        for (int i = 0; i < Cultists.Count; i++)
                        {
                            if (i % 5 == 0)
                            {
                                yield return 0;
                            }

                            try
                            {

                                if (Cultists[i].Traits.Find(t => t == Database.CultistReligiousTrait) != null)
                                {
                                    DevotedCultists.Add(Cultists[i]);
                                    cultistsToRemove.Add(Cultists[i]);
                                }
                            }
                            catch (System.Exception error)
                            {
                                SendFeedBack(error.Message);
                            }
                        }

                        try
                        {
                            foreach (Character cultist in cultistsToRemove)
                            {
                                Cultists.Remove(cultist);
                            }
                        }
                        catch (System.Exception error)
                        {
                            SendFeedBack(error.Message);
                        }


                        int CharactersDevoted = 0;

                        if (Cultists.Count > 0)
                        {
                            try
                            {
                                if (DevotedCultists.Count == 0)
                                {
                                    Cultists[Random.Range(0, Cultists.Count)].AddTrait(Database.CultistReligiousTrait);
                                    Cultists[Random.Range(0, Cultists.Count)].AddTrait(Database.CultistReligiousTrait);
                                    Cultists[Random.Range(0, Cultists.Count)].AddTrait(Database.CultistReligiousTrait);
                                    CharactersDevoted += 3;
                                }
                            }
                            catch (System.Exception error)
                            {
                                SendFeedBack(error.Message);
                            }

                            for (int i = 0; i < DevotedCultists.Count; i++)
                            {
                                try
                                {
                                    if (Random.Range(0f, 1f) < 0.05f)
                                    {
                                        Cultists[Random.Range(0, Cultists.Count)].AddTrait(Database.CultistReligiousTrait);
                                        CharactersDevoted++;
                                    }
                                }
                                catch (System.Exception error)
                                {
                                    SendFeedBack(error.Message);
                                }

                                if (i % 5 == 0)
                                {
                                    yield return 0;
                                }
                            }
                        }

                        if (CharactersDevoted > 0)
                        {
                            TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance(CharactersDevoted + " cultists became devoted this turn.", ResourcesLoader.Instance.GetSprite("therapy"), CORE.PC));
                        }


                    }
                }

                try
                {
                    TechTreeItem rebels = CORE.Instance.TechTree.Find(X => X.name == "Rebels");

                    if (rebels != null && rebels.IsResearched)
                    {
                        PC.CGold += 5;
                        PC.CRumors += 5;
                        PC.CConnections += 5;
                        PC.CProgress += 5;

                        TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("+5 To ALL resources from your remote advocates.", ResourcesLoader.Instance.GetSprite("painting_rebel"), CORE.PC));
                    }
                

                    TechTreeItem cultCommercialTech = TechTree.Find(y => y.name == Database.CultistTechCommercial.name);

                    if (cultCommercialTech != null)
                    {
                        if (cultCommercialTech.IsResearched)
                        {
                            int commercialEarn = (Cultists.Count + DevotedCultists.Count) / 2;

                            PC.CGold += commercialEarn;

                            if (commercialEarn > 0)
                            {
                                if (PC.PropertiesOwned.Count > 0)
                                {
                                    SplineAnimationObject(
                                    "CoinCollectedWorld",
                                    PC.PropertiesOwned[0].transform,
                                    StatsViewUI.Instance.GoldText.transform,
                                    () => { StatsViewUI.Instance.RefreshGold(); },
                                    false);

                                    AudioControl.Instance.PlayInPosition("resource_gold", PC.PropertiesOwned[0].transform.position);
                                }

                                TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("+" + commercialEarn + " Gold from your Pyramid Scheme.", ResourcesLoader.Instance.GetSprite("painting_commercial"), CORE.PC));
                            }
                        }
                    }
                }
                catch (System.Exception error)
                {
                    SendFeedBack(error.Message);
                }
            }
        }

        try
        {
            TurnPassedRoutineInstance = null;

            if (CORE.PC.CurrentFaction.HasPsychoEffect)
            {
                if (Random.Range(0, 2) == 0)
                {
                    PsychoEffectRate++;
                }
            }
            else
            {
                PsychoEffectRate = 0;
            }
        }
        catch (System.Exception error)
        {
            SendFeedBack(error.Message);
        }
    }

    public void SendFeedBack(string text)
    {
        try
        {
            if (SendFeedbackInstance != null)
            {
                return;
            }

            SendFeedbackInstance = StartCoroutine(SendFeedbackRoutine(text));
        }
        catch (System.Exception error)
        {
            SendFeedBack(error.Message);
        }
    }

    Coroutine SendFeedbackInstance;
    IEnumerator SendFeedbackRoutine(string text)
    {
        WWWForm form = new WWWForm();

        form.AddField("entry.459344423", text);


        byte[] rawData = form.data;

        WWW request = new WWW(FEEDBACK_URL, form);

        while (!request.isDone)
        {
            yield return 0;
        }

        SendFeedbackInstance = null;
    }

    #endregion

    #region Misc

    #region Steam

    public void WinAchievement(string key)
    {
        Debug.Log("#### - ACHIEVEMENT " + key);

        if (AchievementsWonThisSession.Contains(key))
        {
            return;
        }

        AchievementsWonThisSession.Add(key);

        StartCoroutine(WinAchievementRoutine(key));
    }

    public List<string> AchievementsWonThisSession = new List<string>();

    IEnumerator WinAchievementRoutine(string key)
    {
        while (!SteamManager.Initialized)
        {
            yield return 0;
        }

        SteamUserStats.SetAchievement(key);
        SteamUserStats.StoreStats();
    }

    #endregion

    #region Portrait Effect

    public void ShowPortraitEffect(GameObject effect, Character character, LocationEntity targetLocation)
    {
        if(activePortraitEffects.ContainsKey(targetLocation))
        {
            activePortraitEffects[targetLocation]++;
        }
        else
        {
            activePortraitEffects.Add(targetLocation, 0);
        }

        DelayedInvokation(activePortraitEffects[targetLocation]*1.5f, () =>
        {
            GameObject effectObj = ResourcesLoader.Instance.GetRecycledObject(effect);
            effectObj.transform.SetParent(CORE.Instance.DisposableContainer.transform);
            effectObj.transform.localScale = Vector3.one;
            effectObj.GetComponent<PortraitUI>().SetCharacter(character);
            effectObj.GetComponent<WorldPositionLerperUI>().SetTransform(targetLocation.transform);

            effectObj.GetComponent<UnityEventInvokerEntity>().EventsList[0].AddListener(() =>
            {
                if (activePortraitEffects.ContainsKey(targetLocation))
                {
                    activePortraitEffects[targetLocation]--;

                    if (activePortraitEffects[targetLocation] <= 0)
                    {
                        activePortraitEffects.Remove(targetLocation);
                    }
                }
            });
        });
    }

    Dictionary<LocationEntity, int> activePortraitEffects = new Dictionary<LocationEntity, int>();

    #endregion

    public GameObject SplineAnimationObject(string prefabKey, Transform startPoint, Transform targetPoint, System.Action OnComplete = null, bool canvasElement = true, bool noZ = false)
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

        lerper.SetInfo(startPoint ,targetPoint, OnComplete, noZ);

        return prefabObj;
    }

    public void ShowHoverMessage(string content, Sprite icon, Transform targetTransform)
    {
        HoverPanelUI hoverPanel = ResourcesLoader.Instance.GetRecycledObject("HoverPanelUI").GetComponent<HoverPanelUI>();
        hoverPanel.transform.SetParent(CORE.Instance.MainCanvas.transform);
        hoverPanel.transform.SetAsFirstSibling();
        hoverPanel.Show(targetTransform, content, icon);
    }

    public void GainInformation(Transform Source, Character targetChar)
    {
        CORE.Instance.SplineAnimationObject(
         "PaperCollectedWorld",
         Source,
         InformationLogUI.Instance.transform,
         () => { },
         false);

        KnowledgeRumor rumor = targetChar.Known.GetRandomKnowledgeRumor();
        if (rumor != null)
        {
            targetChar.KnowledgeRumors.Add(rumor);
        }
        else
        {
            GlobalMessagePrompterUI.Instance.Show("You already know everything about " + targetChar.name,1f,Color.red);
        }
    }

    public void GenerateLongTermTask(LongTermTask task, Character requester, Character character, LocationEntity target, Character targetCharacter = null, int turnsLeft = -1, AgentAction actionPerTurn = null, AgentAction originAction = null)
    {
        LongTermTaskEntity longTermTask = Instantiate(ResourcesLoader.Instance.GetObject("LongTermTaskEntity")).GetComponent<LongTermTaskEntity>();

        longTermTask.transform.SetParent(MapViewManager.Instance.MapElementsContainer);
        longTermTask.transform.position = target.transform.position;
        longTermTask.SetInfo(task, requester, character, target, targetCharacter, turnsLeft, actionPerTurn, originAction);
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

        return GetCharacter(ID);
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

    public void SaveGame(string customName = "")
    {
        ReadAllSaveFiles();

        JSONClass savefile = new JSONClass();

        if (!string.IsNullOrEmpty(customName))
        {
            savefile["Name"] = customName;
        }
        else
        {
            savefile["Name"] = "Save" + SaveFiles.Count;
        }

        savefile["Version"] = Application.version.ToString();

        long UNIX = (long)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;

        savefile["Date"] = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        savefile["UNIX"] = UNIX.ToString();
        savefile["SelectedFaction"] = PlayerFaction.name;
        savefile["PlayerCharacter"] = PC.name;

        savefile["PlotsWithBrute"] = Stats.PlotsWithBrute.ToString();
        savefile["PlotsWithCunning"] = Stats.PlotsWithCunning.ToString();
        savefile["PlotsWithStealth"] = Stats.PlotsWithStealth.ToString();

        savefile["GameClock"] = GameClock.Instance.ToJSON();

        savefile["PsychoEffectRate"] = PsychoEffectRate.ToString();

        for (int i=0;i<RecruitmentPools.Count;i++)
        {
            savefile["RecruitmentPools"][i] = RecruitmentPools[i].ToJSON();
        }

        for (int i = 0; i < Factions.Count; i++)
        {
            savefile["Factions"][i] = Factions[i].ToJSON();
        }

        for (int i=0;i<Characters.Count;i++)
        {
            savefile["Characters"][i] = Characters[i].ToJSON();
        }

        for (int i = 0; i < Locations.Count; i++)
        {
            savefile["Locations"][i] = Locations[i].ToJSON();
        }

        savefile["Quests"] = QuestsPanelUI.Instance.ToJSON();
        savefile["Rooms"]  = RoomsManager.Instance.ToJSON();
        savefile["LetterDispenser"] = LetterDispenserEntity.Instance.ToJSON();
        savefile["LettersPanel"] = LettersPanelUI.Instance.ToJSON();
        savefile["TechTree"] = TechTree.ToJSON();
        savefile["SessionRules"] = SessionRules.ToJSON();

        savefile["DialogEntity"] = DialogEntity.Instance.ToJSON();


        string ePath = Application.dataPath + "/Saves/" + savefile["Name"] + ".json";
        JSONNode tempNode = (JSONNode)savefile;
        File.WriteAllText(ePath, tempNode.ToString());

        ReadAllSaveFiles();

        AudioControl.Instance.Play("save");

        SavingGamePanel.SetActive(false);
    }

    public void LoadGame(SaveFile file = null)
    {
        if(LoadingGameRoutine != null)
        {
            return;
        }

        AudioControl.Instance.Play("load");

        DisposeSessionElements();
        LoadingGameRoutine = StartCoroutine(LoadGameRoutine(file));
    }


    IEnumerator LoadGameRoutine(SaveFile file = null)
    {
        LoadingPanel.SetActive(true);

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

        yield return StartCoroutine(LoadMainScene(Database.GetFactionByName(file.Content["SelectedFaction"]),true));

        MapViewManager.Instance.ShowMap();
        MapViewManager.Instance.MapElementsContainer.gameObject.SetActive(true);

        if (file != null)
        {

            GameClock.Instance.FromJSON(file.Content["GameClock"]);

            Stats.PlotsWithBrute = int.Parse(file.Content["PlotsWithBrute"]);
            Stats.PlotsWithCunning = int.Parse(file.Content["PlotsWithCunning"]);
            Stats.PlotsWithStealth = int.Parse(file.Content["PlotsWithStealth"]);

            if (!string.IsNullOrEmpty(file.Content["PsychoEffectRate"]))
            {
                PsychoEffectRate = int.Parse(file.Content["PsychoEffectRate"]);
            }

            Factions.Clear();
            for (int i = 0; i < file.Content["Factions"].Count; i++)
            {
                Faction faction = Database.Factions.Find(x => x.name == file.Content["Factions"][i]["Key"].Value);

                if (faction != null)
                {
                    faction = faction.Clone();
                }
                else
                {
                    string clonedFrom = file.Content["Factions"][i]["ClonedFrom"].Value;
                    if (!string.IsNullOrEmpty(clonedFrom))
                    {
                        Faction clonedFromFaction = Database.Factions.Find(x => x.name == file.Content["Factions"][i]["ClonedFrom"].Value);

                        if (clonedFromFaction != null)
                        {
                            faction = clonedFromFaction.Clone();
                        }
                        else
                        {
                            faction = CORE.Instance.Database.DefaultFaction.Clone();
                        }
                    }
                    else
                    {
                        faction = CORE.Instance.Database.DefaultFaction.Clone();
                    }
                }

                faction.FromJSON(file.Content["Factions"][i]);

                Factions.Add(faction);
            }

            Characters.Clear();
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

            foreach(LocationEntity location in Locations)
            {
                if(PresetLocations.Find(x=>x.gameObject.name == location.gameObject.name) != null)
                {
                    continue;
                }

                Destroy(location.gameObject);
            }

            Locations.Clear();
            
            for (int i = 0; i < file.Content["Locations"].Count; i++)
            {
                LocationEntity tempLocation = GenerateNewLocation(Vector3.zero, new Quaternion(0,0,0,0));

                tempLocation.FromJSON(file.Content["Locations"][i]);

                LocationEntity presetLocation = PresetLocations.Find(x => x.gameObject.name == tempLocation.gameObject.name);
                if(presetLocation != null)
                {
                    tempLocation.PossiblePlayerActions.Clear();
                    tempLocation.PossiblePlayerActions.AddRange(presetLocation.PossiblePlayerActions);

                    tempLocation.PossibleAgentActions.Clear();
                    tempLocation.PossibleAgentActions.AddRange(presetLocation.PossibleAgentActions);
                }

                Locations.Add(tempLocation);

                yield return 0;
            }

            foreach (LocationEntity location in PresetLocations)
            {
                Destroy(location.gameObject);
            }
            PresetLocations.Clear();

            LetterDispenserEntity.Instance.FromJSON(file.Content["LetterDispenser"]);
            LettersPanelUI.Instance.FromJSON(file.Content["LettersPanel"]);
            QuestsPanelUI.Instance.FromJSON(file.Content["Quests"]);
            RoomsManager.Instance.FromJSON(file.Content["Rooms"]);
            TechTree.FromJSON(file.Content["TechTree"]);
            SessionRules.FromJSON(file.Content["SessionRules"]);

            
        }
        yield return 0;

        PC = GetCharacter(file.Content["PlayerCharacter"]);

        DialogEntity.Instance.FromJSON(file.Content["DialogEntity"]);



        foreach (Character character in Characters)
        {
            character.ImplementIDs();

            yield return 0;
        }

        foreach (Faction faction in Factions)
        {
            faction.ImplementIDs();

            yield return 0;
        }

        foreach (LocationEntity location in Locations)
        {
            location.ImplementIDs();

            yield return 0;
        }

        QuestsPanelUI.Instance.ImplementIDs();
        
        LetterDispenserEntity.Instance.ImplementIDs();
        LettersPanelUI.Instance.ImplementIDs();

        MapViewManager.Instance.HideMap();
        MapViewManager.Instance.MapElementsContainer.gameObject.SetActive(false);

        PC.Known.KnowEverything(PC);

        foreach (Faction faction in Factions)
        {
            if (faction.FactionHead != null)
            {
                Character head = CORE.Instance.Characters.Find(x => x.name == faction.FactionHead.name);

                if (head != null)
                {
                    head.CurrentFaction = faction;
                    faction.FactionHead = head;
                }
            }

            if (faction.isAlwaysKnown)
            {
                faction.Known.KnowAll("Existance");

                if (faction.FactionHead != null)
                {
                    //if(faction.FactionHead.Known == null)
                    //{
                    //    faction.FactionHead.InitKnowledge();
                    //}

                    faction.FactionHead.Known.KnowAll("Name");
                    faction.FactionHead.Known.KnowAll("Faction");
                }
            }
        }

        //Delayed recruitment pool... Why? Because fuck you.
        if (RecruitmentPools.Count > 0)
        {
            for (int i = 0; i < file.Content["RecruitmentPools"].Count; i++)
            {
                string key = file.Content["RecruitmentPools"][i]["Key"];
                RecruitmentPool pool = RecruitmentPools.Find(x => x.name == key);

                if (pool == null)
                {
                    continue;
                }

                pool.FromJSON(file.Content["RecruitmentPools"][i]);
            }
        }


        DelayedInvokation(1f, () =>
        {
            foreach (LocationEntity location in Locations)
            {
                location.RefreshState();
            }

            AudioControl.Instance.SetPlaylist();
        });

        LoadingPanel.SetActive(false);

        LoadingGameRoutine = null;

        InvokeEvent("GameLoadComplete");

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


        SaveFiles = SaveFiles.OrderByDescending(x => (long)(x.RealDate.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds).ToList();
    }

    public void RemoveSave(SaveFile currentSave)
    {
        File.Delete(currentSave.Path);
        ReadAllSaveFiles();
    }


    public void DisposeCurrentGame()
    {
        DisposeSessionElements();
        //WorldMissionPanelUI.Instance.gameObject.SetActive(false);
        //InvokeEvent("HideMap");
        Destroy(this.gameObject);
    }

    public void DisposeSessionElements()
    {
        InvokeEvent("HideMap");

        for (int i = 0; i < DisposableContainer.childCount; i++)
        {
            Destroy(DisposableContainer.GetChild(i).gameObject, 0.05f);
        }
    }

    #endregion

    #region 3Day Rumors

    public IEnumerator DisplayDayRumorRoutine()
    {
        if(Random.Range(0,3) == 0)
        {
            yield break;
        }

        List<PopupData> possibleRumors = new List<PopupData>();

        foreach(DayRumor rumor in DayRumors)
        {
            PopupData temp = null;
            try
            {
                temp = rumor.GetPopup();
            }
            catch (System.Exception error)
            {
                SendFeedBack(error.Message);
            }

            if (temp == null)
            {
                continue;
            }

            possibleRumors.Add(temp);

            yield return 0;
        }

        if(possibleRumors.Count <= 0)
        {
            yield break;
        }

        PopupWindowUI.Instance.AddPopup(possibleRumors[Random.Range(0, possibleRumors.Count)]);
    }


    #endregion

    public void FadeOutScreen()
    {
        if(FadeScreenRoutineInstance != null)
        {
            StopCoroutine(FadeScreenRoutineInstance);
        }

        FadeScreenRoutineInstance = StartCoroutine(FadeOutScreenRoutine());
    }

    public void FadeInScreen()
    {
        if (FadeScreenRoutineInstance != null)
        {
            StopCoroutine(FadeScreenRoutineInstance);
        }

        FadeScreenRoutineInstance = StartCoroutine(FadeInScreenRoutine());
    }

    public Coroutine FadeScreenRoutineInstance;
    IEnumerator FadeInScreenRoutine()
    {
        ScreenFader.gameObject.SetActive(true);
        float t = 0f;
        while(t<1f)
        {

            t += 0.5f * Time.deltaTime;

            ScreenFader.color = Color.Lerp(ScreenFader.color, Color.clear, t);

            yield return 0;
        }

        ScreenFader.gameObject.SetActive(false);
        FadeScreenRoutineInstance = null;
    }

    IEnumerator FadeOutScreenRoutine()
    {
        ScreenFader.gameObject.SetActive(true);
        float t = 0f;
        while (t < 1f)
        {

            t += 0.5f * Time.deltaTime;

            ScreenFader.color = Color.Lerp(ScreenFader.color, Color.black, t);

            yield return 0;
        }

        FadeScreenRoutineInstance = null;
    }


}

public class ScehemWon : UnityEvent<SchemeType, LocationEntity, Character>
{

}

public class SaveFile
{
    public SaveFile(string content, string path)
    {
        try
        {
            this.Content = JSON.Parse(content);

            this.Corrupt = false;
            this.Name = this.Content["Name"].Value;
            this.Date = this.Content["Date"].Value;

            if (!string.IsNullOrEmpty(this.Content["UNIX"]))
            {
                this.RealDate = new System.DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
                this.RealDate = this.RealDate.AddSeconds(long.Parse(this.Content["UNIX"]));
            }
        }
        catch (System.Exception error)
        {
            CORE.Instance.SendFeedBack(error.Message);
            this.Corrupt = true;
            this.Name = "Save File's Json is broken...";
            this.Date = "Fixing it will most likely restore this file...";
            this.RealDate = System.DateTime.Now;
        }
            

        this.Path = path;
    }

    public System.DateTime RealDate;
    public string Name;
    public string Date;
    public string Path;
    public bool Corrupt = false;
    public JSONNode Content;
}

[System.Serializable]
public class SessionRulesManager
{
    public List<SessionRule> Rules = new List<SessionRule>();

    public void FromJSON(JSONNode node)
    {
        Rules.Clear();

        for (int i = 0; i < node["Rules"].Count; i++)
        {
            SessionRule rule = CORE.Instance.Database.SessionRules.Find(x => x.name == node["Rules"][i]["Name"].Value);
                
            if(rule == null)
            {
                Debug.LogError("Rule " + node["Rules"][i]["Name"].Value + " DOES NOT EXIST IN DB");
                return;
            }

            rule.Clone();
            Rules.Add(rule);
        }
    }

    public void ImplementIDs()
    {
        
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        for(int i=0;i<Rules.Count;i++)
        {
            node["Rules"][i] = Rules[i].ToJSON();
        }

        return node;
    }
}

public class SessionStats
{
    public int PlotsWithBrute;
    public int PlotsWithStealth;
    public int PlotsWithCunning;
}