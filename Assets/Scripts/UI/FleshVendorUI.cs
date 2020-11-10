using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FleshVendorUI : MonoBehaviour
{
    public static FleshVendorUI Instance;

    [SerializeField]
    PortraitUI SelectedCharacter;

    [SerializeField]
    TextMeshProUGUI GoldWorth;

    [SerializeField]
    TextMeshProUGUI ConnectionsWorth;

    [SerializeField]
    TextMeshProUGUI ProgressionWorth;

    public int CharacterValueMultiplier = 2;
    Button SacrificeButton;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (AudioControl.Instance != null)
        {
            AudioControl.Instance.StopSound("soundscape_combat");
            AudioControl.Instance.UnmuteMusic();
        }

        if (MouseLook.Instance != null)
        {
            MouseLook.Instance.CurrentWindow = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Show(Character character = null)
    {
        MouseLook.Instance.CurrentWindow = this.gameObject;

        AudioControl.Instance.Play("soundscape_combat", true);
        AudioControl.Instance.MuteMusic();

        if(character  != null)
        {
            SelectedCharacter.SetCharacter(character);
        }
        else
        {
            SelectedCharacter.SetCharacter(null);
        }

        this.gameObject.SetActive(true);
        RefreshUI();
    }

    public void RefreshUI()
    {
        if(SelectedCharacter == null)
        {
            GoldWorth.text = "0";
            ConnectionsWorth.text = "0";
            ProgressionWorth.text = "0";
            return;
        }

        int charWorth = SelectedCharacter.CurrentCharacter.TotalBonusScore;

        GoldWorth.text = ""+charWorth * 5 * CharacterValueMultiplier;
        ConnectionsWorth.text = "" + charWorth * 2 * CharacterValueMultiplier;
        ProgressionWorth.text = "" + charWorth * CharacterValueMultiplier;
    }

    public void SelectCharacter()
    {
        SelectCharacterViewUI.Instance.Show((Character selected) =>
        {
            this.SelectedCharacter.SetCharacter(selected);
            RefreshUI();
        },
            x =>
            x.PrisonLocation != null
        && x.PrisonLocation.OwnerCharacter != null
        && x.PrisonLocation.OwnerCharacter.TopEmployer == CORE.PC,
            "Select Victim");
    }

    public void GoldVictim()
    {
        if(SelectedCharacter.CurrentCharacter.UnsellablePrisoner)
        {
            WarningWindowUI.Instance.Show("Nobody is interested in this prisoner (For some strange reason...).", null);
            return;
        }

        int charWorth = SelectedCharacter.CurrentCharacter.TotalBonusScore;

        PopupData popData = new PopupData(CORE.Instance.Database.GetPopupPreset("The Malechite Road Complete Popup"), new List<Character> { SelectedCharacter.CurrentCharacter }, new List<Character> { }, () =>
        {
            CORE.PC.CGold += charWorth * 5 * CharacterValueMultiplier;
            SelectedCharacter.CurrentCharacter.ExitPrison();
            SelectedCharacter.CurrentCharacter.UnsellablePrisoner = true;
            SelectedCharacter.CurrentCharacter.Death(true, false);
        });

        PopupWindowUI.Instance.AddPopup(popData);

        this.gameObject.SetActive(false);


    }


    public void ConnectionsVictim()
    {
        if (SelectedCharacter.CurrentCharacter.UnsellablePrisoner)
        {
            WarningWindowUI.Instance.Show("Nobody is interested in this prisoner (For some strange reason...).", null);
            return;
        }

        int charWorth = SelectedCharacter.CurrentCharacter.TotalBonusScore;

        PopupData popData = new PopupData(CORE.Instance.Database.GetPopupPreset("The Nobility Client Complete Popup"), new List<Character> { SelectedCharacter.CurrentCharacter }, new List<Character> { },
            () =>
        {
            CORE.PC.CConnections += charWorth * 2 * CharacterValueMultiplier;
            SelectedCharacter.CurrentCharacter.ExitPrison();
            SelectedCharacter.CurrentCharacter.UnsellablePrisoner = true;
            SelectedCharacter.CurrentCharacter.Death(true, false);
        });

        PopupWindowUI.Instance.AddPopup(popData);


        this.gameObject.SetActive(false);
    }

    public void ProgressionVictim()
    {
        if (SelectedCharacter.CurrentCharacter.UnsellablePrisoner)
        {
            WarningWindowUI.Instance.Show("Nobody is interested in this prisoner (For some strange reason...).", null);
            return;
        }

        int charWorth = SelectedCharacter.CurrentCharacter.TotalBonusScore;

        PopupData popData = new PopupData(CORE.Instance.Database.GetPopupPreset("Flesh Ceremony Complete Popup"), new List<Character> { SelectedCharacter.CurrentCharacter }, new List<Character> { },
            () =>
            {
                CORE.PC.CProgress += charWorth * CharacterValueMultiplier;
                SelectedCharacter.CurrentCharacter.ExitPrison();
                SelectedCharacter.CurrentCharacter.UnsellablePrisoner = true;
                SelectedCharacter.CurrentCharacter.Death(true, false);
            });

        PopupWindowUI.Instance.AddPopup(popData);

        this.gameObject.SetActive(false);
    }
}
