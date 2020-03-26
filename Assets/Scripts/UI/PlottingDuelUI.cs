
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlottingDuelUI : MonoBehaviour
{
    public static PlottingDuelUI Instance;



    public TextMeshProUGUI EntryTitle;
    public Image EntryImage;

    public TextMeshProUGUI MethodTitle;
    public Image MethodImage;

    public LocationPortraitUI LocationPortrait;

    public Transform ParticipantsContainer;
    public Transform TargetsContainer;

    public Transform ParticipantDuelTransform;
    public Transform TargetDuelTransform;

    public Transform CombatEffectPoint;

    public Animator Anim;

    public Image ParticipantSkillIcon;
    public TextMeshProUGUI ParticipantSkillNumber;

    public Image TargetSkillIcon;
    public TextMeshProUGUI TargetSkillNumber;

    public TextMeshProUGUI PlotName;

    public GameObject ProcEventPanel;
    public TextMeshProUGUI ProcEventTitle;
    public Image ProcEventImage;

    public List<PortraitUI> ParticipantsPortraits = new List<PortraitUI>();
    public List<PortraitUI> TargetsPortraits = new List<PortraitUI>();

    List<Transform> ParticipantsPositionTransforms = new List<Transform>();
    List<Transform> TargetsPositionTransforms = new List<Transform>();

    PlotEntry CurrentEntry;
    PlotMethod CurrentMethod;
    public Character LastDefeatedParticipant;
    public Character LastDefeatedTarget;

    public System.Action<DuelResultData> OnComplete;

    public PlotData CurrentPlot;

    public bool SpeedMode = false;

    public bool IsPlayerAttacker
    {
        get
        {
            return CurrentPlot.Plotter.TopEmployer == CORE.PC;
        }
    }

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void LateUpdate()
    {
        RefreshPortraitPositions();
    }

    private void OnDisable()
    {
        if (AudioControl.Instance != null)
        {
            AudioControl.Instance.StopSound("soundscape_combat");
        }

        if (MouseLook.Instance == null) return;

        MouseLook.Instance.CurrentWindow = null;
    }

    public void Show(
        PlotData plot,
        LocationEntity location,
        System.Action<DuelResultData> onComplete)
    {
        SpeedMode = false;

        MouseLook.Instance.CurrentWindow = this.gameObject;

        this.gameObject.SetActive(true);

        AudioControl.Instance.Play("soundscape_combat", true);

        CurrentPlot = plot;
        this.OnComplete = onComplete;

        CurrentEntry = plot.Entry;
        CurrentMethod = plot.Method;

        PlotName.text = CurrentPlot.Name;

        EntryImage.sprite = plot.Entry.Icon;
        EntryTitle.text = plot.Entry.name;
        LocationPortrait.SetLocation(location);

        MethodImage.sprite = plot.Method.Icon;
        MethodTitle.text = plot.Method.name;

        plot.Participants.ForEach((x) => { GenerateParticipant(x); });
        plot.TargetParticipants.ForEach     ((x) => { GenerateTarget(x); });

        ParticipantsPortraits.OrderByDescending(x => x.CurrentCharacter.Rank).ToList();
        TargetsPortraits.OrderByDescending(x => x.CurrentCharacter.Rank).ToList();

        RefreshPositionTransforms();

        StopAllCoroutines();

        StartCoroutine(DuelsRoutine());
    }
    
    IEnumerator DuelsRoutine()
    {

        if (!SpeedMode)
        {
            yield return new WaitForSeconds(1f);
        }

        yield return StartCoroutine(InvokeStage("StartMatch"));

        while (ParticipantsPortraits.Count > 0 && TargetsPortraits.Count > 0)
        {
            PortraitUI Participant = ParticipantsPortraits[0];
            ParticipantsPortraits.RemoveAt(0);

            PortraitUI Target = TargetsPortraits[0];
            TargetsPortraits.RemoveAt(0);



            yield return StartCoroutine(SpecificDuelRoutine(Participant, Target));

            if (ParticipantsPortraits.Count == 0)
            {
                yield return StartCoroutine(InvokeStage("MatchFailed"));
            }
            else if (TargetsPortraits.Count == 0)
            {
                yield return StartCoroutine(InvokeStage("MatchWon"));
            }
        }

        yield return StartCoroutine(InvokeStage("EndMatch"));

        ExecuteDuelResult();
    }

    public void ExecuteDuelResult()
    {
        FailReason reason = null;

        if (ParticipantsPortraits.Count == 0)
        {
            reason = new FailReason("Lost Duels");
        }

        List<Character> participants = new List<Character>();
        List<Character> targets = new List<Character>();

        if (CurrentMethod == CurrentPlot.BaseMethod)//Ended Up Brutally
        {
            ParticipantsPortraits.ForEach((x) => { participants.Add(x.CurrentCharacter); });
            TargetsPortraits.ForEach((x) => { targets.Add(x.CurrentCharacter); });
        }
        else
        {
            participants = CurrentPlot.Participants;
            targets = CurrentPlot.TargetParticipants;
        }

        DuelResultData result = new DuelResultData(CurrentPlot, participants, targets, reason);
        OnComplete(result);
        
        Hide();
    }

    IEnumerator SpecificDuelRoutine(PortraitUI Participant, PortraitUI Target)
    {
        //Move duelists to positions

        float randomHeight = Random.Range(-1f,1f);
        Vector3 initParticipantPos = Participant.transform.position;
        Vector3 initTargetPos = Target.transform.position;

        float t = 0f;
        while(t<1f)
        {
            if (!SpeedMode)
            {
                t += 1f * Time.deltaTime;
            }
            else
            {
                t += 2f * Time.deltaTime;
            }

            Participant.transform.position = Util.SplineLerpX(initParticipantPos, ParticipantDuelTransform.position, randomHeight, t);
            Target.transform.position = Util.SplineLerpX(initTargetPos, TargetDuelTransform.position, randomHeight, t);

            RefreshPortraitPositions();

            yield return 0;
        }

        if (!SpeedMode)
        {
            yield return new WaitForSeconds(1f);
        }
        //Show Duel;

        float offenseSkill = Participant.CurrentCharacter.GetBonus(CurrentMethod.OffenseSkill).Value;
        if (CurrentEntry.Skill == CurrentMethod.OffenseSkill)
        {
            offenseSkill += CurrentEntry.BonusToSkill;
        }

        float defenceSkill = Target.CurrentCharacter.GetBonus(CurrentMethod.DefenceSkill).Value;

        ParticipantSkillIcon.sprite = CurrentMethod.OffenseSkill.icon;
        ParticipantSkillNumber.text = offenseSkill.ToString();
        TargetSkillIcon.sprite = CurrentMethod.DefenceSkill.icon;
        TargetSkillNumber.text = defenceSkill.ToString();

        //Show COmbat Effect
        if(CurrentMethod.MethodCombatEffect != null)
        {
            GameObject effect = ResourcesLoader.Instance.GetRecycledObject(CurrentMethod.MethodCombatEffect);
            effect.transform.SetParent(CombatEffectPoint);
            effect.transform.position = CombatEffectPoint.position;
            effect.transform.localScale = Vector3.one;
        }


        if (Random.Range(0, offenseSkill + defenceSkill) < offenseSkill) //Attackers Win
        {
            Anim.SetTrigger("WinParticipant");

            if (!SpeedMode)
            {
                yield return new WaitForSeconds(3f);
            }

            yield return StartCoroutine(KillCharacterRoutine(Target));

            yield return StartCoroutine(RetrieveCharacterRoutine(Participant));

            if (Participant.CurrentCharacter.TopEmployer == CORE.PC)
            {
                yield return StartCoroutine(InvokeStage("DuelWon"));
            }
            else
            {
                yield return StartCoroutine(InvokeStage("DuelFailed"));
            }
        }
        else //Defenders Win
        {
            Anim.SetTrigger("WinTarget");

            if (!SpeedMode)
            {
                yield return new WaitForSeconds(3f);
            }

            if (CurrentMethod != CurrentPlot.BaseMethod) // BRUTE SWITCH
            {
                ChangeMethod(CurrentPlot.BaseMethod);

                ParticipantsPortraits.Add(Participant);
                TargetsPortraits.Add(Target);

                yield return StartCoroutine(InvokeStage("MethodFailure"));
            }
            else //Normal State
            {
                yield return StartCoroutine(KillCharacterRoutine(Participant));

                yield return StartCoroutine(RetrieveCharacterRoutine(Target));
            }

            if (Target.CurrentCharacter.TopEmployer == CORE.PC)
            {
                yield return StartCoroutine(InvokeStage("DuelWon"));
            }
            else
            {
                yield return StartCoroutine(InvokeStage("DuelFailed"));
            }
        }

        if (!SpeedMode)
        {
            yield return new WaitForSeconds(2f);
        }

        yield return 0;
    }

    void RefreshPortraitPositions()
    {
        for (int i = 0; i < ParticipantsPortraits.Count; i++)
        {
            if(ParticipantsContainer.childCount <= i)
            {
                break;
            }

            ParticipantsPortraits[i].transform.position =
                Vector2.Lerp(ParticipantsPortraits[i].transform.position, ParticipantsContainer.GetChild(i).position, Time.deltaTime * 3f);
        }

        for (int i = 0; i < TargetsPortraits.Count; i++)
        {
            if (TargetsContainer.childCount <= i)
            {
                break;
            }

            TargetsPortraits[i].transform.position =
                Vector2.Lerp(TargetsPortraits[i].transform.position, TargetsContainer.GetChild(i).position, Time.deltaTime * 3f);
        }
    }

    public void Hide()
    {
        StopAllCoroutines();
        ClearGenerated();
        this.gameObject.SetActive(false);
    }

    void RefreshPositionTransforms()
    {
        ClearPositionTransforms();

        foreach(PortraitUI participant in ParticipantsPortraits)
        {
            GameObject posObj = ResourcesLoader.Instance.GetRecycledObject("PortraitPosition");
            posObj.transform.SetParent(ParticipantsContainer, false);
            posObj.transform.localScale = Vector3.one;
        }

        foreach (PortraitUI target in TargetsPortraits)
        {
            GameObject posObj = ResourcesLoader.Instance.GetRecycledObject("PortraitPosition");
            posObj.transform.SetParent(TargetsContainer, false);
            posObj.transform.localScale = Vector3.one;
        }
    }

    void ClearGenerated()
    {
        while(ParticipantsPortraits.Count > 0)
        {
            ParticipantsPortraits[0].transform.SetParent(CORE.Instance.DisposableContainer, false);
            ParticipantsPortraits[0].gameObject.SetActive(false);
            ParticipantsPortraits.RemoveAt(0);
        }

        while (TargetsPortraits.Count > 0)
        {
            TargetsPortraits[0].transform.SetParent(CORE.Instance.DisposableContainer, false);
            TargetsPortraits[0].gameObject.SetActive(false);
            TargetsPortraits.RemoveAt(0);
        }

        ClearPositionTransforms();
    }

    void ClearPositionTransforms()
    {
        while (ParticipantsPositionTransforms.Count > 0)
        {
            ParticipantsPositionTransforms[0].transform.SetParent(CORE.Instance.DisposableContainer, false);
            ParticipantsPositionTransforms[0].gameObject.SetActive(false);
            ParticipantsPositionTransforms.RemoveAt(0);
        }

        while (TargetsPositionTransforms.Count > 0)
        {
            TargetsPositionTransforms[0].transform.SetParent(CORE.Instance.DisposableContainer, false);
            TargetsPositionTransforms[0].gameObject.SetActive(false);
            TargetsPositionTransforms.RemoveAt(0);
        }
    }

    public void StartSpeedMode()
    {
        if(SpeedMode)
        {
            return;
        }

        SpeedMode = true;
    }

    public PortraitUI GenerateParticipant(Character character)
    {
        character.Known.Know("Appearance", CurrentPlot.TargetParticipants[0].TopEmployer);
        character.Known.Know("Appearance", CurrentPlot.Participants[0].TopEmployer);

        GameObject portraitObj = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
        portraitObj.transform.SetParent(transform);
        portraitObj.transform.localScale = new Vector3(-1, 1, 1);
        PortraitUI portrait = portraitObj.GetComponent<PortraitUI>();
        portrait.SetCharacter(character);
        ParticipantsPortraits.Add(portrait);

        return portrait;
    }

    public PortraitUI GenerateTarget(Character character)
    {
        character.Known.Know("Appearance", CurrentPlot.TargetParticipants[0].TopEmployer);
        character.Known.Know("Appearance", CurrentPlot.Participants[0].TopEmployer);

        GameObject portraitObj = ResourcesLoader.Instance.GetRecycledObject("PortraitUI");
        portraitObj.transform.SetParent(transform);
        portraitObj.transform.localScale = Vector3.one;
        PortraitUI portrait = portraitObj.GetComponent<PortraitUI>();
        portrait.SetCharacter(character);
        TargetsPortraits.Add(portrait);

        return portrait;
    }

    public void RetrieveCharacter(Character character)
    {
        PortraitUI portrait = TargetsPortraits.Find(x => x.CurrentCharacter == character);

        if (portrait == null)
        {
            portrait = ParticipantsPortraits.Find(x => x.CurrentCharacter == character);
        }

        if (portrait == null)
        {
            return;
        }

        StartCoroutine(RetrieveCharacterRoutine(portrait));
    }

    IEnumerator RetrieveCharacterRoutine(PortraitUI portrait)
    {
        if (CurrentPlot.Participants.Contains(portrait.CurrentCharacter))
        {
            ParticipantsPortraits.Add(portrait);
        }
        else if (CurrentPlot.TargetParticipants.Contains(portrait.CurrentCharacter))
        {
            TargetsPortraits.Add(portrait);
        }

        yield return 0;

    }

    public void KillCharacter(Character character)
    {
        PortraitUI portrait = TargetsPortraits.Find(x => x.CurrentCharacter == character);

        if (portrait == null)
        {
            portrait = ParticipantsPortraits.Find(x => x.CurrentCharacter == character);
            
            if (portrait == null)
            {
                return;
            }

            ParticipantsPortraits.Remove(portrait);
        }
        else
        {
            TargetsPortraits.Remove(portrait);
        }


        StartCoroutine(KillCharacterRoutine(portrait));
    }

    IEnumerator KillCharacterRoutine(PortraitUI portrait)
    {
        if (CurrentPlot.Participants.Contains(portrait.CurrentCharacter))
        {
            LastDefeatedParticipant = portrait.CurrentCharacter;
        }
        else if (CurrentPlot.TargetParticipants.Contains(portrait.CurrentCharacter))
        {
            LastDefeatedTarget = portrait.CurrentCharacter;
        }

        float t = 0f;
        while (t < 1f)
        {
            if (!SpeedMode)
            {
                t += 1f * Time.deltaTime;
            }
            else
            {
                t += 2f * Time.deltaTime;
            }

            portrait.transform.localScale = Vector3.Lerp(portrait.transform.localScale, Vector3.zero, t);


            yield return 0;
        }
        
        portrait.transform.localScale = Vector3.one;
        portrait.gameObject.SetActive(false);
    }
    
    public IEnumerator InvokeStage(string stageKey)
    {
        List<DuelProc> duelProcs = CORE.Instance.Database.DuelProcs.FindAll(x => x.Type.name == stageKey);

        if(duelProcs != null)
        {
            foreach(DuelProc duelProc in duelProcs)
            {
                if(CurrentPlot.ProcsUsed.Contains(duelProc))
                {
                    continue;
                }

                yield return StartCoroutine(duelProc.Execute());
            }
        }
    }

    public IEnumerator SetProcEvent(DuelProc proc)
    {
        if (!proc.Repeatable)
        {
            CurrentPlot.ProcsUsed.Add(proc);
        }

        ProcEventPanel.gameObject.SetActive(true); /// IS NULL?!?!?!? WHY!??!?
        ProcEventTitle.text = proc.name;
        ProcEventImage.sprite = proc.Icon;

        yield return new WaitForSeconds(2f);

        ProcEventPanel.gameObject.SetActive(false);
    }

    public void ChangeMethod(PlotMethod method)
    {
        CurrentMethod = method;
        MethodImage.sprite = CurrentMethod.Icon;
        MethodTitle.text = CurrentMethod.name;
    }
}