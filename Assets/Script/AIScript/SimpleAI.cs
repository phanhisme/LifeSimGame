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

    private void Awake()
    {
        pathScript = GetComponent<Units>();
        gridScript = FindObjectOfType<Grid>();
    }

    private void Update()
    {
        if (currentInteraction != null)
        {
            pathScript.CheckOnTarget();
            if (pathScript.atTargetPosition)
            {
                //we are the performer, allow the interaction to run
                currentInteraction.Perform(this, OnInteractionbFinished);
            }
        }

        else
        {
            //if the AI is not at the target position
            TimeUntilNextInteractionPick -= Time.deltaTime;

            //time to pick an interaction
            if (TimeUntilNextInteractionPick <= 0)
            {
                TimeUntilNextInteractionPick = PickInteractionInterval;
                PickRandomInteraction();
            }
        }             
    }

    private void OnInteractionbFinished(BaseInteraction interaction)
    {
        interaction.UnlockInteraction(); //done with it, unlock the interaction
        currentInteraction = null;

        Debug.Log($"Finished {interaction.DisplayName}");
        pathScript.atTargetPosition = false;
    }

    private void PickRandomInteraction()
    {
        //pick an randoom object
        int index = Random.Range(0, SmartObjectManager.Instance.RegisteredObjects.Count);
        SmartObject selectedObject = SmartObjectManager.Instance.RegisteredObjects[index];
        
        //gridScript.CheckWalkable(selectedObject);
        if (!gridScript.CheckWalkable(selectedObject))
        {
            //if the picked object is not walkable -> pick another interaction
            currentInteraction = null;
            return;
        }

        //pick a random interaction
        int interactionIndex = Random.Range(0, selectedObject.Interations.Count);
        BaseInteraction selectedInteraction = selectedObject.Interations[interactionIndex];

        //can perform the interaction
        if (selectedInteraction.CanPerform())
        {
            currentInteraction = selectedInteraction;
            currentInteraction.LockInteraction();

            //request path
            Debug.Log($"Going to {currentInteraction.DisplayName} at {selectedObject.DisplayName}");
            PathRequestManager.RequestPath(transform.position, selectedObject.InteractionPoint, pathScript.OnPathFound);
        }
    }

}
