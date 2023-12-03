using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Base))]
public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Unit _unit;

    private Base _base;
    private int _countUnits;

    public event UnityAction<Unit> UnitCreated;

    private void Awake()
    {
        _countUnits = transform.childCount;
        _base = GetComponent<Base>();
    }

    private void Start()
    {
        for (int number = 0; number < _countUnits; number++)        
            SpawnUnit(number);        
    }

    private void SpawnUnit(int number)
    {
        Unit unit = Instantiate(_unit);

        float spawnHeight = unit.HeightCollider / 2;
        Vector3 spawnPosition = new Vector3(transform.GetChild(number).position.x, spawnHeight, transform.GetChild(number).position.z);
        unit.transform.position = spawnPosition;        

        TargetSetter targetSetter = unit.GetComponent<TargetSetter>();
        targetSetter.Construct(_base);

        UnitCreated?.Invoke(unit);
    }
}