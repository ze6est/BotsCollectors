using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourcesScanner : MonoBehaviour
{
    private const string FoundResource = "FoundResource";

    [SerializeField] private LayerMask _newResourcesMask;
    [SerializeField] private float _scanRadius;
    [SerializeField] private float _duration;

    private Dictionary<float, Resource> _foundResources = new Dictionary<float, Resource>();
    private Coroutine _scanResourceJob;
    private bool _isGameWorked;    

    private void Awake() => 
        _isGameWorked = true;

    private void OnEnable() =>
        _scanResourceJob = StartCoroutine(ScanResource());    

    private void OnDisable()
    {
        if (_scanResourceJob != null)
            StopCoroutine(_scanResourceJob);
    }

    public Resource GetResource()
    {
        float minDistanceToResource = int.MaxValue;
        Resource currentResource = null;

        foreach (float distanse in _foundResources.Keys)
        {
            if (minDistanceToResource > distanse)
            {
                minDistanceToResource = distanse;
                currentResource = _foundResources[minDistanceToResource];
            }
        }

        _foundResources.Remove(minDistanceToResource);

        return currentResource;
    }

    private IEnumerator ScanResource()
    {
        WaitForSeconds waitTime = new WaitForSeconds(_duration);

        while (_isGameWorked)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _scanRadius, _newResourcesMask);
            int length = colliders.Length;

            if(length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    if (colliders[i].TryGetComponent(out Resource resource))
                    {
                        float distanse = Vector3.Distance(transform.position, resource.transform.position);

                        _foundResources.Add(distanse, resource);

                        resource.gameObject.layer = LayerMask.NameToLayer(FoundResource);
                    }
                }
            }

            yield return waitTime;
        }
    }
}