using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class AntennaTuner : MonoBehaviour
{
    [Header("Rotation de l'Antenne")]
    public float maxAngle = 180f;

    [Header("Zone Cible (Inspecteur)")]
    [Range(-180f, 180f)] public float targetAngle = 45f;// L'angle parfait (#Tape dans le fond c pas ta m�re)
    public float zoneTolerance = 5f;// La marge d'erreur (Ta un peu rat� le fond)
    public float maxDistanceForRed = 90f;// La distance � partir de laquelle c'est 100% rouge (Tu t tromp� de trou)

    [Header("Visuel & UI")]
    public MeshRenderer targetRenderer;
    public RadioBehaviour RadioBehaviour;
    public Slider AntennaSlider; // NOUVEAU : La reference a ton slider UI

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
            Debug.Log("<color=cyan>[ANTENNE] Coup de vent ! L'antenne a bouge.</color>");

            m_currentSliderValue = Mathf.Clamp01(m_currentSliderValue + windShift);

            // On met a jour la rotation 3D et les couleurs
            UpdateRotationY(m_currentSliderValue);

            // On force le slider UI a bouger tout seul, 
            
            if (AntennaSlider != null)
            {
                AntennaSlider.SetValueWithoutNotify(m_currentSliderValue);
            }
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