using UnityEngine;
using UnityEngine.UI;

public class CustomLoadingBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _loadingBarSprite; 
    [Range(0, 1)] public float fillAmount = 0.5f;

    // Must match the Reference name in Shader Graph
    private static readonly int FillProperty = Shader.PropertyToID("_FillAmount");
    
    private Material _dynamicMaterial;

    void Start()
    {
        // We create a clone of the material so we don't change 
        // the fill amount for EVERY object using this material.
        _dynamicMaterial = new Material(_loadingBarSprite.material);
        _loadingBarSprite.material = _dynamicMaterial;
    }

    void Update()
    {
        // Update the shader value
        if (_dynamicMaterial != null && SceneLoader.Instance != null)
        {
            _dynamicMaterial.SetFloat(FillProperty, 1 - SceneLoader.Instance.CurrentLoadingProgress);
        }
    }
}
