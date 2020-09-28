using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HeatPanelUI : MonoBehaviour
{
    [SerializeField]
    Transform HeatContainer;

    [SerializeField]
    GameObject WarningEffect;

    [SerializeField]
    TextMeshProUGUI SuspicionLabel;

    [SerializeField]
    TooltipTargetUI TooltipTarget;

    [SerializeField]
    public List<string> SuspicionLabels;

    [SerializeField]
    public List<string> SuspicionTooltips;

    [SerializeField]
    public List<Trait> ManAtArmsTraits;

    [SerializeField]
    Sprite ManAtArmsPortrait;

    [SerializeField]
    public List<SchemeType> PotentialPlotSchemes = new List<SchemeType>();

    [SerializeField]
    CanvasGroup CG;

    private void Start()
    {
        CORE.Instance.SubscribeToEvent("HeatChanged", RefreshUI);
        CORE.Instance.SubscribeToEvent("PassTimeComplete", PassTurn);
        CORE.Instance.SubscribeToEvent("GameLoadComplete", ()=>CORE.Instance.DelayedInvokation(1f,RefreshUI));
    }

    public void PassTurn()
    {
        if(CORE.PC.CurrentFaction.HasPromotionSystem)
        {
            return;
        }

        if(CORE.PC.Heat > 0)
        {


            TechTreeItem rebels = CORE.Instance.TechTree.Find(X => X.name == "Rebels");

            if (rebels != null && rebels.IsResearched)
            {
            }
            else
            {
                TechTreeItem deceptive = CORE.Instance.TechTree.Find(X => X.name == "Deceptive Measures");
                if (deceptive != null && deceptive.IsResearched)
                {
                    if (Random.Range(0f, 1f) < 0.3f)
                    {
                        CORE.PC.Heat--;
                    }
                }
                else
                {
                    if (Random.Range(0f, 1f) < 0.1f)
                    {
                        CORE.PC.Heat--;
                    }
                }
            }
        }

        RefreshUI();

        if(CORE.PC.Heat == 2)
        {
            if(Random.Range(0f,1f) < 0.2f)
            {
                List<Character> Characters = CORE.PC.CharactersInCommand;

                Characters.RemoveAll(x => x.PrisonLocation != null && x.IsDisabled);

                Character character = Characters[Random.Range(0, Characters.Count)];
                CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, character, character.CurrentLocation);
            }
        }
        else if (CORE.PC.Heat == 3)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                List<Character> Characters = CORE.PC.CharactersInCommand;

                Characters.RemoveAll(x => x.PrisonLocation != null && x.IsDisabled);

                Character character = Characters[Random.Range(0, Characters.Count)];
                CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, character, character.CurrentLocation);
            }
        }
        else if (CORE.PC.Heat == 4)
        {
            List<Character> Characters = CORE.PC.CharactersInCommand;

            Characters.RemoveAll(x => x.PrisonLocation != null && x.IsDisabled);

            Character character = Characters[Random.Range(0, Characters.Count)];
            CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, character, character.CurrentLocation);   
        }
        else if (CORE.PC.Heat == 5)
        {
            List<Character> Characters = CORE.PC.CharactersInCommand;

            Characters.RemoveAll(x => x.PrisonLocation != null && x.IsDisabled);

            Character character = Characters[Random.Range(0, Characters.Count)];
            CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, character, character.CurrentLocation);

            AttemptPlot(PotentialPlotSchemes[Random.Range(0, PotentialPlotSchemes.Count)], CORE.PC.CurrentFaction);
        }
    }

    public virtual FailReason AttemptPlot(SchemeType currentSchemeType, Faction againstFaction)
    {
        //Find Target
        if (againstFaction.FactionHead == null)
        {
            return null;
        }

        AgentInteractable plotTarget = null;

        Character factionHead = CORE.Instance.Characters.Find(x => x.name == againstFaction.FactionHead.name);

        if (factionHead == null || factionHead.IsDead)
        {
            return null;
        }

        if (currentSchemeType.TargetIsLocation)
        {
            List<LocationEntity> possibleLocationTargets = factionHead.PropertiesInCommand.FindAll(x => !x.IsRuined);
            if (possibleLocationTargets.Count == 0)
            {
                return null;
            }

            possibleLocationTargets = possibleLocationTargets.OrderBy(x => x.GuardsCharacters.Count).ToList();
            plotTarget = possibleLocationTargets[0];
        }
        else
        {
            List<Character> possibleCharacters = factionHead.CharactersInCommand.FindAll(x => !x.IsDead && !x.IsInTrouble && x.PrisonLocation == null && !x.IsInHiding);
            if (possibleCharacters.Count == 0)
            {
                return null;
            }

            possibleCharacters = possibleCharacters.OrderBy(x => x.Rank).ToList();

            PortraitUI portrait = ResourcesLoader.Instance.GetRecycledObject("PortraitUI").GetComponent<PortraitUI>();
            portrait.transform.position = new Vector3(9999, 9999, 9999);
            portrait.SetCharacter(possibleCharacters[0]);
            plotTarget = portrait;
        }



        Character plotter = GenerateManAtArms();

        //Find Entry
        List<PlotEntry> possibleEntries = new List<PlotEntry>();
        possibleEntries.AddRange(currentSchemeType.PossibleEntries);

        PlotEntry randomEntry = possibleEntries[UnityEngine.Random.Range(0, possibleEntries.Count)];

        //Find Method
        PlotMethod randomMethod = currentSchemeType.PossibleMethods[UnityEngine.Random.Range(0, currentSchemeType.PossibleMethods.Count)];


        //Gather Participants
        List<Character> participants = new List<Character>();

        for (int i = 0; i < randomMethod.MaximumParticipants; i++)
        {
            participants.Add(GenerateManAtArms());
        }


        //Gather target participants
        List<Character> targetParticipants = new List<Character>();
        if (plotTarget.GetType() == typeof(PortraitUI) || plotTarget.GetType() == typeof(PortraitUIEmployee))
        {
            Character targetCharacter = ((PortraitUI)plotTarget).CurrentCharacter;

            if (targetCharacter.PrisonLocation == null && targetCharacter.Age >= 15 && !targetCharacter.IsInTrouble)
            {
                targetParticipants.Add(targetCharacter);
            }

            targetParticipants.AddRange(targetCharacter.GuardsInCommand.FindAll(x =>
           x.PrisonLocation == null
           && !x.IsInTrouble));
        }
        else if (plotTarget.GetType() == typeof(LocationEntity))
        {
            LocationEntity location = ((LocationEntity)plotTarget);

            if (location.OwnerCharacter != null)
            {
                targetParticipants.AddRange(CORE.Instance.Characters.FindAll(x =>
                    !targetParticipants.Contains(x)
                    && x.PrisonLocation == null
                    && x.CurrentLocation == location
                    && x.Age >= 15));
            }
        }



        PlotData Plot = new PlotData(currentSchemeType.name, CORE.Instance.Database.GOD, plotter, participants, targetParticipants, plotTarget, randomMethod, randomEntry);

        Plot.BaseMethod = currentSchemeType.BaseMethod;

        //if ((CurrentCharacter.Gold - 30) < GOLD_SCARCE_VALUE/2)
        //{
        //    return new FailReason("Not Enough Gold");
        //}

        //CurrentCharacter.Gold -= 30;

        currentSchemeType.Execute(Plot);


        if (plotTarget.GetType() == typeof(PortraitUI) || plotTarget.GetType() == typeof(PortraitUIEmployee))
        {
            plotTarget.gameObject.SetActive(false);
        }

        return null;
    }

    public Character GenerateManAtArms()
    {
        Character temp = CORE.Instance.GenerateCharacter(-1, 15, 70);

        foreach (Trait trait in ManAtArmsTraits)
        {
            temp.AddTrait(trait);
        }

        temp.Known.KnowEverything(CORE.PC);

        temp.StartWorkingFor(CORE.Instance.Factions.Find(x => x.name == "Constabulary").FactionHead.PropertiesOwned[0]);

        temp.UniquePortrait = ManAtArmsPortrait;

        return temp;
    }


    public void RefreshUI()
    {

        if (CORE.PC.Heat > 0)
        {
            Display();
        }
        else
        {
            Hide();
        }

        for (int i=0;i<HeatContainer.childCount;i++)
        {
            GameObject pointObject = HeatContainer.GetChild(i).GetChild(1).gameObject;

            if (CORE.PC.Heat > i)
            {
                pointObject.SetActive(true);
            }
            else
            {
                pointObject.SetActive(false);
            }
        }

        SuspicionLabel.text = SuspicionLabels[CORE.PC.Heat];

        WarningEffect.gameObject.SetActive(CORE.PC.Heat > 2);

        TooltipTarget.Text = "<color=red>Heat:</color> How suspicious is your faction the eyes of the Constabulary.";
        TooltipTarget.Text += System.Environment.NewLine;

        TooltipTarget.Text += SuspicionTooltips[CORE.PC.Heat];
    }

    public void Display()
    {
        if (CORE.PC.CurrentFaction.HasPromotionSystem)
        {
            Hide();
            return;
        }

        CG.alpha = 1f;
        CG.interactable = true;
        CG.blocksRaycasts = true;

        TutorialScreenUI.Instance.Show("Heat");
    }

    public void Hide()
    {
        CG.alpha = 0f;
        CG.interactable = false;
        CG.blocksRaycasts = false;
    }
}
