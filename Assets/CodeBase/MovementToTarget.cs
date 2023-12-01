using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CollectingResources))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovementToTarget : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _speedRotation;
    [SerializeField] private float _offsetToTarget;

    private CollectingResources _collectingResources;    
    private Vector3 _targetPosition;    
    private Coroutine _moveToTargetJob;    
    private float _radius;
    private float _resourceRadius;
    private float _minDistanceToTargetForAction;
    private bool _isTargetNotReached;

    public event UnityAction ResourceReached;
    public event UnityAction BaseReached;    

    private void Awake()
    {
        _collectingResources = GetComponent<CollectingResources>();
        _radius = GetComponent<CapsuleCollider>().radius;        
    }

    private void OnEnable()
    {
        _collectingResources.ResourcePassed += OnResourcePassed;
        _collectingResources.ResourceRaised += OnResourceRaised;
    }

    private void OnDisable()
    {
        _collectingResources.ResourcePassed -= OnResourcePassed;
        _collectingResources.ResourceRaised -= OnResourceRaised;
    }

    private void OnResourcePassed(Resource resource)
    {
        _resourceRadius = resource.Radius;
        _minDistanceToTargetForAction = _resourceRadius + _radius + _offsetToTarget;
        StartMoving(resource.transform);
    }

    private void OnResourceRaised(Base @base)
    {
        _minDistanceToTargetForAction += @base.Radius + _resourceRadius + _offsetToTarget;
        StartMoving(@base.transform);
    }

    private void StartMoving(Transform target)
    {
        _isTargetNotReached = true;
        _moveToTargetJob = StartCoroutine(MoveToTarget(target));
    }

    private IEnumerator MoveToTarget(Transform target) 
    {
        bool isResource = target.TryGetComponent(out Resource resource);        

        _targetPosition = target.position;

        while (_isTargetNotReached)
        {
            Vector3 targetPosition = new Vector3(_targetPosition.x, transform.position.y, _targetPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);

            Vector3 targetDirection = (targetPosition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _speedRotation * Time.deltaTime);            

            if (Vector3.Distance(_targetPosition, transform.position) <= _minDistanceToTargetForAction)
            {
                if(isResource)                
                    NotifyAboutReachingTarget(ResourceReached);                
                else                
                    NotifyAboutReachingTarget(BaseReached);
            }

            yield return null;
        }
    }

    private void NotifyAboutReachingTarget(UnityAction action)
    {
        _isTargetNotReached = false;

        if (_moveToTargetJob != null)
            StopCoroutine(_moveToTargetJob);

        action?.Invoke();
    }
}