using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Unit : MonoBehaviour
{
    public float HeightCollider { get; private set; }

    private void Awake() => 
        HeightCollider = GetComponent<CapsuleCollider>().height;
}