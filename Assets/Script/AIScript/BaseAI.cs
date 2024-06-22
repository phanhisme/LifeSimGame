using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

    public bool toggleDecay = true; //turn decay on/off
    public bool starRatingInProcess = false;

    private Expressions starRating;
    private MoodletManager moodManager;

    protected virtual void Awake()
    {
        pathScript = GameObject.Find("_AI Logic").GetComponent<Units>();
        gridScript = FindObjectOfType<Grid>();
        starRating = FindObjectOfType<Expressions>();

        moodManager = FindObjectOfType<MoodletManager>();

        FunDisplay.value = CurrentFun = InitialFunLevel;
        EnergyDisplay.value = CurrentEnergy = InitialEnergyLevel;
        HungerDisplay.value = CurrentHunger = InitialHungerLevel;
    }

    //using virtual in case we need to override
    protected virtual void Update() 
    {
        if (toggleDecay) //get info once helps to prevent decay when star rating is running
        {
            DecayNeeds();
        }

        else //star rating in process
        {
            toggleDecay = false;

            if (!toggleDecay && !starRatingInProcess) //this have to find a way to only show 1 stat of the main player if the game has 2 players
            {
                //stop decaying and get the total value to validate points

                //starRating.UpdateExpression(); //update the expression after day
                //GetInfoOnce will only turn back on after the star rating is finished
                
                starRatingInProcess = true;
            }
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

            foreach (Moodlet mood in moodManager.runningMoodlet)
            {
                amount *= mood.effectPercentage;
            }
        }

        switch (target)
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
        Debug.Log("Total value = " + totalValue);

        foreach(AIStat stat in Enum.GetValues(typeof(AIStat)))
        {
            float penaltyPoint = 0; //for each need that does not do well
            if (CurrentFun < 0.25 || CurrentEnergy < 0.25 || CurrentHunger < 0.25)
            {
                penaltyPoint = 0.25f;
            }
            else if (CurrentFun < 0.5 || CurrentEnergy < 0.5 || CurrentHunger < 0.5)
            {
                penaltyPoint = 0.1f;
            }
            else if (CurrentFun < 0.75 || CurrentEnergy < 0.75 || CurrentHunger < 0.75)
            {
                penaltyPoint = 0.05f;
            }

            Debug.Log(penaltyPoint);
            totalValue -= penaltyPoint;
        }

            return totalValue;
    }
}
