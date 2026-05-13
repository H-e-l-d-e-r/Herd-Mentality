using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class AntennaTunerBehaviour : RadioComponentBehaviour<float>
{
    [Header("Components")]
    public Slider Slider;
    public Transform Antenna;

    [Header("Parameters")]
    public float MaxAngle;

    [Range(0f, 1f)]
    public float Smoothing = 0.5f;

    void Start()
    {
        Slider.onValueChanged.AddListener((value) => SetValue(value));

        SetValue(Default);
        Slider.value = 0.5f + (Default / MaxAngle);
    }

    public override void SetValue(float value)
    {
        value = Mathf.Lerp(-MaxAngle, MaxAngle, value);
        
        Antenna.rotation = Quaternion.Euler(
            Antenna.rotation.eulerAngles.x,
            value,
            Antenna.rotation.eulerAngles.z
        );

        base.SetValue(value);
    }
}