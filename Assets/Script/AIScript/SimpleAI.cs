using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    Units pathScript;
    Grid gridScript;

    protected BaseInteraction currentInteraction = null;
    
    [SerializeField] protected float PickInteractionInterval = 2f;

    protected float TimeUntilNextInteractionPick = -1f;

    protected bool HasPerformedOnObject = false;

    private void Start()
    {
        pathScript = GetComponent<Units>();
        gridScript = FindObjectOfType<Grid>();
    }

    private void Update()
    {
        pathScript.CheckOnTarget();
        if (currentInteraction != null)
        {
            if (pathScript.atTargetPosition && !HasPerformedOnObject)
            {
                Debug.Log("perform on a new object");
                //we are the performer, allow the interaction to run
                currentInteraction.Perform(this, OnInteractionFinished);
            }
            else if (!pathScript.atTargetPosition)
            {
                HasPerformedOnObject = false;
            }
        }

        else
        {
            //if the AI does not have any interaction
            TimeUntilNextInteractionPick -= Time.deltaTime;

            //time to pick an interaction
            if (TimeUntilNextInteractionPick <= 0)
            {
                TimeUntilNextInteractionPick = PickInteractionInterval;
                PickRandomInteraction();
            }
        }             
    }

    private void OnInteractionFinished(BaseInteraction interaction)
    {
        interaction.UnlockInteraction(); //done with it, unlock the interaction
        currentInteraction = null;
        HasPerformedOnObject = true;

        Debug.Log($"Finished {interaction.DisplayName}");
    }

    private void PickRandomInteraction()
    {
        //pick an random object
        int index = Random.Range(0, SmartObjectManager.Instance.RegisteredObjects.Count);
        SmartObject selectedObject = SmartObjectManager.Instance.RegisteredObjects[index];
        
        //pick a random interaction
        int interactionIndex = Random.Range(0, selectedObject.Interations.Count);
        BaseInteraction selectedInteraction = selectedObject.Interations[interactionIndex];

        //can perform the interaction
        if (selectedInteraction.CanPerform())
        {
            //gridScript.CheckWalkable(selectedObject);
            if (!gridScript.CheckWalkable(selectedObject))
            {
                Debug.LogError($"Could not move to {selectedObject.name}");
                currentInteraction = null;
            }
            else if (gridScript.CheckWalkable(selectedObject))
            {
                currentInteraction = selectedInteraction;
                currentInteraction.LockInteraction();

                if (!HasPerformedOnObject && !pathScript.atTargetPosition)
                {
                    //not at the destination -> request path
                    Debug.Log($"Going to {currentInteraction.DisplayName} at {selectedObject.DisplayName}");
                    PathRequestManager.RequestPath(transform.position, selectedObject.InteractionPoint, pathScript.OnPathFound);
                }
                else if (HasPerformedOnObject && pathScript.atTargetPosition)
                {
                    //already at the target position
                    Debug.Log("already at the target position");
                }
                
            }
        }
    }
}