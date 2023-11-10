using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EInteractionType
{
    Instananeous = 0,
    Overtime = 1
}

public abstract class BaseInteraction : MonoBehaviour
{
    [SerializeField] protected string _DisplayName;
    [SerializeField] protected EInteractionType _InteractionType = EInteractionType.Instananeous;
    [SerializeField] protected float _Duration = 0f;

    public string DisplayName => _DisplayName;

    public abstract bool CanPerform();

    //lock the interaction so that they will not do other interaction befor finishing the current action
    public abstract void LockInteraction();

    public abstract void Perform();

    public abstract void UnlockInteraction();
}
