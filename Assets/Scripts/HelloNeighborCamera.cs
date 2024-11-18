using UnityEngine;

[ExecuteInEditMode]
public class HelloNeighborCamera : MonoBehaviour
{
    [SerializeField] private Material postProcessMaterial;
    
    [Header("Color Adjustments")]
    [Range(0f, 2f)]
    public float saturation = 1.2f;
    [Range(0f, 2f)]
    public float contrast = 1.1f;
    [Range(0f, 1f)]
    public float warmth = 0.3f;

    [Header("Effects")]
    [Range(0f, 1f)]
    public float vignetteIntensity = 0.3f;
    [Range(0f, 1f)]
    public float vignetteRoundness = 0.5f;
    [Range(0f, 0.01f)]
    public float chromaticAberration = 0.002f;
    [Range(0f, 0.01f)]
    public float blurAmount = 0.001f;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postProcessMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        postProcessMaterial.SetFloat("_Saturation", saturation);
        postProcessMaterial.SetFloat("_Contrast", contrast);
        postProcessMaterial.SetFloat("_Warmth", warmth);
        postProcessMaterial.SetFloat("_VignetteIntensity", vignetteIntensity);
        postProcessMaterial.SetFloat("_VignetteRoundness", vignetteRoundness);
        postProcessMaterial.SetFloat("_ChromaticAberration", chromaticAberration);
        postProcessMaterial.SetFloat("_BlurAmount", blurAmount);

        Graphics.Blit(source, destination, postProcessMaterial);
    }
} 