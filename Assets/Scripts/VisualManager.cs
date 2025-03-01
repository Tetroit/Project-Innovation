using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VisualManager : MonoBehaviour
{
    [SerializeField]
    Camera cam;
    [SerializeField]
    Volume volume;
    Vignette vignette;
    LensDistortion lensDistortion;
    [SerializeField]
    Material PostProcessingMaterial;

    [SerializeField]
    float ghostTimer;
    [SerializeField]
    public float ghostTime;

    [SerializeField]
    [Header("White Noise")]
    public Vector2 WNFac;
    [Header("Vignette")]
    public Vector2 VIntensity;
    [Header("Lens Distortion")]
    public Vector2 LDIntensity;

    [Header("Blind")]
    public Vector2 BIntensity;
    public Vector2 BRadius;
    public Vector2 BClampStart;
    public Vector2 BClampEnd;

    [Header("Grayscale")]
    public Vector2 GIntensity;
    public Vector2 GRadius;
    public Vector2 GClampStart;
    public Vector2 GClampEnd;

    public Material ppMat => PostProcessingMaterial;

    private void OnEnable()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out lensDistortion);
    }
    private void Update()
    {
        SetState(Mathf.Repeat(ghostTimer, ghostTime));
    }

    public void SetState(float time)
    {
        float norm = time / ghostTime;
        if (time < 0.0f) time = 0.0f;

        vignette.intensity.Override(Mathf.Lerp(VIntensity.x, VIntensity.y, norm));
        lensDistortion.intensity.Override(Mathf.Lerp(LDIntensity.x, LDIntensity.y, norm));

        ppMat.SetFloat("_White_Noise_Fac", Mathf.Lerp(WNFac.x, WNFac.y, norm));
        ppMat.SetFloat("_Blinding_Fac", Mathf.Lerp(BIntensity.x, BIntensity.y, norm));
        ppMat.SetFloat("_Blinding_Radius", Mathf.Lerp(BRadius.x, BRadius.y, norm));
        ppMat.SetVector("_Blinding_Clamp", Vector2.Lerp(BClampStart, BClampEnd, norm));
        ppMat.SetFloat("_Gray_Fac", Mathf.Lerp(GIntensity.x, GIntensity.y, norm));
        ppMat.SetFloat("_Gray_Radius", Mathf.Lerp(GRadius.x, GRadius.y, norm));
        ppMat.SetVector("_Gray_Clamp", Vector2.Lerp(GClampStart, GClampEnd, norm));
    }
}
