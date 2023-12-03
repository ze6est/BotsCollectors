using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<Unit> _unitsFree = new List<Unit>();
    private List<Unit> _unitsOccupied = new List<Unit>();
    private Resource _currentResource;
    private Coroutine _trySendUnitForNearestResourceJob;    
    private bool _isGameWorked;

    public float Radius { get; private set; }

    public event UnityAction ResourceCollected;

    private void Awake()
    {
        _unitSpawner = GetComponent<UnitSpawner>();
        _resourcesScanner = GetComponent<ResourcesScanner>();
        Radius = GetComponent<CapsuleCollider>().radius;

        _isGameWorked = true;
    }

    private void OnEnable() => 
        _unitSpawner.UnitCreated += OnUnitCreated;

    private void Start() => 
        _trySendUnitForNearestResourceJob = StartCoroutine(TrySendUnitForNearestResource());

    private void OnDisable()
    {
        _unitSpawner.UnitCreated -= OnUnitCreated;

        if(_trySendUnitForNearestResourceJob != null)
            StopCoroutine(_trySendUnitForNearestResourceJob);
    }

    private void OnUnitCreated(Unit unit) => 
        _unitsFree.Add(unit);

    private IEnumerator TrySendUnitForNearestResource()
    {
        WaitForSeconds waitTime = new WaitForSeconds(_resourceCollectionDuration);

        while (_isGameWorked)
        {
            yield return waitTime;

            if (_currentResource == null)
                _currentResource = _resourcesScanner.GetResource();

            if (_unitsFree.Count > 0 && _currentResource != null)
            {
                Unit unit = _unitsFree.First();
                _unitsOccupied.Add(unit);
                _unitsFree.Remove(unit);

                CollectingResources collectingResources = unit.GetComponent<CollectingResources>();
                collectingResources.ResourceDelivered += OnResourceDelivered;
                collectingResources.CollectResource(_currentResource);

                _currentResource.gameObject.layer = LayerMask.NameToLayer(SelectedResource);
                _currentResource = null;
            }
        }
    }

    private void OnResourceDelivered(Unit acceptedUnit)
    { 
        Unit freeUnit = _unitsOccupied.First(unit => unit.Number == acceptedUnit.Number);

        CollectingResources collectingResources = freeUnit.GetComponent<CollectingResources>();

        _unitsFree.Add(freeUnit);
        _unitsOccupied.Remove(freeUnit);

        ResourceCollected?.Invoke();

        collectingResources.ResourceDelivered -= OnResourceDelivered;        
    }
}