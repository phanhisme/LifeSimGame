using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInteraction : BaseInteraction
{
    protected class PerformerInfo
    {
        public float ElapseTime;
        public UnityEvent<BaseInteraction> OnCompleted;
    }

    [SerializeField] protected int _MaxSimultaneousUsers = 1;
    
    protected int NumCurrentUsers = 0;
    protected List<PerformerInfo> CurrentPerformers = new List<PerformerInfo>();

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

    //which one is the one who perform this action, know when the action is complete => simultaneous
    public override void Perform(MonoBehaviour performer, UnityEvent<BaseInteraction> onCompleted)
    {
        if (NumCurrentUsers <= 0)
        {
            Debug.LogError($"Trying to perform an interaction when there are no users {_DisplayName}");
            return;
        }

        //check the interaction type
        if (_InteractionType == EInteractionType.Instananeous)
        {
            onCompleted.Invoke(this);
        }
        else if (_InteractionType == EInteractionType.Overtime)
        {
            CurrentPerformers.Add(new PerformerInfo() { ElapseTime = 0, OnCompleted = onCompleted });
        }
    }

    public override void UnlockInteraction()
    {
        if (NumCurrentUsers <= 0)
        {
            Debug.LogError($"Trying to unlock an already unlocked interaction {_DisplayName}");
            --NumCurrentUsers;
        }
    }

    protected virtual void Update()
    {
        //update any current performers (in reversed order)
        for (int index = CurrentPerformers.Count - 1; index >= 0; index--)
        {
            PerformerInfo performer = CurrentPerformers[index];

            performer.ElapseTime += Time.deltaTime;

            //interaction complete?
            if (performer.ElapseTime >= _Duration)
            {
                performer.OnCompleted.Invoke(this);
                CurrentPerformers.RemoveAt(index);
            }
        }
    }
}
