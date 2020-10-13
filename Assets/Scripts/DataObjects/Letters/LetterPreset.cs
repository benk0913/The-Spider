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
    public string RTLDescription;

    [SerializeField]
    [TextArea(4, 6)]
    public string SideNotes;

    [SerializeField]
    public string From;

    [SerializeField]
    public Material Seal;

    [SerializeField]
    public Quest QuestAttachment;

    [SerializeField]
    public Character PresetSender;

    [SerializeField]
    public Character PresetSubjectCharacter;

    [SerializeField]
    public Cipher Encryption;

    [SerializeField]
    public bool FromRaven = false;

    [SerializeField]
    public string DTKeyword;

    public bool LockPassTime = false;

    public string VoiceLine;

    public LetterPreset CreateClone()
    {
        LetterPreset clone = Instantiate(this);
        clone.name = this.name;
        
        if (clone.QuestAttachment != null)
        {
            clone.QuestAttachment.CreateClone();
        }

        return clone;
    }


}
