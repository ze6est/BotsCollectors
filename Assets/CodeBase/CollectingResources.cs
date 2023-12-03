using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TargetSetter))]
public class CollectingResources : MonoBehaviour
{
    [SerializeField] private float _liftingHeight;

    private TargetSetter _targetSetter;
    private Resource _resource;        

    public event UnityAction<Resource> ResourcePassed;       
    public event UnityAction<Unit> ResourceDelivered;

    private void Awake() => 
        _targetSetter = GetComponent<TargetSetter>();

    private void OnEnable()
    {
        _targetSetter.ResourceReached += OnResourceReached;
        _targetSetter.BaseReached += OnBaseReached;
    }

    private void OnDisable()
    {
        _targetSetter.ResourceReached -= OnResourceReached;
        _targetSetter.BaseReached -= OnBaseReached;
    }    

    public void CollectResource(Resource resource)
    {
        _resource = resource;
        ResourcePassed?.Invoke(_resource);
    }

    private void OnResourceReached()
    {
        _resource.transform.position += new Vector3(0, _liftingHeight, 0);
        _resource.transform.parent = transform;
    }

    private void OnBaseReached()
    {
        Unit unit = GetComponent<Unit>();        
        Destroy(_resource.gameObject);
        _resource = null;
        ResourceDelivered?.Invoke(unit);
    }
}