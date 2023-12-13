using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    Units pathScript;
    Grid gridScript;

    protected BaseInteraction currentInteraction = null;
    protected SmartObject pastObject;
    
    [SerializeField] protected float PickInteractionInterval = 2f;
    protected float TimeUntilNextInteractionPick = -1f;

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
            if (pathScript.atTargetPosition /*&& !HasPerformedOnObject*/)
            {
                //we are the performer, allow the interaction to run
                currentInteraction.Perform(this, OnInteractionFinished);
            }
        }

        else
        {
            TimeUntilNextInteractionPick -= Time.deltaTime;
            
            //time to pick an interaction
            if (TimeUntilNextInteractionPick <= 0)
            {
                TimeUntilNextInteractionPick = PickInteractionInterval;
                //HasPerformedOnObject = false;
                PickRandomInteraction();
            }
        }             
    }

    private void OnInteractionFinished(BaseInteraction interaction)
    {
        interaction.UnlockInteraction(); //done with it, unlock the interaction
        
        //current interaction become null right away after touching the destination
        currentInteraction = null;

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

        if (selectedObject != null)
        {
            if (selectedObject == pastObject)
            {
                Debug.Log("Chose the same object " + selectedObject + " past: " + pastObject);
                return;
            }
        }

        //can perform the interaction
        if (selectedInteraction.CanPerform())
        {
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
}