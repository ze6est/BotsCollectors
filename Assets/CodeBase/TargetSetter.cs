using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CollectingResources))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(MovementToTarget))]
public class TargetSetter : MonoBehaviour
{
    [SerializeField] private float _offsetToTarget;    

    private CollectingResources _collectingResources;
    private MovementToTarget _movementToTarget;
    private Resource _resource;
    private Base _base;
    private float _radius;
    private float _resourceRadius;
    private float _minDistanceToTargetForAction;
    private bool _isResource;

    public event UnityAction<Transform, float> TargetSet;
    public event UnityAction ResourceReached;
    public event UnityAction BaseReached;

    public void Construct(Base @base) =>
        _base = @base;

    private void Awake()
    {
        _collectingResources = GetComponent<CollectingResources>();
        _movementToTarget = GetComponent<MovementToTarget>();
        _radius = GetComponent<CapsuleCollider>().radius;
    }

    private void OnEnable()
    {
        _collectingResources.ResourcePassed += OnResourcePassed;        
        _movementToTarget.TargetReached += OnTargetReached;
    }

    private void OnDisable()
    {
        _collectingResources.ResourcePassed -= OnResourcePassed;        
        _movementToTarget.TargetReached -= OnTargetReached;
    }    

    private void OnResourcePassed(Resource resource)
    {
        _isResource = true;
        _resource = resource;
        _resourceRadius = _resource.Radius;
        _minDistanceToTargetForAction = _resourceRadius + _radius + _offsetToTarget;

        TargetSet?.Invoke(resource.transform, _minDistanceToTargetForAction);
    }

    private void OnTargetReached()
    {
        if(_isResource)
        {
            ResourceReached?.Invoke();            
            _minDistanceToTargetForAction += _base.Radius + _resourceRadius + _offsetToTarget;
            TargetSet?.Invoke(_base.transform, _minDistanceToTargetForAction);

            _isResource = false;
        }
        else
            BaseReached?.Invoke();
    }    
}