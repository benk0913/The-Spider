using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAndClickRegion : PointAndClickEntity
{
    [SerializeField]
    public RegionData CurrentRegion;

    public override void InternalHover()
    {
        base.InternalHover();

        GlobalMessagePrompterUI.Instance.Show(CurrentRegion.name, 1f);
    }

    public override void InternalUnhover()
    {
        base.InternalUnhover();

    }
}
