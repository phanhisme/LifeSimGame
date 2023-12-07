using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TV_SmartObject))]

public class TVInteraction : SimpleInteraction
{
    protected TV_SmartObject LinkedTV;

    protected void Awake()
    {
        LinkedTV = GetComponent<TV_SmartObject>();
    }

    public override bool CanPerform()
    {
        return base.CanPerform() && LinkedTV.IsOn;
    }
}
