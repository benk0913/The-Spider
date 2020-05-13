using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestioningInstance", menuName = "DataObjects/QuestioningInstance", order = 2)]
public class QuestioningInstance : ScriptableObject
{
    public string Title;

    public LetterPreset CompleteLetter;

    public bool InstantFailure = false;

}
