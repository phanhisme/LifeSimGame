using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    Units pathScript;
    Pathfinding pathfindingScript;

    protected BaseInteraction currentInteraction = null;
    
    [SerializeField] protected float PickInteractionInterval = 2f;

    protected float TimeUntilNextInteractionPick = -1;

    private void Awake()
    {
        pathScript = GetComponent<Units>();
        pathfindingScript = FindObjectOfType<Pathfinding>();
    }

    private void Update()
    {
        if(currentInteraction!=null&&pathfindingScript.isWalkable)
        currentInteraction.Perform(this, OnInteractionbFinished);
    }

    private void OnInteractionbFinished(BaseInteraction interaction)
    {
        interaction.UnlockInteraction();
        currentInteraction = null;
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
            if (!pathfindingScript.isWalkable)
            {
                Debug.LogError($"Could not move to {selectedObject.name}");
                currentInteraction = null;
            }


            //request path here
            PathRequestManager.RequestPath(transform.position, selectedObject.gameObject.transform.position, pathScript.OnPathFound);

        }
    }
}
