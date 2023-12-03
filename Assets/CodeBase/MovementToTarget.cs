using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TargetSetter))]

public class MovementToTarget : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _speedRotation;    
    
    private TargetSetter _targetSetter;
    private Vector3 _targetPosition;
    private Coroutine _moveToTargetJob;
    private float _minDistanceToTargetForAction;
    private bool _isTargetNotReached;

    public event UnityAction TargetReached;    

    private void Awake() => 
        _targetSetter = GetComponent<TargetSetter>();

    private void OnEnable() => 
        _targetSetter.TargetSet += OnTargetSet;

    private void OnDisable() => 
        _targetSetter.TargetSet -= OnTargetSet;

    private void OnTargetSet(Transform target, float minDistanceToTargetForAction)
    {
        _minDistanceToTargetForAction = minDistanceToTargetForAction;
        StartMoving(target);
    }    

    public void StartMoving(Transform target)
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
                _isTargetNotReached = false;

                if (_moveToTargetJob != null)
                    StopCoroutine(_moveToTargetJob);

                TargetReached?.Invoke();
            }

            yield return null;
        }
    }    
}