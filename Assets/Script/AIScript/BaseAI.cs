using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum AIStat
{
    Energy,
    Fun,
    Hunger,
    None
}

public class BaseAI : MonoBehaviour
{
    [Header("Fun")]
    [SerializeField] float InitialFunLevel = 0.5f;
    [SerializeField] public float BaseFunDecayRate = 0.005f;
    [SerializeField] Slider FunDisplay;

    [Header("Energy")]
    [SerializeField] float InitialEnergyLevel = 0.5f;
    [SerializeField] public float BaseEnergyDecayRate = 0.005f;
    [SerializeField] Slider EnergyDisplay;

    [Header("Hunger")]
    [SerializeField] float InitialHungerLevel = 0.5f;
    [SerializeField] public float BaseHungerDecayRate = 0.005f;
    [SerializeField] Slider HungerDisplay;

    public float CurrentFun { get; protected set; }
    public float CurrentEnergy { get; protected set; }
    public float CurrentHunger { get; protected set; }

    protected Units pathScript;
    protected Grid gridScript;

    protected BaseInteraction currentInteraction = null;
    protected SmartObject pastObject;

    private CycleDay dayTimer;
    private MoodletManager moodManager;

    public float needValue;

    protected virtual void Awake()
    {
        pathScript = GameObject.Find("_AI Logic").GetComponent<Units>();
        gridScript = FindObjectOfType<Grid>();

        moodManager = FindObjectOfType<MoodletManager>();
        dayTimer = FindObjectOfType<CycleDay>();

        FunDisplay.value = CurrentFun = InitialFunLevel;
        EnergyDisplay.value = CurrentEnergy = InitialEnergyLevel;
        HungerDisplay.value = CurrentHunger = InitialHungerLevel;
    }

    //using virtual in case we need to override
    protected virtual void Update() 
    {
        if (dayTimer.currentStatus == CycleDay.Status.RUNNING) //get info once helps to prevent decay when star rating is running
        {
            DecayNeeds();
        }
        
        pathScript.CheckOnTarget();

        if (currentInteraction != null)
        {
            if (pathScript.atTargetPosition)
            {
                //we are the performer, allow the interaction to run
                currentInteraction.Perform(this, OnInteractionFinished);
            }
        }

        moodManager.MoodletDataNeeds();

        if (dayTimer.currentStatus == CycleDay.Status.RATINGINPROCESS && !dayTimer.getValue)
        {
            //get value after the day ends -> set star
            Expressions expression = FindObjectOfType<Expressions>();
            needValue = GetTotalValue();
            expression.StarSystem(needValue);

            dayTimer.getValue = true;
        }
    }

    protected virtual void OnInteractionFinished(BaseInteraction interaction)
    {
        moodManager.AddData(); // => check after finish the object to check for its performance

        interaction.UnlockInteraction(); //done with it, unlock the interaction

        //current interaction become null right away after touching the destination
        currentInteraction = null;
        //thus, sometimes result in the interaction completed when the player is not on the targeted interaction point

        Debug.Log($"Finished {interaction.DisplayName}");
    }

    public void UpdateIndividualStat(AIStat target, float amount)
    {
        //update stats after choosing the interaction => can modify to only call this after finish performing
        //Debug.Log($"Update {target} by {amount}");

        //check moodlet action first
        if (moodManager.runningMoodlet != null)
        {
            Debug.Log("Moodlet available for checking");

            foreach (Moodlet mood in moodManager.runningMoodlet) //if there is a running moodlet that may effect the outcome => check
            {
                //boost regen rate
                if (mood.effectType == Moodlet.EffectType.POSITIVE || mood.effectType == Moodlet.EffectType.INFLUENCE) //while positive helps regen faster
                {
                    amount += mood.effectPercentage;
                }
            }
        }

        switch (target) //adding the amount based on the object chose and its chosen stat
        {
            case AIStat.Energy:
                CurrentEnergy += amount;
                break;
            case AIStat.Fun:
                CurrentFun += amount;
                break;
            case AIStat.Hunger:
                CurrentHunger += amount;
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

    public float GetTotalValue()
    {
        float totalValue = CurrentFun + CurrentEnergy + CurrentHunger;
        float penaltyPoint = 0f; //for each need that does not do well

        if (CurrentFun < 0.25f || CurrentEnergy < 0.25f || CurrentHunger < 0.25f)
        {
            penaltyPoint += 0.25f;
        }
        if (CurrentFun < 0.5f || CurrentEnergy < 0.5f || CurrentHunger < 0.5f)
        {
            penaltyPoint += 0.1f;
        }
        if (CurrentFun < 0.75f || CurrentEnergy < 0.75f || CurrentHunger < 0.75f)
        {
            penaltyPoint += 0.05f;
        }

        //Debug.Log(penaltyPoint);
        totalValue -= penaltyPoint;

        //Debug.Log("Total value = " + totalValue);
        return totalValue;
    }
}
