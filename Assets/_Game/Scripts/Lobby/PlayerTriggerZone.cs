using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PlayerTriggerZone : MonoBehaviour
{
    [Serializable]
    public class PlayerEvent : UnityEvent<SlotIdentifier> { }

    [SerializeField] private PlayerEvent onPlayerEnter;
    [SerializeField] private PlayerEvent onPlayerExit;

    private Collider _zoneCollider;

    private void OnEnable()
    {
        if (_zoneCollider == null)
        {
            _zoneCollider = GetComponent<Collider>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SlotIdentifier>(out var slot))
        {
            onPlayerEnter?.Invoke(slot);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<SlotIdentifier>(out var slot))
        {
            onPlayerExit?.Invoke(slot);
        }
    }

    private void OnDrawGizmos()
    {
        if (_zoneCollider == null)
        {
            return;
        }

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;

        switch (_zoneCollider)
        {
            case BoxCollider box:
                Gizmos.DrawCube(box.center, box.size);
                break;
            case SphereCollider sphere:
                Gizmos.DrawSphere(sphere.center, sphere.radius);
                break;
            case CapsuleCollider capsule:
                Gizmos.DrawCube(capsule.center, new Vector3(capsule.radius * 2f, capsule.height, capsule.radius * 2f));
                break;
        }
    }
}
