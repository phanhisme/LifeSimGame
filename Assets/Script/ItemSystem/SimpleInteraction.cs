using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInteraction : BaseInteraction
{
    [SerializeField] protected int _MaxSimultaneousUsers = 1;
    protected int NumCurrentUsers = 0;

    public override bool CanPerform()
    {
        return NumCurrentUsers < _MaxSimultaneousUsers;
    }

    public override void LockInteraction()
    {
        ++NumCurrentUsers;

        if (NumCurrentUsers > _MaxSimultaneousUsers)
        {
            Debug.LogError($"Too many users have locked this interaction {_DisplayName}");
        }
    }

    public override void Perform()
    {
        if (NumCurrentUsers <= 0)
        {
            Debug.LogError($"Trying to perform an interaction when there are no users");
            return;
        }

        if (_InteractionType == EInteractionType.Instananeous)
        {

        }
    }

    public override void UnlockInteraction()
    {
        if (NumCurrentUsers <= 0)
        {
            Debug.LogError($"Trying to unlock an already unlocked interaction");
            --NumCurrentUsers;
        }
    }
}
