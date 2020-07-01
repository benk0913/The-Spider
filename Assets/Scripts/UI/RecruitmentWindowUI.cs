using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentWindowUI : MonoBehaviour
{
    public static RecruitmentWindowUI Instance;

    [SerializeField]
    public Transform RecruitmentPoolsContainer;

    [SerializeField]
    public Transform CharactersContainer;

    [SerializeField]
    public GameObject NoOneToRecruitLabel;

    public TextMeshProUGUI RecruitmentLabelTitle;

    public RecruitmentPool CurrentPool;

    Action<Character> OnRecruit;

    public int AvailablePools
    {
        get
        {
            int count = 0;

            foreach (RecruitmentPool pool in CORE.Instance.RecruitmentPools)
            {
                if (CORE.PlayerFaction.name != pool.RequiresFaction.name)
                {
                    continue;
                }

                if (pool.RequiresTech != null && !CORE.Instance.TechTree.Find(x => x.name == pool.RequiresTech.name).IsResearched)
                {
                    continue;
                }

                count++;
            }

            return count;
        }
    }
    private void Awake()
    {
        Instance = this;
        HideWindow();
    }

    private void OnDisable()
    {
        if (AudioControl.Instance != null)
        {
            AudioControl.Instance.StopSound("soundscape_research_character");
            AudioControl.Instance.UnmuteMusic();
        }

        if (MouseLook.Instance == null) return;

        MouseLook.Instance.CurrentWindow = null;
    }

    public void Show(LocationEntity location, Character requester, int minAge = 6, int maxAge = 70, int gender = -1, LocationEntity spawn = null, Action<Character> onRecruit = null)
    {
        this.OnRecruit = onRecruit;

        this.gameObject.SetActive(true);

        MouseLook.Instance.CurrentWindow = this.gameObject;

        AudioControl.Instance.Play("soundscape_research_character", true);
        AudioControl.Instance.MuteMusic();

        ClearPoolsContainers();

        RecruitmentPoolUI poolItem = ResourcesLoader.Instance.GetRecycledObject("RecruitmentPool").GetComponent<RecruitmentPoolUI>();
        poolItem.transform.SetParent(RecruitmentPoolsContainer, false);
        poolItem.SetInfo(null, () => 
        {
            if(requester.CConnections < 3)
            {
                GlobalMessagePrompterUI.Instance.Show("Not enough connections! ("+ requester.CConnections+"/"+3+")", 1f, Color.red);
                return;
            }

            OnRecruit?.Invoke(CORE.Instance.GenerateCharacter(gender,minAge,maxAge,spawn));
            HideWindow();

            requester.CConnections -= 3;
        });

        foreach (RecruitmentPool pool in CORE.Instance.RecruitmentPools)
        {
            if(CORE.PlayerFaction.name != pool.RequiresFaction.name)
            {
                continue;
            }

            if (pool.RequiresTech != null && !CORE.Instance.TechTree.Find(x=>x.name == pool.RequiresTech.name).IsResearched)
            {
                continue;
            }

            poolItem = ResourcesLoader.Instance.GetRecycledObject("RecruitmentPool").GetComponent<RecruitmentPoolUI>();
            poolItem.transform.SetParent(RecruitmentPoolsContainer, false);
            poolItem.SetInfo(pool, () => { ShowPool(pool, requester, minAge,maxAge, gender, spawn); });
        }



        ShowPool(CORE.Instance.Database.DefaultPool, requester);
    }



    void ClearPoolsContainers()
    {
        while (RecruitmentPoolsContainer.childCount > 0)
        {
            RecruitmentPoolsContainer.GetChild(0).gameObject.SetActive(false);
            RecruitmentPoolsContainer.GetChild(0).transform.SetParent(transform);
        }
    }

    void ClearCharactersContainer()
    { 
        while (CharactersContainer.childCount > 0)
        {
            CharactersContainer.GetChild(0).gameObject.SetActive(false);
            CharactersContainer.GetChild(0).transform.SetParent(transform);
        }
    }

    public void ShowPool(RecruitmentPool pool, Character requester, int minAge = 6, int maxAge = 70, int gender = -1, LocationEntity spawn = null)
    {

        RecruitmentLabelTitle.text = pool.name;

        ClearCharactersContainer();
        foreach (Character character in pool.Characters)
        {
            if(character.Age < minAge)
            {
                continue;
            }

            if (character.Age > maxAge)
            {
                continue;
            }

            if (gender != -1 && (int)character.Gender != gender)
            {
                continue;
            }

            character.Known.KnowEverythingAll();
            GameObject selectableCharacter = ResourcesLoader.Instance.GetRecycledObject("SelectablePortraitUI");
            selectableCharacter.transform.SetParent(CharactersContainer, false);
            selectableCharacter.transform.GetChild(0).GetComponent<PortraitUI>().SetCharacter(character);
            selectableCharacter.GetComponent<Button>().onClick.RemoveAllListeners();
            selectableCharacter.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (requester.CConnections < pool.Cost)
                {
                    GlobalMessagePrompterUI.Instance.Show("Not enough connections! (" + requester.CConnections + "/" + pool.Cost + ")", 1f, Color.red);
                    return;
                }

                //Todo add connections cost 
                OnRecruit?.Invoke(character);
                pool.Remove(character);
                HideWindow();

                requester.CConnections -= pool.Cost;

                if(spawn != null)
                {
                    character.GoToLocation(spawn);
                }
            });
        }

        NoOneToRecruitLabel.SetActive(CharactersContainer.childCount <= 0);
    }

    void HideWindow()
    {
        this.gameObject.SetActive(false);
    }

}
