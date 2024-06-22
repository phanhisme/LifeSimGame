using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartObject : MonoBehaviour
{
    public int itemID;

    public bool isWalkable;

    [SerializeField] protected string _DisplayName;
    [SerializeField] protected Transform _InteractionMarker;
    protected List<BaseInteraction> CachedInteractions = null;

    public Vector3 InteractionPoint => _InteractionMarker != null ? _InteractionMarker.position : transform.position;

    public string DisplayName => _DisplayName;
    public List<BaseInteraction> Interations
    {
        get
        {
            if (CachedInteractions == null)
                CachedInteractions = new List<BaseInteraction>(GetComponents<BaseInteraction>());

            return CachedInteractions;
        }
    }

    private void Start()
    {
        SmartObjectManager.Instance.RegisterSmartObject(this);
    }

    private void OnDestroy()
    {
        SmartObjectManager.Instance.DeregisterSmartObject(this);
    }
}
