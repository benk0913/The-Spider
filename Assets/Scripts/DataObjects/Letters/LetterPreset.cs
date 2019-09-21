using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "LetterPreset", menuName = "DataObjects/LetterPreset", order = 2)]
public class LetterPreset : ScriptableObject
{
    [SerializeField]
    public string Title;

    [SerializeField][TextArea(4,6)]
    public string Description;

    [SerializeField]
    [TextArea(4, 6)]
    public string SideNotes;

    [SerializeField]
    public string From;

    [SerializeField]
    public Material Seal;

    [SerializeField]
    public Quest QuestAttachment;


}
