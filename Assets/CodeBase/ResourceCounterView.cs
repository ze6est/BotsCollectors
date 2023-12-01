using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ResourceCounterView : MonoBehaviour
{
    [SerializeField] private ResourceCounter _resourceCounter;    

    private TextMeshProUGUI _countResourcesHUD;

    private void OnValidate() => 
        _countResourcesHUD = GetComponent<TextMeshProUGUI>();

    private void OnEnable() => 
        _resourceCounter.CountResourcesChanged += OnCountResourcesChanged;
    
    private void OnDisable() => 
        _resourceCounter.CountResourcesChanged -= OnCountResourcesChanged;

    private void OnCountResourcesChanged(int countResources) => 
        RefreshText(countResources);

    private void RefreshText(int countResources) => 
        _countResourcesHUD.text = countResources.ToString();
}