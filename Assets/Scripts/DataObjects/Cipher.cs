using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cipher", menuName = "DataObjects/Cipher", order = 2)]
public class Cipher : ScriptableObject
{
    public List<CipherLetter> Replacements = new List<CipherLetter>();

    public string Convert(string message)
    {
        foreach(CipherLetter replacement in Replacements)
        {
            message.Replace(replacement.letter, replacement.toLetter);
        }

        return message;
    }
}

[System.Serializable]
public class CipherLetter
{
    public char letter;
    public char toLetter;
}
