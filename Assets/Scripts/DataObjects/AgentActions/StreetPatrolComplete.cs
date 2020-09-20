using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StreetPatrolComplete", menuName = "DataObjects/AgentActions/Work/StreetPatrolComplete", order = 2)]
public class StreetPatrolComplete : AgentAction
{

    public int DuelBonus;

    [SerializeField]
    public GameObject PortraitEffectWanted;

    public override void Execute(Character requester, Character character, AgentInteractable target)
    {
        base.Execute(requester, character, target);

        FailReason reason;
        if (!CanDoAction(requester, character, target, out reason))
        {
            return;
        }

        if (!RollSucceed(character))
        {
            if (FailureResult != null)
            {
                FailureResult.Execute(requester, character, target);
            }

            return;
        }

        LocationEntity location = (LocationEntity)target;

        if(character.TopEmployer == CORE.PC)//Patrol events.
        {
            
            if(Random.Range(0f,1f) < 0.08f)
            {
                PopupDataPreset preset = CORE.Instance.Database.GetPopupPreset("Constabulary Patrol Scenario");

                Dictionary<string, string> POPparameters = new Dictionary<string, string>();
                POPparameters.Add("ActorName", character.name);

                PopupData popupData = new PopupData(preset, new List<Character> { character }, new List<Character> { CORE.PC }, () =>
                {
                    DialogPiece patrolDialog = CORE.Instance.Database.AllDialogPieces.Find(X => X.name == "PATROL - Known Criminal");

                    if (MapViewManager.Instance != null && MouseLook.Instance != null && MouseLook.Instance.isAbleToLookaround)
                    {
                        MapViewManager.Instance.ForceInteractWithMap();
                    }

                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters.Add("ActorName", character.name);
                    parameters.Add("Actor", character);
                    parameters.Add("Location", character.CurrentLocation);
                    DialogWindowUI.Instance.StartNewDialog(patrolDialog, parameters);
                },POPparameters);

                PopupWindowUI.Instance.AddPopup(popupData);

                return;
            }
            else if(Random.Range(0f,1f) < 0.1f)
            {
                int revenue = 5;

                Sprite actionIcon = null;
                if (Random.Range(0, 2) == 0)
                {
                    actionIcon = ResourcesLoader.Instance.GetSprite("pay_money");
                    CORE.Instance.ShowHoverMessage("<color=yellow>Collected a fine +" + 5 + " Gold.</color>", actionIcon, character.CurrentLocation.transform);
                    TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("Collected a fine...", actionIcon, character));
                    CORE.PC.CGold += revenue;
                }
                else
                {
                    actionIcon = ResourcesLoader.Instance.GetSprite("earicon");
                    CORE.Instance.ShowHoverMessage("<color=purple>Collected Criminal Testimony - " + 5 + " Rumors.</color>", actionIcon, character.CurrentLocation.transform);
                    TurnReportUI.Instance.Log.Add(new TurnReportLogItemInstance("Collected Testimony", actionIcon, character));
                    CORE.PC.CRumors += revenue;
                }
            }
        }

        float targetDiscreet;
        float heroAware;

        //Hunt for wanteds
        List<Character> wantedChars = location.CharactersInLocation.FindAll(
            (Character charInQuestion) =>
            {
                return charInQuestion.PrisonLocation == null
                && !charInQuestion.IsDead
                && charInQuestion.Traits.Find(x => x.name == "Wanted") != null
                && (charInQuestion.CurrentTaskEntity == null || charInQuestion.CurrentTaskEntity.CurrentTask.name != "In Hiding")
                && !character.TopEmployer.IsPuppetOf(charInQuestion.CurrentFaction);
            });

        if(wantedChars != null && wantedChars.Count > 0)
        {

            foreach(Character wantedChar in wantedChars)
            {
                if (wantedChar.TopEmployer != CORE.PC
                       && Random.Range(0f, 1f) < 0.5f)
                {
                    continue;
                }
                
                if(wantedChar.TopEmployer == CORE.PC 
                    && CORE.Instance.TechTree.Find(x=>x.name == "Deceptive Measures").IsResearched
                    && Random.Range(0f, 1f) < 0.3f)
                {
                    continue;
                }

                targetDiscreet = wantedChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;
                heroAware = wantedChar.GetBonus(CORE.Instance.Database.GetBonusType("Aware")).Value;
                if (Random.Range(0, targetDiscreet + heroAware + DuelBonus) < heroAware + DuelBonus)
                {
                    wantedChar.StopDoingCurrentTask();
                    CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, wantedChar, target);
                    continue;
                }
            }
        }

        //Hunt for illegal activity
        List<Character> illegalTaskChars = location.CharactersInLocation.FindAll(
            (Character charInQuestion) => 
            {
                return charInQuestion.CurrentTaskEntity != null 
                && charInQuestion.CurrentTaskEntity.CurrentTask.Illegal
                && !character.TopEmployer.IsPuppetOf(charInQuestion.CurrentFaction);
            });

        if(illegalTaskChars == null || illegalTaskChars.Count == 0)
        {
            return;
        }

        
        foreach(Character targetChar in illegalTaskChars)
        {
            if (targetChar.Traits.Find(x => x.name == "Wanted"))
            {
                continue;
            }

            if (targetChar.TopEmployer != CORE.PC
                && Random.Range(0f, 1f) < 0.5f)
            {
                continue;
            }

            if (targetChar.TopEmployer == CORE.PC
                    && CORE.Instance.TechTree.Find(x => x.name == "Deceptive Measures").IsResearched
                    && Random.Range(0f, 1f) < 0.3f)
            {
                continue;
            }

            targetDiscreet = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Discreet")).Value;
            heroAware = targetChar.GetBonus(CORE.Instance.Database.GetBonusType("Aware")).Value;
            if (Random.Range(0,targetDiscreet+heroAware+DuelBonus) < heroAware+DuelBonus)
            {
                targetChar.AddTrait(CORE.Instance.Database.Traits.Find(x => x.name == "Wanted"));
                CORE.Instance.ShowPortraitEffect(PortraitEffectWanted, targetChar, targetChar.CurrentLocation);

                continue;
            }
        }

        //Hunt for corpses
        if (character.CurrentLocation.CorpsesBuried > 0)
        {
            for (int i = 0; i < character.CurrentLocation.CorpsesBuried; i++)
            {
                if(Random.Range(0f,1f) < 0.05f)
                {
                    if(character.CurrentLocation.OwnerCharacter != null && character.CurrentLocation.OwnerCharacter.PrisonLocation == null)
                    {

                        List<Character> pplInvolved = new List<Character>();

                        pplInvolved.Add(character.CurrentLocation.OwnerCharacter);

                        pplInvolved.AddRange(character.CurrentLocation.EmployeesCharacters.FindAll(x => x.CurrentLocation == character.CurrentLocation));
                        pplInvolved.AddRange(character.CurrentLocation.GuardsCharacters.FindAll(x => x.CurrentLocation == character.CurrentLocation));

                        foreach (Character involved in pplInvolved)
                        {
                            CORE.Instance.Database.GetAgentAction("Get Arrested").Execute(CORE.Instance.Database.GOD, involved, target);
                        }
                    }

                    character.CurrentLocation.CorpsesBuried--;
                    break;
                }
            }
        }
    }

    public override bool CanDoAction(Character requester, Character character, AgentInteractable target, out FailReason reason)
    {
        if (!base.CanDoAction(requester, character, target, out reason))
        {
            return false;
        }

        if (requester != CORE.Instance.Database.GOD && character.TopEmployer != requester && requester != character)
        {
            return false;
        }

        return true;
    }
}
