using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization; //useful for rename 

public enum EInteractionType
{
    Instananeous = 0,
    Overtime = 1
}

[System.Serializable]
public class InteractionStatChange
{
    public AIStat Target;
    public float Value;
}

public abstract class BaseInteraction : MonoBehaviour
{
    [SerializeField] protected string _DisplayName;
    [SerializeField] protected EInteractionType _InteractionType = EInteractionType.Instananeous;
    [SerializeField] protected float _Duration = 0f;
    [SerializeField,FormerlySerializedAs("StatChanges")] protected InteractionStatChange[] _StatChanges;

    public string DisplayName => _DisplayName;
    public EInteractionType InteractionType => _InteractionType;
    public float Duration => _Duration;
    public InteractionStatChange[] StatChanges => _StatChanges;

    public abstract bool CanPerform();

    //lock the interaction so that they will not do other interaction befor finishing the current action
    public abstract void LockInteraction();

    public abstract void Perform(BaseAI performer, UnityAction<BaseInteraction> onCompleted);

    public abstract void UnlockInteraction();

    public void ApplyStatChanges(BaseAI performer,float proportion)
    {
        foreach (InteractionStatChange statChange in StatChanges)
        {
            performer.UpdateIndividualStat(statChange.Target, statChange.Value * proportion);
        }
    }
}
