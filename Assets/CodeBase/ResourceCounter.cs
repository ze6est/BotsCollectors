using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Base))]
public class ResourceCounter : MonoBehaviour
{
    [SerializeField] private int _countResourcesAtStart;

    private Base _base;
    private int _countCollectedResources;    

    public event UnityAction<int> CountResourcesChanged;

    private void OnValidate() => 
        _base = GetComponent<Base>();    

    private void OnEnable() => 
        _base.ResourceCollected += OnResourceCollected;

    private void Awake()
    {
        _countCollectedResources += _countResourcesAtStart;
        CountResourcesChanged?.Invoke(_countCollectedResources);
    }

    private void OnDisable() => 
        _base.ResourceCollected -= OnResourceCollected;

    private void OnResourceCollected()
    {
        _countCollectedResources++;
        CountResourcesChanged?.Invoke(_countCollectedResources);
    }
}