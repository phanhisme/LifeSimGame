using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class AdvancedAI : BaseAI
{
    [SerializeField] protected float DefaultInteractionScore = 0f;
    [SerializeField] protected float PickInteractionInterval = 2f;
    [SerializeField] protected int InteractionPickSize = 3;

    protected float TimeUntilNextInteractionPick = -1f;

    public SmartObject lastPicked;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        if (currentInteraction == null)
        {
            TimeUntilNextInteractionPick -= Time.deltaTime;

            //time to pick an interaction
            if (TimeUntilNextInteractionPick <= 0)
            {
                TimeUntilNextInteractionPick = PickInteractionInterval;
                PickBestInteraction();
            }
        }
    }
    
    float ScoreInteraction(BaseInteraction interaction)
    {
        //if there is nothing changes in the interaction score => score = 0
        if (interaction.StatChanges.Length == 0)
        {
            return DefaultInteractionScore;
        }

        float score = 0f;

        foreach(InteractionStatChange change in interaction.StatChanges)
        {
            score += ScoreChange(change.Target, change.Value);

            //check moodlet
            MoodletManager moodManager = FindObjectOfType<MoodletManager>();
            foreach(Moodlet mood in moodManager.runningMoodlet)
            {
                if (mood.effectType == Moodlet.EffectType.INFLUENCE)
                {
                    //Influence the AI to do what they critically needs by upgrading their score needs
                    score += 0.5f;
                }
            }
        }

        return score;
    }

    float ScoreChange(AIStat target, float amount)
    {
        float currentValue = 0f;
        switch (target)
        {
            case AIStat.Energy:
                currentValue = CurrentEnergy;
                break;
            case AIStat.Fun:
                currentValue = CurrentFun;
                break;
            case AIStat.Hunger:
                currentValue = CurrentHunger;
                break;
        }

        return (1f - currentValue) * amount;
    }

    class ScoredInteraction
    {
        public SmartObject TargetObject;
        public BaseInteraction Interaction;
        public float Score;
    }

    private void PickBestInteraction()
    {
        //loopthrough the objects
        List<ScoredInteraction> unsortedInteraction = new List<ScoredInteraction>();
        foreach(SmartObject smartObject in SmartObjectManager.Instance.RegisteredObjects)
        {
            foreach(BaseInteraction interaction in smartObject.Interations)
            {
                if (!interaction.CanPerform())
                {
                    continue;
                }

                float score = ScoreInteraction(interaction);
                unsortedInteraction.Add(new ScoredInteraction()
                {
                    TargetObject = smartObject,
                    Interaction = interaction,
                    Score = score
                });
            }
        }

        if (unsortedInteraction.Count == 0)
            return;

        //sort and pick from one of the best interaction
        List<ScoredInteraction> sortedInteraction = unsortedInteraction.OrderByDescending(scoredInteraction => scoredInteraction.Score).ToList();
        
        int maxIndex = Mathf.Min(InteractionPickSize, sortedInteraction.Count);
        int selectedIndex = Random.Range(0, maxIndex);

        SmartObject selectedObject = sortedInteraction[selectedIndex].TargetObject;
        BaseInteraction selectedInteraction = sortedInteraction[selectedIndex].Interaction;

        if (selectedObject != null)
        {
            if (selectedObject == pastObject)
            {
                Debug.Log("Chose the same object " + selectedObject + " past: " + pastObject);
                //when choose the same object again, update stat is not working currently for the second time
                TimeUntilNextInteractionPick = 1;
                return;
            }
        }

        currentInteraction = selectedInteraction;
        currentInteraction.LockInteraction();

        if (!gridScript.CheckWalkable(selectedObject))
        {
            Debug.LogError($"Could not move to {selectedObject.name}");
            currentInteraction = null;
        }
        else if (gridScript.CheckWalkable(selectedObject))
        {
            //not at the destination -> request path
            Debug.Log($"Going to {currentInteraction.DisplayName} at {selectedObject.DisplayName}");
           
            //last picked this item => check for chances to get bad outcomes
            lastPicked = selectedObject;

            PathRequestManager.RequestPath(transform.position, selectedObject.InteractionPoint, pathScript.OnPathFound);
            pastObject = selectedObject;
        }
    }

    public float DecayCheck(Moodlet moodlet) //this calls after getting a negative moodlet
    {
        if (moodlet.statRelated == AIStat.Fun) //based on what AI stat the moodlet has, increase the decay rate of its AI stat path
        {
            return BaseFunDecayRate += moodlet.effectPercentage;
        }

        else if (moodlet.statRelated == AIStat.Energy)
        {
            return BaseEnergyDecayRate += moodlet.effectPercentage;
        }

        else if (moodlet.statRelated == AIStat.Hunger)
        {
            return BaseHungerDecayRate += moodlet.effectPercentage;
        }

        else
            return 0f;

        //the rate used to be BaseHungerDecayRate *= moodlet.effectPercentage, though it was removed since the data can increase if recieve 2 moodlets at the same time
        //thus, using +=,-= will keep the rate clean.
        //using -= also allows to track back to its original data easily
    }
}