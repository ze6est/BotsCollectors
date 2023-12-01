using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UnitSpawner))]
[RequireComponent(typeof(ResourcesScanner))]
[RequireComponent(typeof(CapsuleCollider))]
public class Base : MonoBehaviour
{
    private const string SelectedResource = "SelectedResource";

    [SerializeField] private float _resourceCollectionDuration;

    private UnitSpawner _unitSpawner;
    private ResourcesScanner _resourcesScanner;
    private Unit[] _units;
    private Dictionary<float, Resource> _foundResources = new Dictionary<float, Resource>();
    private Coroutine _trySendUnitForNearestResourceJob;
    private int _currentUnitNumber;
    private bool _isGameWorked;

    public float Radius { get; private set; }

    public event UnityAction ResourceCollected;

    private void Awake()
    {
        int countUnits = transform.childCount;
        _units = new Unit[countUnits];

        _unitSpawner = GetComponent<UnitSpawner>();
        _resourcesScanner = GetComponent<ResourcesScanner>();
        Radius = GetComponent<CapsuleCollider>().radius;

        _isGameWorked = true;
    }

    private void OnEnable()
    {
        _unitSpawner.UnitCreated += OnUnitCreated;
        _resourcesScanner.ResourceFound += OnResourceFound;
        _trySendUnitForNearestResourceJob = StartCoroutine(TrySendUnitForNearestResource());
    }    

    private void OnDisable()
    {
        _unitSpawner.UnitCreated -= OnUnitCreated;
        _resourcesScanner.ResourceFound -= OnResourceFound;
        StopCoroutine(_trySendUnitForNearestResourceJob);
    }

    private void OnDestroy()
    {
        foreach (Unit unit in _units)
        {
            if(unit != null)
            {
                CollectingResources collectingResources = unit.GetComponent<CollectingResources>();
                collectingResources.ResourceDelivered -= OnResourceDelivered;
            }            
        }
    }

    private void OnUnitCreated(Unit unit)
    {
        CollectingResources collectingResources = unit.GetComponent<CollectingResources>();
        collectingResources.ResourceDelivered += OnResourceDelivered;
        _units[_currentUnitNumber] = unit;
        _currentUnitNumber++;
    }

    private void OnResourceFound(float distanse, Resource resource) => 
        _foundResources.Add(distanse, resource);

    private IEnumerator TrySendUnitForNearestResource()
    {
        WaitForSeconds waitTime = new WaitForSeconds(_resourceCollectionDuration);

        while (_isGameWorked)
        {
            if(_foundResources.Count > 0)
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

                foreach (Unit unit in _units)
                {
                    CollectingResources collectingResources = unit.GetComponent<CollectingResources>();

                    if (collectingResources.TryCollectResource(currentResource))
                    {                        
                        currentResource.gameObject.layer = LayerMask.NameToLayer(SelectedResource);

                        _foundResources.Remove(minDistanceToResource);

                        break;
                    }
                }             
            }

            yield return waitTime;
        }
    }

    private void OnResourceDelivered() => 
        ResourceCollected?.Invoke();
}