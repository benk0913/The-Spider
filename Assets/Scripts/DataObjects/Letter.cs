using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Letter", menuName = "DataObjects/Letter", order = 2)]
public class Letter : ScriptableObject
{
    [SerializeField]
    public string Title;

    [SerializeField][TextArea(4,2)]
    public string Description;

    [SerializeField]
    public string From;

    [SerializeField]
    public Material Seal;



}
