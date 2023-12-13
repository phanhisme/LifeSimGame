using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AIStat
{
    Energy,
    Fun,
    Hunger
}

public class BaseAI : MonoBehaviour
{
    [Header("Fun")]
    [SerializeField] float InitialFunLevel = 0.5f;
    [SerializeField] float BaseFunDecayRate = 0.005f;
    [SerializeField] Slider FunDisplay;

    [Header("Energy")]
    [SerializeField] float InitialEnergyLevel = 0.5f;
    [SerializeField] float BaseEnergyDecayRate = 0.005f;
    [SerializeField] Slider EnergyDisplay;

    [Header("Hunger")]
    [SerializeField] float InitialHungerLevel = 0.5f;
    [SerializeField] float BaseHungerDecayRate = 0.005f;
    [SerializeField] Slider HungerDisplay;

    public float CurrentFun { get; protected set; }
    public float CurrentEnergy { get; protected set; }
    public float CurrentHunger { get; protected set; }

    protected Units pathScript;
    protected Grid gridScript;

    protected BaseInteraction currentInteraction = null;
    protected SmartObject pastObject;


    protected virtual void Awake()
    {
        pathScript = GameObject.Find("_AI Logic").GetComponent<Units>();
        gridScript = FindObjectOfType<Grid>();

        FunDisplay.value = CurrentFun = InitialFunLevel;
        EnergyDisplay.value = CurrentEnergy = InitialEnergyLevel;
        HungerDisplay.value = CurrentHunger = InitialHungerLevel;
    }

    //using virtual in case we need to override
    protected virtual void Update() 
    {
        DecayNeeds();
        pathScript.CheckOnTarget();

        if (currentInteraction != null)
        {
            if (pathScript.atTargetPosition)
            {
                //we are the performer, allow the interaction to run
                currentInteraction.Perform(this, OnInteractionFinished);
            }
        }
    }

    protected virtual void OnInteractionFinished(BaseInteraction interaction)
    {
        interaction.UnlockInteraction(); //done with it, unlock the interaction

        //current interaction become null right away after touching the destination
        currentInteraction = null;

        Debug.Log($"Finished {interaction.DisplayName}");
    }

    public void UpdateIndividualStat(AIStat target, float amount)
    {
        //update stats after choosing the interaction => can modify to only call this after finish performing
        Debug.Log($"Update {target} by {amount}");
        switch (target)
        {
            case AIStat.Energy: CurrentEnergy += amount;
                break;
            case AIStat.Fun: CurrentFun += amount;
                break;
            case AIStat.Hunger: CurrentHunger += amount;
                break;
        }
    }

    void DecayNeeds()
    {
        //decay needs overtime
        CurrentFun = Mathf.Clamp01(CurrentFun - BaseFunDecayRate * Time.deltaTime);
        FunDisplay.value = CurrentFun;

        CurrentEnergy = Mathf.Clamp01(CurrentEnergy - BaseEnergyDecayRate * Time.deltaTime);
        EnergyDisplay.value = CurrentEnergy;

        CurrentHunger = Mathf.Clamp01(CurrentHunger - BaseHungerDecayRate * Time.deltaTime);
        HungerDisplay.value = CurrentHunger;
    }
}
