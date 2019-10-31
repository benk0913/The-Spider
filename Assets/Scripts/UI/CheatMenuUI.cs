using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatMenuUI : MonoBehaviour
{
    public void ShowAllCharacters()
    {
        foreach(Character character in CORE.Instance.Characters)
        {
            character.Known.KnowAllBasic();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ShowAllCharacters();
        }
    }
}
