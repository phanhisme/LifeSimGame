using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : BaseAI
{

    [SerializeField] protected float PickInteractionInterval = 2f;
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
                PickRandomInteraction();
            }
        }
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
                TimeUntilNextInteractionPick = 1;
                return;
            }
        }

        //can perform the interaction (number of user not exceed the limit)
        if (selectedInteraction.CanPerform())
        {
            currentInteraction = selectedInteraction;
            currentInteraction.LockInteraction(); //+1 number of user to the object

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