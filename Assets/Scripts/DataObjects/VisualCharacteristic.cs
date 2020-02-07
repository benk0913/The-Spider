using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Visual Characteristic", menuName = "DataObjects/Visual Characteristic", order = 2)]
public class VisualCharacteristic : ScriptableObject
{
    [SerializeField]
    public VCType Type;

    [SerializeField]
    public List<VisualCharacteristic> Pool = new List<VisualCharacteristic>();

    [SerializeField]
    public Sprite Sprite;

    public List<string> PossibleRumors = new List<string>();


    public VisualCharacteristic GetNext(VisualCharacteristic set)
    {
        int cIndex = GetVCIndex(set);
        cIndex++;

        if (cIndex >= Pool.Count)
        {
            cIndex = 0;
        }

        return Pool[cIndex];
    }

    public VisualCharacteristic GetPrevious(VisualCharacteristic set)
    {
        int cIndex = GetVCIndex(set);
        cIndex--;

        if (cIndex < 0)
        {
            cIndex = Pool.Count - 1;
        }

        return Pool[cIndex];
    }

    int GetVCIndex(VisualCharacteristic set)
    {
        for (int i = 0; i < Pool.Count; i++)
        {
            if (Pool[i] == set)
            {
                return i;
            }
        }

        return 0;
    }

    public VisualCharacteristic GetVCByName(string sName, bool fallback = true)
    {

        if (this.name == sName)
        {
            return this;
        }

        VisualCharacteristic visualChar = null;
        foreach (VisualCharacteristic vc in Pool)
        {
            visualChar = vc.GetVCByName(sName);

            if (vc != null
                && vc.name == sName)
            {
                return visualChar;
            }
        }

        if(fallback && Pool.Count > 0)
        {
            return Pool[0];
        }

        return null;
    }
}

public enum VCType
{
    SkinColor,
    Face,
    HairColor,
    Hair,
    Clothing
}
