using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuestioningWindowUI : MonoBehaviour
{
    public static QuestioningWindowUI Instance;

    [SerializeField]
    TextMeshProUGUI Title;

    [SerializeField]
    TextMeshProUGUI CharacterDeckText;

    [SerializeField]
    TextMeshProUGUI TargetDeckText;

    [SerializeField]
    Transform OptionsContainer;

    [SerializeField]
    PortraitUI CharacterPortrait;

    [SerializeField]
    PortraitUI TargetPortrait;

    [SerializeField]
    public List<QuestioningItem> PotentialAnswers;

    [SerializeField]
    public List<QuestioningItem> PotentialQuestions;


    [SerializeField]
    public Transform TargetHandPosition;

    [SerializeField]
    public GameObject MatchEffectPanel;

    [SerializeField]
    TextMeshProUGUI MatchEffectTitle;

    [SerializeField]
    GameObject InputBlocker;
    


    public QuestioningInstance CurrentInstance;

    public Character CurrentCharacter;
    public Character CurrentTarget;

    public List<QuestioningItem> CharacterDeck = new List<QuestioningItem>();
    public List<QuestioningItem> TargetDeck = new List<QuestioningItem>();

    public List<QuestioningItemUI> Hand = new List<QuestioningItemUI>();
    public QuestioningItemUI TargetHand;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show(Character character, Character target)
    {
        this.gameObject.SetActive(true);

        InputBlocker.SetActive(true);

        ClearAll();

        CurrentInstance = target.CurrentQuestioningInstance;
        CurrentCharacter = character;
        CurrentTarget = target;

        ResetDecks();

        RefreshUI();
        
        for(int i=0;i<3;i++)
        {
            CORE.Instance.DelayedInvokation(1f * i, DrawCard);
        }

        TargetDrawCard();

        CORE.Instance.DelayedInvokation(3f, () => 
        {
            MatchEffectPanel.gameObject.SetActive(true);
            MatchEffectPanel.transform.SetAsLastSibling();
            MatchEffectTitle.text = "Questioning...";

            CORE.Instance.DelayedInvokation(2f, () => { InputBlocker.SetActive(false); });
        });
        
    }

    void ResetDecks()
    {
        CurrentCharacter.Bonuses.ForEach(x =>
        {
            QuestioningItem item = PotentialQuestions.Find(y => y.RelevantSkill == x.Type);

            if (item != null)
            {
                for (int i = 0; i < x.Value; i++)
                {
                    CharacterDeck.Add(item);
                }
            }
        });

        CharacterDeck = CharacterDeck.OrderBy(i => System.Guid.NewGuid()).ToList();


        CurrentTarget.Bonuses.ForEach(x =>
        {
            QuestioningItem item = PotentialAnswers.Find(y => y.RelevantSkill == x.Type);

            if (item != null)
            {
                for (int i = 0; i < x.Value; i++)
                {
                    TargetDeck.Add(item);
                }
            }
        });

        TargetDeck = TargetDeck.OrderBy(i => System.Guid.NewGuid()).ToList();
    }

    void ClearAll()
    {
        while (OptionsContainer.childCount > 0)
        {
            OptionsContainer.GetChild(0).gameObject.SetActive(false);
            OptionsContainer.GetChild(0).SetParent(transform);
        }

        Hand.Clear();

        if (TargetHand != null)
        {
            TargetHand.gameObject.SetActive(false);
            TargetHand = null;
        }

        CharacterDeck.Clear();
        TargetDeck.Clear();
    }

    public void RefreshUI()
    {
        Title.text = CurrentInstance.Title;

        CharacterPortrait.SetCharacter(CurrentCharacter);
        TargetPortrait.SetCharacter(CurrentTarget);

        RefreshDeckCounts();
    }

    public void RefreshDeckCounts()
    {
        CharacterDeckText.text = CharacterDeck.Count.ToString();
        TargetDeckText.text = TargetDeck.Count.ToString();
    }

    public void TargetDrawCard()
    {
        if (TargetDeck.Count == 0)
        {
            Win();
            return;
        }

        QuestioningItem card = TargetDeck[TargetDeck.Count - 1];
        TargetDeck.Remove(card);

        QuestioningItemUI cardUI = ResourcesLoader.Instance.GetRecycledObject("QuestioningItemUI").GetComponent<QuestioningItemUI>();
        cardUI.SetInfo(card,true);
        TargetHand = cardUI;

        cardUI.transform.SetParent(transform);
        cardUI.transform.localScale = Vector3.one;
        cardUI.transform.position = CharacterDeckText.transform.position;
        cardUI.transform.SetAsLastSibling();

        StartCoroutine(AnimateToTargetHand(cardUI));

        RefreshDeckCounts();
    }

    public void DrawCard()
    {
        if(CharacterDeck.Count == 0)
        {
            Lose();
            return;
        }

        QuestioningItem card = CharacterDeck[CharacterDeck.Count - 1];

        QuestioningItemUI cardUI = ResourcesLoader.Instance.GetRecycledObject("QuestioningItemUI").GetComponent<QuestioningItemUI>();
        cardUI.SetInfo(card);

        Hand.Add(cardUI);
        CharacterDeck.Remove(card);

        cardUI.transform.SetParent(transform);
        cardUI.transform.localScale = Vector3.one;
        cardUI.transform.position = CharacterDeckText.transform.position;
        cardUI.transform.SetAsLastSibling();

        StartCoroutine(AnimateToContainer(cardUI));

        RefreshDeckCounts();
    }

    IEnumerator AnimateToContainer(QuestioningItemUI card)
    {
        InputBlocker.SetActive(true);

        float height = Random.Range(-500f, 500f);

        float t = 0f;
        while (t < 1f)
        {
            t += (t + 1f) * Time.deltaTime;

            card.transform.position = Util.SplineLerpX(CharacterDeckText.transform.position, OptionsContainer.transform.position, height, t);

            yield return 0;
        }

        card.transform.SetParent(OptionsContainer, false);

        InputBlocker.SetActive(false);
    }

    IEnumerator AnimateToTargetHand(QuestioningItemUI card)
    {
        InputBlocker.SetActive(true);

        float height = Random.Range(-500f, 500f);

        float t = 0f;
        while (t < 1f)
        {
            t += (t + 1f) * Time.deltaTime;

            card.transform.position = Util.SplineLerpX(TargetDeckText.transform.position, TargetHandPosition.transform.position, height, t);

            yield return 0;
        }

        InputBlocker.SetActive(false);
    }

    public bool UseCard(QuestioningItemUI card)
    {
        if (!Hand.Contains(card))
        {
            return false;
        }

        Hand.Remove(card);
        card.transform.SetParent(transform);

        if (IsWin(card.CurrentItem, TargetHand.CurrentItem))
        {
            TargetHand.UseRight();
            TargetDrawCard();
            DrawCard();
            return true;
        }

        card.UseWrong();
        DrawCard();

        return false;
    }

    public void SpendCard(QuestioningItemUI card)
    {
        if (!Hand.Contains(card))
        {
            return;
        }

        Hand.Remove(card);
        card.transform.SetParent(transform);
        DrawCard();

        InputBlocker.SetActive(true);
        MatchEffectPanel.SetActive(true);
        MatchEffectPanel.transform.SetAsLastSibling();
        MatchEffectTitle.text = System.Text.RegularExpressions.Regex.Replace(card.CurrentItem.Type.ToString(), @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");


        switch (card.CurrentItem.Type)
        {
            case QuestioningItem.SpendType.AddTwoNewCardsToTheDeck:
                {
                    CORE.Instance.DelayedInvokation(2f,() =>
                    {
                        CharacterDeck.Add(PotentialQuestions[Random.Range(0, PotentialQuestions.Count)]);
                        CharacterDeck.Add(PotentialQuestions[Random.Range(0, PotentialQuestions.Count)]);
                        RefreshDeckCounts();

                        InputBlocker.SetActive(false);
                    });
                    break;
                }
            case QuestioningItem.SpendType.DrawTwoCards:
                {
                    CORE.Instance.DelayedInvokation(2f, () =>
                    {
                        DrawCard();
                        InputBlocker.SetActive(false);
                    });
                    break;
                }
            case QuestioningItem.SpendType.RandomizeOpponentsCard:
                {
                    CORE.Instance.DelayedInvokation(2f, () =>
                    {
                        TargetHand.SetInfo(PotentialAnswers[Random.Range(0, PotentialAnswers.Count)], true);
                        InputBlocker.SetActive(false);
                    });
                    break;
                }
            case QuestioningItem.SpendType.RedrawAllCards:
                {
                    CORE.Instance.DelayedInvokation(2f, () =>
                    {
                        int cardsToDraw = Hand.Count();
                        foreach(QuestioningItemUI cardUI in Hand)
                        {
                            CharacterDeck.Add(cardUI.CurrentItem);
                            cardUI.UseRight();
                        }
                        Hand.Clear();
                        RefreshDeckCounts();

                        CharacterDeck = CharacterDeck.OrderBy(i => System.Guid.NewGuid()).ToList();

                        for (int i=0;i<cardsToDraw;i++)
                        {
                            DrawCard();
                        }

                    });
                    break;
                }
        }
        return;
    }

    public bool IsWin(QuestioningItem charItem, QuestioningItem targetItem)
    {
        if(charItem.BeatsSkill != null && charItem.BeatsSkill == targetItem.RelevantSkill)
        {
            return true;
        }

        return false;
    }

    public void Forfeit()
    {
        WarningWindowUI.Instance.Show("Do you wish to forfeit this match?", Lose);
    }

    public void Lose()
    {
        InputBlocker.SetActive(true);
        MatchEffectPanel.SetActive(true);
        MatchEffectPanel.transform.SetAsLastSibling();
        MatchEffectTitle.text = "Failure!";

        CORE.Instance.DelayedInvokation(3f, () =>
        {
            InputBlocker.SetActive(false);
            this.gameObject.SetActive(false);

            WarningWindowUI.Instance.Show(CurrentCharacter.name + " Has failed questioning "+CurrentTarget.name+" properly.",null);
        });
    }

    public void Win()
    {
        InputBlocker.SetActive(true);
        MatchEffectPanel.SetActive(true);
        MatchEffectPanel.transform.SetAsLastSibling();
        MatchEffectTitle.text = "Success!";

        CORE.Instance.DelayedInvokation(3f, () =>
        {
            InputBlocker.SetActive(false);
            this.gameObject.SetActive(false);

            if (CurrentTarget.CurrentQuestioningInstance.CompleteLetter != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("Letter_From", CurrentTarget.CurrentQuestioningInstance.CompleteLetter.PresetSender);
                parameters.Add("Letter_To", CORE.PC);

                Letter letter = new Letter(CurrentTarget.CurrentQuestioningInstance.CompleteLetter, parameters);

                LetterDispenserEntity.Instance.DispenseLetter(letter);
            }


            CurrentTarget.CurrentQuestioningInstance = null;

        });
    }


    //DEBUG

    public void Test()
    {
        Character chara = CORE.Instance.Characters[Random.Range(0, CORE.Instance.Characters.Count)];
        Character targe = CORE.Instance.Characters[Random.Range(0, CORE.Instance.Characters.Count)];
        targe.CurrentQuestioningInstance = CORE.Instance.Database.DefaultQeustioningInstance.Clone();
        targe.CurrentQuestioningInstance.Title = "Test";

        Show(chara, targe);
    }
}

[System.Serializable]
public class QuestioningItem
{
    public BonusType RelevantSkill;

    public BonusType BeatsSkill;

    public List<string> Texts = new List<string>();

    public SpendType Type;

    public enum SpendType
    {
        DrawTwoCards,
        RandomizeOpponentsCard,
        RedrawAllCards,
        AddTwoNewCardsToTheDeck
    }
}
