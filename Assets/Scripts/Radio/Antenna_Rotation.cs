using UnityEngine;

public class AntennaTuner : MonoBehaviour
{
    [Header("Rotation de l'Antenne")]
    public float maxAngle = 180f;

    [Header("Zone Cible (Inspecteur)")]
    [Range(-180f, 180f)] public float targetAngle = 45f; // L'angle parfait (#Tape dans le fond c pas ta mère)
    public float zoneTolerance = 5f; // La marge d'erreur (Ta un peu raté le fond)
    public float maxDistanceForRed = 90f; // La distance à partir de laquelle c'est 100% rouge (Tu t trompé de trou)

    [Header("Visuel")]
    public MeshRenderer targetRenderer; // L'objet qui va changer de couleur (Ta tes règles ou quoi?! #SexistePasSexy)
    private Material m_materialInstance;

    void Start()
    {
        if (targetRenderer != null)
            m_materialInstance = targetRenderer.material;
    }


    public void UpdateRotationY(float sliderValue)
    {
        //  On tourne l'antenne
        float currentAngle = Mathf.Lerp(-maxAngle, maxAngle, sliderValue);
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, currentAngle, transform.localEulerAngles.z);

        //  On change la couleur
        UpdateColorFeedback(currentAngle);
    }

    private void UpdateColorFeedback(float currentAngle)
    {
        // 1. On calcule l'écart
        float distance = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
        float hue;
        float signalQuality; // NOUVEAU : On prépare la variable de qualité du son

        if (distance <= zoneTolerance)
        {
            hue = 0.33f; // Vert
            signalQuality = 1f; // Son parfait (100%)
        }
        else
        {
            // On calcule le dégradé (0 = proche du vert, 1 = rouge vif)
            float t = Mathf.InverseLerp(zoneTolerance, maxDistanceForRed, distance);
            hue = Mathf.Lerp(0.33f, 0f, t);

            // NOUVEAU : La qualité du son est l'inverse de la couleur (Si t=1, le signal est à 0)
            signalQuality = 1f - t;
        }

        // On envoie la qualité du son à la radio !
        if (RadioBehaviour.Instance != null)
        {
            RadioBehaviour.Instance.AntennaSignalQuality = signalQuality;
        }

        // On applique la couleur
        if (m_materialInstance != null)
        {
            m_materialInstance.color = Color.HSVToRGB(hue, 1f, 1f);
        }
    }
}