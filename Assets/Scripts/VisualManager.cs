using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
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
    Light globalLight;

    [Header("Light settings")]
    public Color lightColor = new Color(1,1,1);
    public Color darkColor = new Color(0.3f, 0f, 0.6f);
    public float transitionTime;
    float transitionFac = 0;
    float distortionFac = 0;
    Tween transition;
    bool midTransition => (transition != null && transition.active);
    
    [Header("Wobble Noise")]
    public Vector2 NFac;
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

    [Header("Glow materials")]
    [SerializeField]
    List<Material> glowMaterials;

    public Material ppMat => PostProcessingMaterial;

    private void OnEnable()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out lensDistortion);
        GameManager.instance.onSwitchLight += SwitchToLight;
        GameManager.instance.onSwitchDark += SwitchToDark;
        GameManager.instance.onGhostTimer += OnGhostTimer;
    }

    private void OnDisable()
    {
        GameManager.instance.onSwitchLight -= SwitchToLight;
        GameManager.instance.onSwitchDark -= SwitchToDark;
        GameManager.instance.onGhostTimer -= OnGhostTimer;
    }
    private void Start()
    {
        if (GameManager.instance.isLight)
            SwitchToLight();
        else
            SwitchToDark();
    }
    private void Update()
    {
        SetState(distortionFac * (1-transitionFac));

        if (midTransition)
        {
            globalLight.color = Color.Lerp(darkColor, lightColor, transitionFac);

            foreach (var mat in glowMaterials)
            {
                mat.SetFloat("_Alpha", 1-transitionFac);
            }
        }
    }
    public void OnGhostTimer(float time)
    {
        distortionFac = time/GameManager.instance.ghostTimeTotal;
    }
    [ContextMenu("Reset Postprocessing Material")]
    public void ResetState()
    {
        SetState(0);
    }

    [ContextMenu("Set Full Distortion")]
    public void SetFull()
    {
        SetState(1);
    }
    public void SetState(float norm)
    {
        if (vignette == null)
            volume.profile.TryGet(out vignette);
        if (lensDistortion == null)
            volume.profile.TryGet(out lensDistortion);

        vignette.intensity.Override(Mathf.Lerp(VIntensity.x, VIntensity.y, norm));
        lensDistortion.intensity.Override(Mathf.Lerp(LDIntensity.x, LDIntensity.y, norm));

        ppMat.SetFloat("_Noise_Intensity", Mathf.Lerp(NFac.x, NFac.y, Mathf.Clamp01(norm*2-1)));
        ppMat.SetFloat("_White_Noise_Fac", Mathf.Lerp(WNFac.x, WNFac.y, norm));
        ppMat.SetFloat("_Blinding_Fac", Mathf.Lerp(BIntensity.x, BIntensity.y, norm));
        ppMat.SetFloat("_Blinding_Radius", Mathf.Lerp(BRadius.x, BRadius.y, norm));
        ppMat.SetVector("_Blinding_Clamp", Vector2.Lerp(BClampStart, BClampEnd, norm));
        ppMat.SetFloat("_Gray_Fac", Mathf.Lerp(GIntensity.x, GIntensity.y, norm));
        ppMat.SetFloat("_Gray_Radius", Mathf.Lerp(GRadius.x, GRadius.y, norm));
        ppMat.SetVector("_Gray_Clamp", Vector2.Lerp(GClampStart, GClampEnd, norm));
    }

    public void SwitchToLight()
    {
        Debug.Log("Switching to light");
        if (midTransition)
            transition.Kill();
        transition = DOTween.To(() => transitionFac, x => transitionFac = x, 1f, transitionTime);
    }
    public void SwitchToDark()
    {
        Debug.Log("Switching to dark");
        if (midTransition)
            transition.Kill();
        transition = DOTween.To(() => transitionFac, x => transitionFac = x, 0f, transitionTime);
    }
}
