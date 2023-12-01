using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MovementToTarget))]
public class CollectingResources : MonoBehaviour
{
    [SerializeField] private float _liftingHeight;

    private MovementToTarget _movementToTarget;
    private Resource _resource;
    private Base _base;
    private bool _isNotSent = true;

    public event UnityAction<Resource> ResourcePassed;
    public event UnityAction<Base> ResourceRaised;
    public event UnityAction ResourceDelivered;

    public void Construct(Base @base) =>
        _base = @base;

    private void Awake() => 
        _movementToTarget = GetComponent<MovementToTarget>();

    private void OnEnable()
    {
        _movementToTarget.ResourceReached += OnResourceReached;
        _movementToTarget.BaseReached += OnBaseReached;
    }

    private void OnDisable()
    {
        _movementToTarget.ResourceReached -= OnResourceReached;
        _movementToTarget.BaseReached -= OnBaseReached;
    }    

    private void OnResourceReached()
    {
        _resource.transform.position += new Vector3(0, _liftingHeight, 0);
        _resource.transform.parent = transform;
        ResourceRaised?.Invoke(_base);
    }

    private void OnBaseReached()
    {
        ResourceDelivered?.Invoke();
        Destroy(_resource.gameObject);
        _resource = null;

        _isNotSent = true;
    }

    public bool TryCollectResource(Resource resource)
    {
        if (_isNotSent)
        {
            _isNotSent = false;

            _resource = resource;
            ResourcePassed?.Invoke(_resource);

            return !_isNotSent;
        }
        else
            return _isNotSent;
    }    
}