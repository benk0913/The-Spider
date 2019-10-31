using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Faction", menuName = "DataObjects/Faction", order = 2)]
public class Faction : ScriptableObject
{
    public Material WaxMaterial;
    public Color FactionColor;
    public string RoomScene;
    public Character FactionHead;
    public GameObject FactionSelectionPrefab;

    public Property[] FactionProperties;

    public LetterPreset[] StartingLetters;

    public AgentAction[] AgentActionsOnLocation;
    public AgentAction[] AgentActionsOnAgent;
    public PlayerAction[] PlayerActionsOnLocation;
    public PlayerAction[] PlayerActionsOnAgent;

    public bool isLockedByDefault = true;
    public bool isLocked =  false;
    public bool isPlayable = true;
}
