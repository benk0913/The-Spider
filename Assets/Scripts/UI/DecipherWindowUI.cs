using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecipherWindowUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI OriginalText;

    [SerializeField]
    TextMeshProUGUI DecipheredText;

    [SerializeField]
    Transform LettersContainer;

    public string OriginalEncryptedMessage;
    public string OriginalDecryptedMessage;
    public Cipher Cipher;

    char[] lettersInText;

    Dictionary<char, char> CurrentReplacements = new Dictionary<char, char>();

    public char SelectedCharacter = '@';

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.inputString.Length > 0)
            {
                foreach (char letter in lettersInText)
                {
                    if (letter == Input.inputString[0])
                    {
                        SetCharacter(letter);
                        break;
                    }
                }
            }
        }
    }

    public void SelectCharacter(char character)
    {
        SelectedCharacter = character;
        RefreshUI();
    }

    public void SetCharacter(char character)
    {
        if(SelectedCharacter == '@')
        {
            return;
        }

        UnsetCharacter(character);

        CurrentReplacements.Add(SelectedCharacter, character);

        RefreshUI();
    }

    public void UnsetCharacter(char character)
    {
        if(!CurrentReplacements.ContainsKey(character))
        {
            return;
        }

        CurrentReplacements.Remove(character);

        RefreshUI();
    }

    public void Show(string encryptedMessage, Cipher cipher)
    {
        this.OriginalEncryptedMessage = encryptedMessage;
        this.Cipher = cipher;

        this.OriginalDecryptedMessage = this.Cipher.Decipher(OriginalEncryptedMessage);

        lettersInText = this.Cipher.GetAllExistingLetters(this.OriginalDecryptedMessage);

        CurrentReplacements.Clear();

        SelectedCharacter = '@';

        RefreshUI();
    }

    void RefreshUI()
    {
        if (SelectedCharacter != '@')
        {
            OriginalText.text = GetHighlightedCharacterString(SelectedCharacter, OriginalEncryptedMessage);
        }
        else
        {
            OriginalText.text = OriginalEncryptedMessage;
        }

        ClearContainer();
        foreach(char letter in lettersInText)
        {
            GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject("LetterInstanceUI");
            tempObj.GetComponentInChildren<TextMeshProUGUI>().text = letter.ToString();

            char currentLetter = letter;

            tempObj.GetComponent<Button>().onClick.RemoveAllListeners();

            if (CurrentReplacements.ContainsKey(letter))
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
    }

    public string GetHighlightedCharacterString(char letter, string inString)
    {
        string startTag = "<mark=#FF8000>";
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

    void ClearContainer()
    {
        while(LettersContainer.childCount > 0)
        {
            LettersContainer.GetChild(0).gameObject.SetActive(false);
            LettersContainer.GetChild(0).SetParent(transform);
        }
    }
}
