using System.Collections;
using UnityEngine;

public class AntennaTuner : MonoBehaviour
{
    [Header("Rotation de l'Antenne")]
    public float maxAngle = 180f;

    [Header("Zone Cible (Inspecteur)")]
    [Range(-180f, 180f)] public float targetAngle = 45f;
    public float zoneTolerance = 5f;
    public float maxDistanceForRed = 90f;

    [Header("Visuel")]
    public MeshRenderer targetRenderer;
    public RadioBehaviour RadioBehaviour;

    [Header("Systeme de Vent")]
    public bool windEnabled = true;
    public float minWindInterval = 10f;
    public float maxWindInterval = 30f;
    public float maxWindForce = 0.2f;

    private Material m_materialInstance;
    private float m_currentSliderValue = 0.5f;

    void Start()
    {
        NullComponents.ThrowIfNull(RadioBehaviour);

        if (targetRenderer != null)
            m_materialInstance = targetRenderer.material;

        if (windEnabled) StartCoroutine(WindSimulation());
    }

    IEnumerator WindSimulation()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWindInterval, maxWindInterval));

            float windShift = Random.Range(-maxWindForce, maxWindForce);
            Debug.Log("<color=cyan>VENT : L'antenne a bouge !</color>");

            m_currentSliderValue = Mathf.Clamp01(m_currentSliderValue + windShift);
            UpdateRotationY(m_currentSliderValue);

            // Si tu as un objet UI Slider, n'oublie pas de le mettre a jour ici pour que le visuel suive !
        }
    }

    public void UpdateRotationY(float sliderValue)
    {
        m_currentSliderValue = sliderValue;
        float currentAngle = Mathf.Lerp(-maxAngle, maxAngle, sliderValue);
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, currentAngle, transform.localEulerAngles.z);

        UpdateColorFeedback(currentAngle);
    }

    private void UpdateColorFeedback(float currentAngle)
    {
        float distance = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
        float hue;
        float signalQuality;

        if (distance <= zoneTolerance)
        {
            hue = 0.33f;
            signalQuality = 1f;
        }
        else
        {
            float t = Mathf.InverseLerp(zoneTolerance, maxDistanceForRed, distance);
            hue = Mathf.Lerp(0.33f, 0f, t);
            signalQuality = 1f - t;
        }

        RadioBehaviour.AntennaSignalQuality = signalQuality;

        if (m_materialInstance != null)
        {
            m_materialInstance.color = Color.HSVToRGB(hue, 1f, 1f);
        }
    }
}