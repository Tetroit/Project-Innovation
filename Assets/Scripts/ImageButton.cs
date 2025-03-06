using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageButton : PuzzleElement
{
    public List<Material> icons;
    public int currentIcon = 0;
    MeshRenderer mr;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }
    public override void Move()
    {
        if (SensorInput.DeviceFound(SensorInput.touchscreenLayout))
            if (SensorInput.touchscreen.primaryTouch.press.wasPressedThisFrame)
            {
                SetIcon((currentIcon+1) % icons.Count);
            }
    }

    public override void OnSolved()
    {
    }
    public void SetIcon(int icon)
    {
        currentIcon = icon;
        mr.SetMaterials(new List<Material>() { icons[icon] });
    }
}
