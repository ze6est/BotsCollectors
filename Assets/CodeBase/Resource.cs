using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Resource : MonoBehaviour
{
    public float Radius { get; private set; }

    private void Awake() => 
        Radius = GetComponent<SphereCollider>().radius;
}