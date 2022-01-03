using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ForgeryWindowUI : MonoBehaviour
{
    public static ForgeryWindowUI Instance;

    [SerializeField]
    public Transform CaseElementsContainer;

    [SerializeField]
    GameObject VSCharacterPanel;

    [SerializeField]
    GameObject VSLocationPanel;

    public TextMeshProUGUI TargetNameLabel;

    public TextMeshProUGUI TargetLocationOwnerLabel;
    public TextMeshProUGUI TargetLocationLabel;

    public TextMeshProUGUI CaseStrengthLabel;

    public PortraitUI TargetPortrait;

    public PortraitUI LocationOwnerPortrait;
    public LocationPortraitUI LocationPortrait;

    public PortraitUI RepresentitivePortrait;

    public Character TargetCharacter;
    public LocationEntity TargetLocation;

    [FormerlySerializedAs("RelevantElements")]
    public List<ForgeryCaseElement> VSCharacterElements = new List<ForgeryCaseElement>();

    public List<ForgeryCaseElement> VSLocationElements = new List<ForgeryCaseElement>();

    public PopupDataPreset WinPopup;
    public PopupDataPreset LosePopup;

    public Character Representitive;

    public bool IsVsCharacter
    {
        get
        {

            return TargetCharacter != null;
        }
    }

    public int StrengthPerElemnent
    {
        get
        {
            if (IsVsCharacter)
            {
                return Mathf.FloorToInt(100f / VSCharacterElements.Count);
            }
            else
            {
                return Mathf.FloorToInt(100f / VSLocationElements.Count);
            }
        }
    }

    public int CaseStrength;

    private void Awake()
    {
        Instance = this;
        HideWindow();
    }

    private void OnDisable()
    {
        if (AudioControl.Instance != null)
        {
            AudioControl.Instance.StopSound("soundscape_case");
            AudioControl.Instance.UnmuteMusic();
        }

        if (MouseLook.Instance == null) return;

        MouseLook.Instance.CurrentWindow = null;

    }

    public void Show(Character targetCharacter)
    {
        this.gameObject.SetActive(true);
        VSCharacterPanel.SetActive(true);
        VSLocationPanel.SetActive(false);

        TargetCharacter = targetCharacter;
        TargetLocation = null;

        MouseLook.Instance.CurrentWindow = this.gameObject;


        AudioControl.Instance.Play("soundscape_case", true);
        AudioControl.Instance.MuteMusic();

        TargetPortrait.SetCharacter(TargetCharacter);

        TargetNameLabel.text = targetCharacter.name;

        RefreshElements();
    }

    public void Show(LocationEntity targetLocation)
    {
        if(targetLocation.OwnerCharacter == null)
        {
            GlobalMessagePrompterUI.Instance.Show("No one owns this porperty...", 1f, Color.red);
            return;
        }

        this.gameObject.SetActive(true);
        VSCharacterPanel.SetActive(false);
        VSLocationPanel.SetActive(true);

        TargetLocation = targetLocation;
        TargetCharacter = null;

        MouseLook.Instance.CurrentWindow = this.gameObject;

        AudioControl.Instance.Play("soundscape_case", true);
        AudioControl.Instance.MuteMusic();

        LocationPortrait.SetLocation(TargetLocation);

        TargetLocationLabel.text = TargetLocation.Name;
        
        LocationOwnerPortrait.SetCharacter(TargetLocation.OwnerCharacter);
        TargetLocationOwnerLabel.text = TargetLocation.OwnerCharacter.name;


        RefreshElements();
    }

    public void SetRepresentitive()
    {
        SelectAgentWindowUI.Instance.Show(x => 
        {
            Representitive = x;
            RepresentitivePortrait.SetCharacter(Representitive);
        }
        , x => x.IsAgent, "Select Representitive:");
    }

    void RefreshElements()
    {
        List<ForgeryCaseElement> ownedElements = new List<ForgeryCaseElement>();
        List<ForgeryCaseElement> relevantElements;


        if (IsVsCharacter)
        {
            relevantElements = VSCharacterElements;
            ownedElements.AddRange(relevantElements.FindAll(x => TargetCharacter.CaseElements.Find(y => x.name == y.name) != null));
        }
        else
        {
            relevantElements = VSLocationElements;
            ownedElements.AddRange(relevantElements.FindAll(x => TargetLocation.CaseElements.Find(y => x.name == y.name) != null));
        }

        CaseStrength = (ownedElements.Count * StrengthPerElemnent);
        

        CaseStrengthLabel.text = CaseStrength + "%";

        ClearPoolsContainers();

        RepresentitivePortrait.SetCharacter(Representitive);

        foreach (ForgeryCaseElement element in relevantElements)
        {
            ForgeryCaseElementUI elementUI = ResourcesLoader.Instance.GetRecycledObject("ForgeryCaseElementUI").GetComponent<ForgeryCaseElementUI>();
            elementUI.transform.SetParent(CaseElementsContainer, false);
            elementUI.transform.localScale = Vector3.one;
            elementUI.transform.position = Vector3.one;

            if (IsVsCharacter)
            {
                elementUI.SetInfo(element, TargetCharacter, () =>
                {
                    RefreshElements();

                });
            }
            else
            {
                elementUI.SetInfo(element, TargetLocation, () =>
                {
                    RefreshElements();

                });
            }
        }
    }
    
    void ClearPoolsContainers()
    {
        while (CaseElementsContainer.childCount > 0)
        {
            CaseElementsContainer.GetChild(0).gameObject.SetActive(false);
            CaseElementsContainer.GetChild(0).transform.SetParent(transform);
        }
    }
    
    void HideWindow()
    {
        this.gameObject.SetActive(false);
    }

    public void Enforce()
    {
        if(Representitive == null)
        {
            GlobalMessagePrompterUI.Instance.Show("You must first select a representitive.", 1f, Color.red);
            return;
        }

        HideWindow();

        if (IsVsCharacter)
        {
            if (UnityEngine.Random.Range(0, 100) < CaseStrength)
            {
                PopupData popData = new PopupData(WinPopup, new List<Character> { Representitive }, new List<Character> { TargetCharacter }, null);
                popData.OnPopupDisplayed = () =>
                {
                    WarningWindowUI.Instance.Show("Your conviction of " + TargetCharacter.name + " has been found JUST!",
                    () =>
                    {
                        TargetCharacter.StopDoingCurrentTask(false);
                        CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(TargetCharacter.TopEmployer, TargetCharacter, TargetCharacter.CurrentLocation);
                        CORE.PC.Reputation -= 2;
                        AudioControl.Instance.Play("courtgood");

                        TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("Abused the law to scheme - Reputation -2",
                        ResourcesLoader.Instance.GetSprite("pointing"),
                        CORE.PC));
                    }
                    , true);
                };

                PopupWindowUI.Instance.AddPopup(popData);
            }
            else
            {

                PopupData popData = new PopupData(LosePopup, new List<Character> { Representitive }, new List<Character> { TargetCharacter }, null);
                popData.OnPopupDisplayed = () =>
                {
                    WarningWindowUI.Instance.Show("Your conviction of " + TargetCharacter.name + " has been found FALSE! (-3 Reputation)",
                    () =>
                    {
                        CORE.PC.Reputation -= 3;
                        AudioControl.Instance.Play("courtbad");

                        TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("Abused the law to scheme and faild - Reputation -3",
                        ResourcesLoader.Instance.GetSprite("pointing"),
                        CORE.PC));

                    }, true);
                };

                PopupWindowUI.Instance.AddPopup(popData);
            }


            TargetCharacter.CaseElements.Clear();
        }
        else
        {
            if (UnityEngine.Random.Range(0, 100) < CaseStrength)
            {
                List<Character> agents = new List<Character>();
                agents.AddRange(CORE.PC.CharactersInCommand.FindAll(x => x.IsAgent));

                Character agent = agents[Random.Range(0, agents.Count)];
                PopupData popData = new PopupData(WinPopup, new List<Character> { agent }, new List<Character> { TargetLocation.OwnerCharacter }, null);
                popData.OnPopupDisplayed = () =>
                {
                    WarningWindowUI.Instance.Show("Your claim over " + TargetLocation.Name + " has been JUSTIFIED!",
                    () =>
                    {
                        agent.StartOwningLocation(TargetLocation);

                        TargetLocation.RefreshPropertyValidity();

                        CORE.PC.Reputation -= 2;
                        AudioControl.Instance.Play("courtgood");

                        TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("Abused the law to scheme - Reputation -2",
                        ResourcesLoader.Instance.GetSprite("pointing"),
                        CORE.PC));
                    }
                    , true);
                };

                PopupWindowUI.Instance.AddPopup(popData);
            }
            else
            {
                List<Character> agents = new List<Character>();
                agents.AddRange(CORE.PC.CharactersInCommand.FindAll(x => x.IsAgent));
                Character agent = agents[Random.Range(0, agents.Count)];

                PopupData popData = new PopupData(LosePopup, new List<Character> { agent }, new List<Character> { TargetLocation.OwnerCharacter }, null);
                popData.OnPopupDisplayed = () =>
                {
                    WarningWindowUI.Instance.Show("Your claim over " + TargetLocation.Name + " has been found ILLEGITIMATE! (-3 Reputation)",
                    () =>
                    {
                        CORE.PC.Reputation -= 3;
                        AudioControl.Instance.Play("courtbad");

                        TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("Abused the law to scheme and failed- Reputation -3",
                        ResourcesLoader.Instance.GetSprite("pointing"),
                        CORE.PC));
                    }, true);
                };

                PopupWindowUI.Instance.AddPopup(popData);
            }


            TargetLocation.CaseElements.Clear();
        }
    }
}
