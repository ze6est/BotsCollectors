using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Unit : MonoBehaviour
{
    private static int Id;

    public int Number { get; private set; }    

    public float HeightCollider { get; private set; }

    private void Awake()
    {
        Id++;
        Number = Id;
        HeightCollider = GetComponent<CapsuleCollider>().height;
    }
}