using System.Collections;
using UnityEngine;

public class ResourcesSpawner : MonoBehaviour
{
    [SerializeField] private Resource _resource;
    [SerializeField] private Transform _plane;
    [SerializeField] private LayerMask _interferencesMask;    
    [SerializeField] private float _duration;
    [SerializeField] private float _deadZone;
    [SerializeField] private float _marginToWidth;

    private Coroutine _spawnResourceJob;
    private float _extremePointX;
    private float _extremePointZ;
    private float _resourceRadius;
    private bool _isGameWorked;

    private void Awake()
    {
        _isGameWorked = true;
        Vector3 planeSize = _plane.GetComponent<Renderer>().bounds.size;

        _extremePointX = planeSize.x / 2 - _deadZone;
        _extremePointZ = planeSize.z / 2 - _deadZone;
        _resourceRadius = _resource.gameObject.GetComponent<SphereCollider>().radius;        
    }

    private void Start() => 
        _spawnResourceJob = StartCoroutine(SpawnResource());

    private void OnDisable()
    {
        if(_spawnResourceJob != null)
            StopCoroutine(_spawnResourceJob);
    }

    private IEnumerator SpawnResource()
    {
        WaitForSeconds waitTime = new WaitForSeconds(_duration);

        while (_isGameWorked)
        {
            Vector3 position = CheckPositionToFree();

            if(position != Vector3.zero)
                Instantiate(_resource, position, Quaternion.identity);

            yield return waitTime;
        }        
    }

    private Vector3 CheckPositionToFree()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(_extremePointX, -_extremePointX), _resourceRadius, Random.Range(_extremePointZ, -_extremePointZ));

        Collider[] hitColliders = new Collider[1];

        int count = Physics.OverlapSphereNonAlloc(spawnPosition, _resourceRadius + _marginToWidth, hitColliders, _interferencesMask);

        if (count == 0)
            return spawnPosition;
        else
            return Vector3.zero;        
    }
}