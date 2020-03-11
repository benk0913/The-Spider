using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Cipher", menuName = "DataObjects/Cipher", order = 2)]
public class Cipher : ScriptableObject
{
    public List<CipherLetter> Replacements = new List<CipherLetter>();

    public bool SupportUpperCase = false;

    public TMP_FontAsset Font;

    public float LineSpacing = 190.5f;

    public string Convert(string message)
    {

        string encryptedString = "";

        if(!SupportUpperCase)
        {
            message.ToLower();
        }

        for(int i=0;i<message.Length;i++)
        {
            char currentChar = message[i];

            CipherLetter cipherFound = Replacements.Find(x => x.letter == message[i]);

            if(cipherFound != null)
            {
                currentChar = cipherFound.toLetter;
            }

            encryptedString += currentChar;
        }

        return encryptedString;
    }
    
    public string Decipher(string message)
    {
        string decryptedString = "";

        if (!SupportUpperCase)
        {
            message.ToLower();
        }

        for (int i = 0; i < message.Length; i++)
        {
            char currentChar = message[i];

            CipherLetter cipherFound = Replacements.Find(x => x.toLetter == message[i]);

            if (cipherFound != null)
            {
                currentChar = cipherFound.letter;
            }

            decryptedString += currentChar;
        }

        return decryptedString;
    }

    public char[] GetAllExistingLetters(string message)
    {
        if(!SupportUpperCase)
        {
            message.ToLower();
        }

        List<char> letters = new List<char>();
        for(int i=0;i<message.Length;i++)
        {
            if(letters.Contains(message[i]))
            {
                continue;
            }

            if(Replacements.Find(x=>x.letter == message[i] || x.toLetter == message[i]) == null)
            {
                continue;
            }

            letters.Add(message[i]);
        }

        return letters.ToArray();
    }

}

[System.Serializable]
public class CipherLetter
{
    public char letter;
    public char toLetter;
}
