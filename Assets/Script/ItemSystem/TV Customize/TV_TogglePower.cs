using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TV_SmartObject))]

public class TV_TogglePower : SimpleInteraction
{
    protected TV_SmartObject LinkedTV;

    protected void Awake()
    {
        LinkedTV = GetComponent<TV_SmartObject>();
    }

    public override void Perform(BaseAI performer, UnityAction<BaseInteraction> onCompleted)
    {
        LinkedTV.ToggleState();
        base.Perform(performer, onCompleted);
    }
}
