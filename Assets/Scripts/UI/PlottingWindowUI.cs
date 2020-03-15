using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlottingWindowUI : MonoBehaviour
{
    public static PlottingWindowUI Instance;
    
    [SerializeField]
    TextMeshProUGUI Title;
    
    [SerializeField]
    PortraitUI PlotterPortrait;

    [SerializeField]
    LocationPortraitUI TargetPortraitLocation;

    [SerializeField]
    PortraitUI TargetPortraitCharacter;

    [SerializeField]
    Transform ParticipantsContainer;

    [SerializeField]
    Transform TargetParticipantsContainer;

    [SerializeField]
    Transform ItemsRequiredContainer;

    [SerializeField]
    Image MethodIcon;

    [SerializeField]
    Image MethodFrame;

    [SerializeField]
    TextMeshProUGUI MethodName;

    [SerializeField]
    Image EntryIcon;

    [SerializeField]
    Image EntryFrame;

    [SerializeField]
    TextMeshProUGUI EntryName;

    [SerializeField]
    TextMeshProUGUI PlotChanceSuccess;



    [SerializeField]
    TextMeshProUGUI RelevantSkill;

    [SerializeField]
    TextMeshProUGUI RelevantSkillAvarage;

    [SerializeField]
    TextMeshProUGUI CounterSkill;

    [SerializeField]
    TextMeshProUGUI CounterSkillAvarage;

    [SerializeField]
    TextMeshProUGUI ChanceOfSuccess;

    [SerializeField]
    Image ApprovalImage;

    [SerializeField]
    Image ApprovalImageBG;

    [SerializeField]
    Button ExecuteButton;

    [SerializeField]
    TooltipTargetUI EntryTooltipTarget;

    [SerializeField]
    TooltipTargetUI MethodTooltipTarget;

    [SerializeField]
    TooltipTargetUI ExecuteTooltipTarget;




    SchemeType CurrentSchemeType;
    AgentInteractable CurrentTarget;
    Character CurrentPlotter;

    public List<Character> Participants = new List<Character>();
    public List<Character> TargetParticipants = new List<Character>();

    public PlotMethod CurrentMethod;
    public PlotEntry CurrentEntry;

    private void OnDisable()
    {
        AudioControl.Instance.StopSound("soundscape_plotting");
    }

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(AgentInteractable target, SchemeType type,  Character plotter = null)
    {
        MouseLook.Instance.CurrentWindow = this.gameObject;

        TargetParticipants.Clear();
        Participants.Clear();

        CurrentTarget = target;
        CurrentSchemeType = type;
        CurrentPlotter = plotter;


        if (target == null)
        {
            GlobalMessagePrompterUI.Instance.Show("No Target", 1f, Color.red);
            Hide();
            return;
        }

        if (CurrentTarget.GetType() == typeof(PortraitUI) || target.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetCharacter = ((PortraitUI)CurrentTarget).CurrentCharacter;

            if (targetCharacter == plotter)
            {
                Hide();
                GlobalMessagePrompterUI.Instance.Show(plotter.name + " is not stupid.", 1f, Color.red);
                return;
            }

            if (targetCharacter.PrisonLocation == null && targetCharacter.Age >= 15 && !targetCharacter.IsInTrouble)
            {
                TargetParticipants.Add(targetCharacter);
            }

            TargetParticipants.AddRange(targetCharacter.GuardsInCommand.FindAll(x =>
            x.PrisonLocation == null
            && !x.IsPuppetOf(CurrentPlotter.CurrentFaction)
            && !x.IsInTrouble));

            TargetParticipants.AddRange(targetCharacter.CurrentLocation.CharactersInLocation.FindAll(x =>
            x.CurrentFaction == targetCharacter.CurrentFaction
            && x.PrisonLocation == null
            && !x.IsPuppetOf(CurrentPlotter.CurrentFaction)
            && !x.IsInTrouble 
            && !TargetParticipants.Contains(x)));
        }
        else if (CurrentTarget.GetType() == typeof(LocationEntity))
        {
            LocationEntity location = ((LocationEntity)CurrentTarget);

            if (location.OwnerCharacter != null)
            {
                TargetParticipants.AddRange(CORE.Instance.Characters.FindAll(x =>
                       !TargetParticipants.Contains(x)
                    && x.PrisonLocation == null
                    && !x.IsPuppetOf(CurrentPlotter.CurrentFaction)
                    && x.CurrentLocation == location
                    && x.Age >= 15
                    && (x.TopEmployer == location.OwnerCharacter.TopEmployer || x.CurrentFaction.name == "Constabulary")));
            }
        }


        CurrentMethod = CurrentSchemeType.PossibleMethods[0];
        CurrentEntry  = CurrentSchemeType.PossibleEntries[0];

        CORE.Instance.DelayedInvokation(0.1f, RefreshUI);

        AudioControl.Instance.Play("soundscape_plotting", true);

        this.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
    }

    public void Hide()
    {
        MouseLook.Instance.CurrentWindow = null;
        this.gameObject.SetActive(false);
    }

    void RefreshUI()
    { 
        if (CurrentPlotter != null)
        {
            PlotterPortrait.SetCharacter(CurrentPlotter);
        }

        if (CurrentTarget.GetType() == typeof(LocationEntity))
        {
            TargetPortraitLocation.gameObject.SetActive(true);
            TargetPortraitCharacter.gameObject.SetActive(false);

            TargetPortraitLocation.SetLocation((LocationEntity)CurrentTarget);
        }
        else if (CurrentTarget.GetType() == typeof(PortraitUI) || CurrentTarget.GetType() == typeof(PortraitUIEmployee))
        {
            TargetPortraitLocation.gameObject.SetActive(false);
            TargetPortraitCharacter.gameObject.SetActive(true);

            TargetPortraitCharacter.SetCharacter(((PortraitUI)CurrentTarget).CurrentCharacter);
        }

        Title.text = CurrentSchemeType.name;

        ClearContainers();

        foreach(Character character in Participants)
        {
            foreach(Item item in CurrentMethod.ItemsRequired)
            {
                GameObject itemObj = ResourcesLoader.Instance.GetRecycledObject("ItemUI");
                itemObj.transform.SetParent(ItemsRequiredContainer, false);
                itemObj.transform.localScale = Vector3.one;
                itemObj.GetComponent<ItemUI>().SetInfo(item);
            }
        }

        foreach (Item item in CurrentEntry.ItemsRequired)
        {
            GameObject itemObj = ResourcesLoader.Instance.GetRecycledObject("ItemUI");
            itemObj.transform.SetParent(ItemsRequiredContainer, false);
            itemObj.transform.localScale = Vector3.one;
            itemObj.GetComponent<ItemUI>().SetInfo(item);
        }

        foreach (Character character in Participants)
        {
            GameObject portraitObj = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
            portraitObj.transform.SetParent(ParticipantsContainer, false);
            portraitObj.transform.localScale = Vector3.one;
            portraitObj.transform.position = Vector3.zero;
            portraitObj.GetComponent<PortraitUI>().SetCharacter(character);
        }

        foreach (Character character in TargetParticipants)
        {
            GameObject portraitObj = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
            portraitObj.transform.SetParent(TargetParticipantsContainer, false);
            portraitObj.transform.localScale = Vector3.one;
            portraitObj.transform.position = Vector3.zero;
            portraitObj.GetComponent<PortraitUI>().SetCharacter(character);
        }

        FailReason entryRequirements = CurrentEntry.AreRequirementsMet(CurrentPlotter.TopEmployer, CurrentTarget);
        EntryIcon.sprite = CurrentEntry.Icon;
        EntryFrame.color =  entryRequirements == null ? Color.black : Color.red;
        EntryName.text = (entryRequirements == null ? "<color=yellow>" : "<color=red>") + CurrentEntry.name + "</color>";

        List<TooltipBonus> entryTTBonuses = new List<TooltipBonus>();
        entryTTBonuses.Add(new TooltipBonus("<color=green>" + "+" + CurrentEntry.BonusToSkill + " " + CurrentEntry.Skill.name + "</color>", CurrentEntry.Skill.icon));
        if (entryRequirements != null)
        {
            entryTTBonuses.Add(new TooltipBonus("<color=yellow>" + CurrentEntry.Skill.name + " +"+CurrentEntry.BonusToSkill+"</color>", ResourcesLoader.Instance.GetSprite("Unsatisfied")));
            entryTTBonuses.Add(new TooltipBonus("<color=red>" + entryRequirements.Key + "</color>", ResourcesLoader.Instance.GetSprite("Unsatisfied")));
        }

        EntryTooltipTarget.SetTooltip(CurrentEntry.name + " \n " + CurrentEntry.Description, entryTTBonuses);


        FailReason methodRequirements = CurrentMethod.AreRequirementsMet(Participants.ToArray());
        MethodIcon.sprite = CurrentMethod.Icon;
        MethodFrame.color = methodRequirements == null? Color.black : Color.red;
        MethodName.text = (methodRequirements == null ? "<color=yellow>" : "<color=red>") + CurrentMethod.name + "</color>";

        List<TooltipBonus> methodTTBonuses = new List<TooltipBonus>();
        if (methodRequirements != null)
        {
            methodTTBonuses.Add(new TooltipBonus("<color=red>"+methodRequirements.Key + "</color>", ResourcesLoader.Instance.GetSprite("Unsatisfied")));
        }
        MethodTooltipTarget.SetTooltip(CurrentMethod.name + " \n " + CurrentMethod.Description, methodTTBonuses);


        RelevantSkill.text = CurrentMethod.OffenseSkill.name;
        if (Participants.Count > 0)
        {
            float totalSkill = 0f;
            foreach (Character character in Participants)
            {
                totalSkill += character.GetBonus(CurrentMethod.OffenseSkill).Value;
            }
            totalSkill /= Participants.Count;

            RelevantSkillAvarage.text = Mathf.RoundToInt(totalSkill).ToString();
        }
        else
        {
            RelevantSkillAvarage.text = "No Participants";
        }

        CounterSkill.text = CurrentMethod.DefenceSkill.name;
        if (TargetParticipants.Count > 0)
        {
            float totalSkill = 0f;
            foreach (Character character in TargetParticipants)
            {
                totalSkill += character.GetBonus(CurrentMethod.DefenceSkill).Value;
            }
            totalSkill /= TargetParticipants.Count;

            CounterSkillAvarage.text = Mathf.RoundToInt(totalSkill).ToString();
        }
        else
        {
            CounterSkillAvarage.text = "No One Stops You";
        }

        ExecuteButton.interactable = entryRequirements == null && methodRequirements == null;

        List<TooltipBonus> bonuses = new List<TooltipBonus>();

        if (entryRequirements != null)
        {
            bonuses.Add(new TooltipBonus("<color=red>"+entryRequirements.Key+"</color>", ResourcesLoader.Instance.GetSprite("Unsatisfied")));
        }

        if (methodRequirements!= null)
        {
            bonuses.Add(new TooltipBonus("<color=red>" + methodRequirements.Key + "</color>", ResourcesLoader.Instance.GetSprite("Unsatisfied")));
        }

        ExecuteTooltipTarget.SetTooltip("Execute The Plan!", bonuses);


        float participantsValue = 0;
        foreach(Character character in Participants)
        {
            participantsValue += character.GetBonus(CurrentMethod.OffenseSkill).Value;
        }

        float targetsValue = 0;
        foreach (Character character in TargetParticipants)
        {
            targetsValue += character.GetBonus(CurrentMethod.DefenceSkill).Value;
        }

        if (Participants.Count == 0)
        {
            PlotChanceSuccess.text = "N/A";
        }
        else if (TargetParticipants.Count == 0)
        {
            PlotChanceSuccess.text = "100%";
        }
        else
        {
            PlotChanceSuccess.text = Mathf.RoundToInt((participantsValue / (participantsValue + targetsValue)) * 100f) + "%";
        }
    }

    void ClearContainers()
    {
        while(ParticipantsContainer.childCount > 0)
        {
            ParticipantsContainer.GetChild(0).gameObject.SetActive(false);
            ParticipantsContainer.GetChild(0).SetParent(transform);
        }

        while (TargetParticipantsContainer.childCount > 0)
        {
            TargetParticipantsContainer.GetChild(0).gameObject.SetActive(false);
            TargetParticipantsContainer.GetChild(0).SetParent(transform);
        }

        while (ItemsRequiredContainer.childCount > 0)
        {
            ItemsRequiredContainer.GetChild(0).gameObject.SetActive(false);
            ItemsRequiredContainer.GetChild(0).SetParent(transform);
        }
    }




    public void Execute()
    {
        PlotData Plot = new PlotData(CurrentSchemeType.name,CORE.PC, CurrentPlotter, Participants, TargetParticipants, CurrentTarget, CurrentMethod, CurrentEntry);

        Plot.BaseMethod = CurrentSchemeType.BaseMethod;

        CurrentSchemeType.Execute(Plot);
        Hide();
    }
     
    public void AddParticipant()
    {
        if(Participants.Count >= 9)
        {
            return;
        }

        //TODO add is puppet support?
        SelectCharacterViewUI.Instance.Show(
            x => { Participants.Add(x); RefreshUI(); },
            x => 
            x.TopEmployer == CORE.PC 
            && x.TopEmployer != x 
            && x.Age > 15
            && !Participants.Contains(x)
            && !TargetParticipants.Contains(x)
            && x != CurrentTarget
            && !x.IsDead 
            && x.PrisonLocation == null
            && x.IsAgent
            && (x.CurrentTaskEntity == null || x.CurrentTaskEntity.CurrentTask.Cancelable),
            "Add Participant:");
    }

    public void RemoveLastParticipant()
    {
        Participants.RemoveAt(Participants.Count - 1);
        RefreshUI();
    }

    public void ReplacePlotter()
    {
        SelectAgentWindowUI.Instance.Show(
            x => { CurrentPlotter = x; RefreshUI();  },
            x => x.TopEmployer == CORE.PC && !x.IsDead && x.CurrentTaskEntity == null && x.IsAgent && x.Age > 15,
            "Select Plotter:");
    }

    public void ShowNextMethod()
    {
        int methodIndex = CurrentSchemeType.PossibleMethods.IndexOf(CurrentMethod);
        methodIndex++;

        if(methodIndex >= CurrentSchemeType.PossibleMethods.Count)
        {
            methodIndex = 0;
        }

        CurrentMethod = CurrentSchemeType.PossibleMethods[methodIndex];

        RefreshUI();
    }

    public void ShowPreviousMethod()
    {
        int methodIndex = CurrentSchemeType.PossibleMethods.IndexOf(CurrentMethod);
        methodIndex--;

        if (methodIndex < 0)
        {
            methodIndex = CurrentSchemeType.PossibleMethods.Count - 1;
        }

        CurrentMethod = CurrentSchemeType.PossibleMethods[methodIndex];

        RefreshUI();
    }

    public void ShowNextEntry()
    {
        int entryIndex = CurrentSchemeType.PossibleEntries.IndexOf(CurrentEntry);
        entryIndex++;

        if (entryIndex >= CurrentSchemeType.PossibleEntries.Count)
        {
            entryIndex = 0;
        }

        CurrentEntry = CurrentSchemeType.PossibleEntries[entryIndex];

        RefreshUI();
    }

    public void ShowPreviousEntry()
    {
        int entryIndex = CurrentSchemeType.PossibleEntries.IndexOf(CurrentEntry);
        entryIndex--;

        if (entryIndex < 0)
        {
            entryIndex = CurrentSchemeType.PossibleEntries.Count - 1;
        }

        CurrentEntry = CurrentSchemeType.PossibleEntries[entryIndex];

        RefreshUI();
    }

}
