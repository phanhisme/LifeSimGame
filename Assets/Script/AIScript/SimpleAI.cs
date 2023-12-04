using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    Units pathScript;

    protected BaseInteraction currentInteraction = null;
    
    [SerializeField] protected float PickInteractionInterval = 2f;

    protected float TimeUntilNextInteractionPick = -1f;

    private void Awake()
    {
        pathScript = GetComponent<Units>();
    }

    private void Update()
    {
        if (currentInteraction != null)
        {
            if (pathScript.atTargetPosition)
            {
                //we are the performer, allow the interaction to run
                currentInteraction.Perform(this, OnInteractionbFinished);
            }
        }

        else
        {
            //if the AI is not at the target position, negative +
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

        Debug.Log($"Finishjed {interaction.name}");
    }

    private void PickRandomInteraction()
    {
        //pick an randoom object
        int index = Random.Range(0, SmartObjectManager.Instance.RegisteredObjects.Count);
        SmartObject selectedObject = SmartObjectManager.Instance.RegisteredObjects[index];

        //pick a random interaction
        int interactionIndex = Random.Range(0, selectedObject.Interations.Count);
        BaseInteraction selectedInteraction = selectedObject.Interations[interactionIndex];

        //can perform the interaction
        if (selectedInteraction.CanPerform())
        {
            currentInteraction = selectedInteraction;
            currentInteraction.LockInteraction();

            //if the target node is not walkable
            if (!selectedObject.isWalkable)
            {
                Debug.LogError($"Could not move to {selectedObject.name}");
                currentInteraction = null;
            }
            else
            {
                //request path here
                PathRequestManager.RequestPath(transform.position, selectedObject.gameObject.transform.position, pathScript.OnPathFound);
                Debug.Log($"Going to {currentInteraction.name} at {selectedObject.name}");
            }
        }
    }
}
