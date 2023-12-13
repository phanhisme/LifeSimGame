using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInteraction : BaseInteraction
{
    protected class PerformerInfo
    {
        public BaseAI PerformingAI;
        public float ElapseTime;
        public UnityAction<BaseInteraction> OnCompleted;
    }

    [SerializeField] protected int MaxSimultaneousUsers = 1;
    
    protected int NumCurrentUsers = 0;
    protected List<PerformerInfo> CurrentPerformers = new List<PerformerInfo>();

    public override bool CanPerform()
    {
        return NumCurrentUsers < MaxSimultaneousUsers;
    }

    public override void LockInteraction()
    {
        ++NumCurrentUsers;

        if (NumCurrentUsers > MaxSimultaneousUsers)
        {
            Debug.LogError($"Too many users have locked this interaction {_DisplayName}");
        }
    }

    //which one is the one who perform this action, know when the action is complete => simultaneous
    public override void Perform(BaseAI performer, UnityAction<BaseInteraction> onCompleted)
    {
        if (NumCurrentUsers <= 0)
        {
            Debug.LogError($"Trying to perform an interaction when there are no users {_DisplayName}");
            return;
        }

        //check the interaction type
        if (InteractionType == EInteractionType.Instananeous)
        {
            if (StatChanges.Length > 0)
            {
                ApplyStatChanges(performer, 1f);
            }

            onCompleted.Invoke(this);
        }
        else if (InteractionType == EInteractionType.Overtime)
        {
            CurrentPerformers.Add(new PerformerInfo() 
            {
                PerformingAI = performer,
                ElapseTime = 0,
                OnCompleted = onCompleted
            });
        }
    }

    public override void UnlockInteraction()
    {
        if (NumCurrentUsers <= 0)
        {
            Debug.LogError($"Trying to unlock an already unlocked interaction {_DisplayName}");
        }

        --NumCurrentUsers;
        //Mathf.Min();
    }

    protected virtual void Update()
    {
        //duration of the interaction is not working at the moment!
        //update any current performers (in reversed order)
        for (int index = CurrentPerformers.Count - 1; index >= 0; index--)
        {
            PerformerInfo performer = CurrentPerformers[index];

            float previousElapsedTime = performer.ElapseTime;
            performer.ElapseTime = Mathf.Min(performer.ElapseTime + Time.deltaTime, _Duration);

            if (StatChanges.Length > 0)
            {
                ApplyStatChanges(performer.PerformingAI, (performer.ElapseTime - previousElapsedTime) / _Duration);
            }

            //interaction complete?
            if (performer.ElapseTime >= _Duration)
            {
                performer.OnCompleted.Invoke(this);
                CurrentPerformers.RemoveAt(index);
            }
        }
    }
}
