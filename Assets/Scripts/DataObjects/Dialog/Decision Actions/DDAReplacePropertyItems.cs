using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DDAReplacePropertyItems", menuName = "DataObjects/Dialog/Actions/DDAReplacePropertyItems", order = 2)]
public class DDAReplacePropertyItems : DialogDecisionAction
{
    public List<Item> WithItems = new List<Item>();
    public bool ClearPreviousItems;

    public Property LookForProperty;
    public string LookForID;
    public Property LookForNearDistrict;
    public bool RandomLocation;

    public override void Activate()
    {
        List<LocationEntity> PossibleLocations = new List<LocationEntity>();
        PossibleLocations.AddRange(CORE.Instance.Locations);

        if(!string.IsNullOrEmpty(LookForID))
        {
            PossibleLocations.RemoveAll(x => x.ID != LookForID);
        }

        if(LookForProperty != null)
        {
            PossibleLocations.RemoveAll(x => x.CurrentProperty != LookForProperty);
        }

        if (LookForNearDistrict != null)
        {
            PossibleLocations.RemoveAll(x => x.NearestDistrict.CurrentProperty != LookForNearDistrict);
        }

        if(PossibleLocations.Count <= 0)
        {
            return;
        }

        if(RandomLocation)
        {
            ApplyItemChange(PossibleLocations[Random.Range(0,PossibleLocations.Count)]);
        }
        else
        {
            ApplyItemChange(PossibleLocations[0]);
        }
    }

    void ApplyItemChange(LocationEntity location)
    {
        if(ClearPreviousItems)
        {
            location.Inventory.Clear();
        }

        WithItems.ForEach((x) => location.Inventory.Add(x.Clone()));
    }
}
