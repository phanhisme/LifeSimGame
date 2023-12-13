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
            PathRequestManager.RequestPath(transform.position, selectedObject.InteractionPoint, pathScript.OnPathFound);
            pastObject = selectedObject;
        }
    }
}