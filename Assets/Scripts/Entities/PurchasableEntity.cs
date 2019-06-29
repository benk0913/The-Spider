using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableEntity : MonoBehaviour
{
    [SerializeField]
    public int Price;

    [SerializeField]
    public PurchasablePlotType Type;

    [SerializeField]
    public float RevenueMultiplier = 1f;

    [SerializeField]
    public float RiskMultiplier = 1f;

    [System.Serializable]
    public enum PurchasablePlotType
    {
        Urban,
        Coastal,
        Naval
    }

    public void OnClick()
    {
        
    }
}
