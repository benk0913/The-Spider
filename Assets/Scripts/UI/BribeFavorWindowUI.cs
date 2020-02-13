using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BribeFavorWindowUI : MonoBehaviour
{
    public static BribeFavorWindowUI Instance;

    public Character CurrentCharacter;

    [SerializeField]
    Transform FavorDecisionsContainer;

    [SerializeField]
    PortraitUI CurrentCharacterPortrait;

    [SerializeField]
    PortraitUI PlayerCharacterPortrait;

    [SerializeField]
    TextMeshProUGUI BribeGoldText;

    [SerializeField]
    TextMeshProUGUI BribeRumorsText;

    [SerializeField]
    TextMeshProUGUI BribeConnectionsText;

    [SerializeField]
    TextMeshProUGUI FavorOwedText;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (MouseLook.Instance == null) return;

        MouseLook.Instance.CurrentWindow = null;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Show(Character ofCharacter)
    {
        if (ofCharacter.TopEmployer == CORE.PC)
        {
            GlobalMessagePrompterUI.Instance.Show("No reason to bribe your own minions!");
            return;
        }

        if (!ofCharacter.IsKnown("Name", CORE.PC))
        {
            GlobalMessagePrompterUI.Instance.Show("Must know this characters NAME");
            return;
        }

        if (!ofCharacter.IsKnown("HomeLocation", CORE.PC))
        {
            GlobalMessagePrompterUI.Instance.Show("Must know this characters 'Home Location'");
            return;
        }

        MouseLook.Instance.CurrentWindow = this.gameObject;

        CurrentCharacter = ofCharacter;
        this.gameObject.SetActive(true);

        RefreshUI();
    }

    void RefreshUI()
    {
        CurrentCharacterPortrait.SetCharacter(CurrentCharacter);
        PlayerCharacterPortrait.SetCharacter(CORE.PC);

        BribeGoldText.text = "Bribe \n<size=20>" + CurrentCharacter.FavorPointGoldPrice(CORE.PC) + "g</size>";
        BribeRumorsText.text = "Blackmail \n<size=20>" + CurrentCharacter.FavorPointRumorsPrice(CORE.PC) + "r</size>";
        BribeConnectionsText.text = "Leverage \n<size=20>" + CurrentCharacter.FavorPointConnectionsPrice(CORE.PC) + "c</size>";

        FavorOwedText.text = "Favor Owed \n" + CurrentCharacter.GetFavorPoints(CORE.PC);

        ClearContainer();

        foreach (FavorDecision favorDecision in CORE.Instance.Database.FavorDecisions)
        {
            GameObject itemObj = ResourcesLoader.Instance.GetRecycledObject("FavorDecisionUI");
            itemObj.transform.SetParent(FavorDecisionsContainer, false);
            itemObj.GetComponent<FavorDecisionUI>().SetInfo(favorDecision, CurrentCharacter);
        }
    }

    public void ClearContainer()
    {
        while (FavorDecisionsContainer.childCount > 0)
        {
            FavorDecisionsContainer.GetChild(0).gameObject.SetActive(false);
            FavorDecisionsContainer.GetChild(0).SetParent(transform);
        }
    }

    public void BuyFavorGold()
    {
        if(CORE.PC.Gold < CurrentCharacter.FavorPointGoldPrice(CORE.PC))
        {
            GlobalMessagePrompterUI.Instance.Show("Not Enough Gold (" + CORE.PC.Gold + "/" + CurrentCharacter.FavorPointGoldPrice(CORE.PC) + ")", 1f, Color.red);
            return;
        }

        CORE.PC.Gold -= CurrentCharacter.FavorPointGoldPrice(CORE.PC);
        CurrentCharacter.AddFavorPoints(CORE.PC, 1);

        RefreshUI();
    }

    public void BuyFavorRumors()
    {
        if (CORE.PC.Rumors < CurrentCharacter.FavorPointRumorsPrice(CORE.PC))
        {
            GlobalMessagePrompterUI.Instance.Show("Not Enough Rumors (" + CORE.PC.Rumors + "/" + CurrentCharacter.FavorPointRumorsPrice(CORE.PC) + ")", 1f, Color.red);
            return;
        }

        CORE.PC.Rumors -= CurrentCharacter.FavorPointRumorsPrice(CORE.PC);
        CurrentCharacter.AddFavorPoints(CORE.PC, 1);

        RefreshUI();
    }

    public void BuyFavorConnections()
    {
        if (CORE.PC.Connections < CurrentCharacter.FavorPointConnectionsPrice(CORE.PC))
        {
            GlobalMessagePrompterUI.Instance.Show("Not Enough Connections (" + CORE.PC.Connections + "/" + CurrentCharacter.FavorPointConnectionsPrice(CORE.PC) + ")", 1f, Color.red);
            return;
        }

        CORE.PC.Connections -= CurrentCharacter.FavorPointConnectionsPrice(CORE.PC);
        CurrentCharacter.AddFavorPoints(CORE.PC, 1);

        RefreshUI();
    }
}
