using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;

public class GameClock : MonoBehaviour, ISaveFileCompatible
{
    public static GameClock Instance;

    public GameTime CurrentTimeOfDay = GameTime.Morning;

    public int CurrentWeek
    {
        get
        {
            return Mathf.FloorToInt(CurrentDay / 7f);
        }
    }

    public int CurrentDay
    {
        get
        {
            return Mathf.FloorToInt(CurrentTurn / 5f);
        }
    }

    public int CurrentTurn = 0;

    public UnityEvent OnTurnPassed = new UnityEvent();

    public UnityEvent OnDayPassed = new UnityEvent();

    public UnityEvent OnWeekPassed = new UnityEvent();

    public Quest LockingQuest;
    public LetterPreset LockingLetter;


    float turnSpamTimer;
    int turnSpamCounter;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    void Update()
    {
        turnSpamTimer += 1f * Time.deltaTime;
    }

    public void Initialize()
    {
        CORE.Instance.SubscribeToEvent("PassTime", PassTime);
    }

    public void Wipe()
    {
        OnTurnPassed.RemoveAllListeners();
        OnDayPassed.RemoveAllListeners();
        LockingQuest = null;
        LockingLetter= null;
    }

    public void PassTime()
    {
        if (PassTimeRoutineInstance != null)
        {
            return;
        }

        if(LockingQuest != null)
        {
            GlobalMessagePrompterUI.Instance.Show("Can not pass time, you must first complete the quest: " + LockingQuest.name,3f,Color.red);
            return;
        }

        if (LockingLetter != null)
        {
            GlobalMessagePrompterUI.Instance.Show("Can not pass time, an important letter is waiting for you.", 3f, Color.red);
            return;
        }

        if(turnSpamTimer <= 3f)
        {
            turnSpamCounter++;
        }
        else
        {
            turnSpamCounter = 0;
        }

        if(turnSpamCounter == 5)
        {
            List<Character> agents = CORE.PC.CharactersInCommand;

            if(agents != null && agents.Count  > 0)
            {

                LetterPreset letterPreset = CORE.Instance.Database.TurnSpamLetter;

                Dictionary<string, object> letterParameters = new Dictionary<string, object>();

                letterParameters.Add("Letter_From", agents[Random.Range(0, agents.Count)]);
                letterParameters.Add("Letter_To", CORE.PC);

                LetterDispenserEntity.Instance.DispenseLetter(new Letter(letterPreset, letterParameters));
            }
        }

        if (turnSpamCounter == 10)
        {
            List<Character> agents = CORE.PC.CharactersInCommand;

            if (agents != null && agents.Count > 0)
            {

                PopupDataPreset popPreset = CORE.Instance.Database.TurnSpamPopup;

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                parameters.Add("From", agents[0].name);
                parameters.Add("To", CORE.PC.name);

                PopupWindowUI.Instance.AddPopup(new PopupData(popPreset, new List<Character>() { agents[0] }, new List < Character >() { CORE.PC }, null, parameters));
            }
        }

        if (turnSpamCounter > 10)
        {
            List<Character> agents = CORE.PC.CharactersInCommand;

            if (agents != null && agents.Count > 0)
            {
                foreach(Character agent in agents)
                {
                    agent.DynamicRelationsModifiers.Add(new DynamicRelationsModifier(new RelationsModifier(CORE.PC.name + " is no-where to be found...", -2), 10, CORE.PC));
                }
            }
        }

        PassTimeRoutineInstance = StartCoroutine(PassTimeRoutine());
    }

    public Coroutine PassTimeRoutineInstance;
    IEnumerator PassTimeRoutine()
    {
        AudioControl.Instance.Play("turn_pass");
        AudioControl.Instance.Play("turn_pass_loading");
        CORE.Instance.InvokeEvent("PassTimeStarted");

        int currentWeek = CurrentWeek;

        CurrentTimeOfDay++;
        CurrentTurn++;

        if (CurrentTimeOfDay > GameTime.Night)
        {
            CurrentTimeOfDay = GameTime.Morning;

            OnDayPassed.Invoke();
        }

        if (currentWeek != CurrentWeek)
        {
            OnWeekPassed.Invoke();
        }

        CORE.Instance.InvokeEvent(CurrentTimeOfDay.ToString());
        OnTurnPassed.Invoke();

        GlobalMessagePrompterUI.Instance.Show(CurrentTimeOfDay.ToString(), 1f);

        yield return 0;

        while (CORE.Instance.TurnPassedRoutineInstance != null)
        {
            yield return 0;
        }

        CORE.Instance.InvokeEvent("PassTimeComplete");

        AudioControl.Instance.StopSound("turn_pass_loading");
        PassTimeRoutineInstance = null;

        turnSpamTimer = 0;
    }

    public JSONNode ToJSON()
    {
        JSONClass node = new JSONClass();

        node["CurrentTurn"] = CurrentTurn.ToString();

        return node;
    }

    public void FromJSON(JSONNode node)
    {
        CurrentTurn = int.Parse(node["CurrentTurn"]);
    }

    public void ImplementIDs()
    {
    }

    [System.Serializable]
    public enum GameTime
    {
        Morning,
        Noon,
        Afternoon,
        Evening,
        Night
    }

    public class GameTimeLength
    {
        public int Days;
        public int DayTime;

        public GameTimeLength(int turns)
        {
            //Days = Mathf.FloorToInt((float)turns / 5f);
            Days = Mathf.FloorToInt((float)(turns + GameClock.Instance.CurrentTimeOfDay) / 5f);

            DayTime = (int)GameClock.Instance.CurrentTimeOfDay + (turns - (Days * 5));
        }
    }



}
