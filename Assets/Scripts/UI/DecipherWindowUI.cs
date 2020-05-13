using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecipherWindowUI : MonoBehaviour
{
    public static DecipherWindowUI Instance;

    [SerializeField]
    TextMeshProUGUI OriginalText;

    [SerializeField]
    TextMeshProUGUI DecipheredText;

    [SerializeField]
    TextMeshProUGUI DecipheredTextHighlighted;

    [SerializeField]
    TextMeshProUGUI CurrentLetterText;

    [SerializeField]
    Transform LettersContainer;

    [System.NonSerialized]
    public string OriginalEncryptedMessage;

    [System.NonSerialized]
    public string OriginalDecryptedMessage;

    [System.NonSerialized]
    public Cipher Cipher;

    public string LetterSpacingTagOpen
    {
        get
        {
            return Cipher ? ("<mspace="+Cipher.LetterSpacing+">") : " < mspace=40>";
        }
    }

    public const string LetterSpacingTagClose = "</mspace>";

    char[] lettersInText;

    Dictionary<char, char> CurrentReplacements = new Dictionary<char, char>();

    [System.NonSerialized]
    public char HoveredLetter = '@';

    [System.NonSerialized]
    public char SelectedLetter = '@';

    [System.NonSerialized]
    public int SelectedIndex = 0;

    [SerializeField]
    Button ResolveButton;

    [SerializeField]
    Button CloseButton;

    public System.Action OnResolveAction;

    public string ResolvedMessage
    {
        get
        {

            string resolve = OriginalEncryptedMessage;

            for(int i=0;i<resolve.Length;i++)
            {
                if (CurrentReplacements.ContainsKey(resolve[i]))
                {
                    string replacement = CurrentReplacements[resolve[i]].ToString();

                    resolve = resolve.Remove(i,1);
                    resolve = resolve.Insert(i, replacement);
                }
                else
                {
                    if (Cipher.Replacements.Find(x => x.letter == resolve[i]) != null)
                    {
                        resolve = resolve.Remove(i, 1);
                        resolve = resolve.Insert(i, '\u0219'.ToString());
                    }
                }
            }

            return resolve;
        }
    }

    public bool IsEncryptionSolved
    {
        get
        {
            return ResolvedMessage == OriginalDecryptedMessage;
        }
    }

    public bool InSelectionZone;

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (MouseLook.Instance == null) return;

        MouseLook.Instance.UnfocusOnItemInHands();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SelectCurrentCharacter();
        }

        if(Input.GetKey(KeyCode.Escape))
        {
            this.gameObject.SetActive(false);
        }

        if (Input.anyKeyDown)
        {
            if (Input.inputString.Length > 0)
            {
                foreach (char letter in lettersInText)
                {
                    char targetChar;

                    if (char.ToLower(letter) == char.ToLower(Input.inputString[0]))
                    {
                        SetCharacter(letter);
                        break;
                    }
                }
            }
        }

    }

    void SelectCurrentCharacter()
    {
        if(!InSelectionZone)
        {
            return;
        }

        SelectedLetter = HoveredLetter;
        RefreshUI();
    }

    public void EnterSelectionZone()
    {
        InSelectionZone = true;
    }

    public void ExitSelectionZone()
    {
        InSelectionZone = false;
    }

    public void SelectIndex(char character, int index)
    {
        HoverLetter(OriginalEncryptedMessage[index], index);
    }

    public void HoverLetter(char character, int index)
    {
        HoveredLetter = character;
        SelectedIndex = index;

        RefreshUI();
    }

    public void SetCharacter(char character)
    {
        if(SelectedLetter == '@')
        {
            return;
        }

        UnsetCharacter(character);

        if (CurrentReplacements.ContainsKey(SelectedLetter))
        {
            UnsetCharacter(CurrentReplacements[SelectedLetter]);
        }

        CurrentReplacements.Add(SelectedLetter, character);

        RefreshUI();
    }

    public void UnsetCharacter(char character)
    {
        for (int i = 0; i < CurrentReplacements.Keys.Count; i++)
        {
            if(CurrentReplacements[CurrentReplacements.Keys.ElementAt(i)] == character)
            {
                CurrentReplacements.Remove(CurrentReplacements.ElementAt(i).Key);
                RefreshUI();
                return;
            }
        }
    }

    public void Show(string encryptedMessage, Cipher cipher, System.Action OnResolve = null)
    {
        this.gameObject.SetActive(true);

        OnResolveAction = OnResolve;

        MouseLook.Instance.FocusOnItemInHands();

        this.OriginalEncryptedMessage = encryptedMessage;
        this.Cipher = cipher;

        this.OriginalDecryptedMessage = this.Cipher.Decipher(OriginalEncryptedMessage);

        lettersInText = this.Cipher.GetAllExistingLetters(this.OriginalDecryptedMessage);

        System.Random rnd = new System.Random();
        lettersInText = lettersInText.OrderBy(x => rnd.Next()).ToArray();

        CurrentReplacements.Clear();

        HoveredLetter = '@';

        if(cipher.Font != null)
        {
            OriginalText.font = cipher.Font;
            CurrentLetterText.font = cipher.Font;
            OriginalText.lineSpacing = cipher.LineSpacing;
        }
        else
        {
            OriginalText.font = CORE.Instance.Database.DefaultFont;
            CurrentLetterText.font = CORE.Instance.Database.DefaultFont;
            OriginalText.lineSpacing = 190.5f;
        }

        RefreshUI();
    }

    void RefreshUI()
    {
        if (SelectedLetter == '@')
        {
            CurrentLetterText.text = HoveredLetter == '@' ? "-" : HoveredLetter.ToString();
            CurrentLetterText.color = Color.white;
        }
        else
        {
            CurrentLetterText.text = SelectedLetter == '@' ? "-" : SelectedLetter.ToString();
            CurrentLetterText.color = Color.yellow;
        }

        OriginalText.text = LetterSpacingTagOpen + OriginalEncryptedMessage;

        if (HoveredLetter != '@')
        {
            DecipheredText.text = LetterSpacingTagOpen + ResolvedMessage;
            DecipheredTextHighlighted.text = LetterSpacingTagOpen + GetHighlightedCharacterStringFromOther(HoveredLetter,ResolvedMessage, OriginalEncryptedMessage);
        }
        else
        {
            DecipheredText.text = LetterSpacingTagOpen + ResolvedMessage;
            DecipheredTextHighlighted.text = LetterSpacingTagOpen + ResolvedMessage;
        }

        ClearContainer();
        foreach(char letter in lettersInText)
        {
            GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("LetterInstanceUI");
            tempObj.transform.SetParent(LettersContainer, false);

            tempObj.GetComponentInChildren<TextMeshProUGUI>().text = letter.ToString();

            char currentLetter = letter;

            tempObj.GetComponent<Button>().onClick.RemoveAllListeners();

            if (CurrentReplacements.ContainsValue(letter))
            {
                tempObj.GetComponent<Image>().color = Color.blue;
                tempObj.GetComponent<Button>().onClick.AddListener(()=> { UnsetCharacter(currentLetter); });
            }
            else
            {
                tempObj.GetComponent<Image>().color = Color.white;
                tempObj.GetComponent<Button>().onClick.AddListener(() => { SetCharacter(currentLetter); });
            }

        }

        if (IsEncryptionSolved)
        {
            ResolveButton.gameObject.SetActive(true);
            CloseButton.gameObject.SetActive(false);
        }
        else
        {
            ResolveButton.gameObject.SetActive(false);
            CloseButton.gameObject.SetActive(true);
        }
    }

    public void ResolvePuzzle()
    {
        OnResolveAction?.Invoke();
        this.gameObject.SetActive(false);

        CORE.Instance.InvokeEvent("Letter Deciphered");
    }

    public string GetHighlightedCharacterString(char letter, string inString)
    {
        string startTagSelected = "<mark=green>";
        string startTag = "<mark=blue>";
        string endTag = "</mark>";

        for (int i = 0; i < inString.Length; i++)
        {
            if (letter == inString[i])
            {
                inString = inString.Insert(i, startTag);
                i += startTag.Length;

                if (i + 1 >= inString.Length)
                {
                    inString += endTag;
                }
                else
                {
                    inString = inString.Insert(i + 1, endTag);
                }

                i += endTag.Length;
            }
        }

        return inString;
    }

    public string GetHighlightedCharacterStringFromOther(char letter, string inString, string otherString)
    {
        string startTagSelected = "<mark=#FFFFFF>";
        string startTag = "<mark=#0096FF>";
        string endTag = "</mark>";

        for (int i = 0; i < inString.Length; i++)
        {
            if (letter == otherString[i])
            {
                inString = inString.Insert(i, startTag);
                otherString = otherString.Insert(i, startTag);
                i += startTag.Length;

                if (i + 1 >= inString.Length)
                {
                    inString += endTag;
                    otherString += endTag;
                }
                else
                {
                    inString = inString.Insert(i + 1, endTag);
                    otherString = otherString.Insert(i + 1, endTag);
                }

                i += endTag.Length;
                otherString += endTag.Length;
            }
            else if (SelectedLetter == otherString[i])
            {
                inString = inString.Insert(i, startTagSelected);
                otherString = otherString.Insert(i, startTagSelected);
                i += startTagSelected.Length;

                if (i + 1 >= inString.Length)
                {
                    inString += endTag;
                    otherString += endTag;
                }
                else
                {
                    inString = inString.Insert(i + 1, endTag);
                    otherString = otherString.Insert(i + 1, endTag);
                }

                i += endTag.Length;
                otherString += endTag.Length;
            }
        }

        return inString;
    }

    void ClearContainer()
    {
        while(LettersContainer.childCount > 0)
        {
            LettersContainer.GetChild(0).gameObject.SetActive(false);
            LettersContainer.GetChild(0).SetParent(transform);
        }
    }
}
