using UnityEngine;

public class AntennaTuner : MonoBehaviour
{
    [Header("Rotation de l'Antenne")]
    public float maxAngle = 180f;

    [Header("Zone Cible (Inspecteur)")]
    [Range(-180f, 180f)] public float targetAngle = 45f; // L'angle parfait (#Tape dans le fond c pas ta m�re)
    public float zoneTolerance = 5f; // La marge d'erreur (Ta un peu rat� le fond)
    public float maxDistanceForRed = 90f; // La distance � partir de laquelle c'est 100% rouge (Tu t tromp� de trou)

    [Header("Visuel")]
    public MeshRenderer targetRenderer; // L'objet qui va changer de couleur (Ta tes r�gles ou quoi?! #SexistePasSexy)
    public RadioBehaviour RadioBehaviour;
    
    private Material m_materialInstance;

    void Start()
    {
        NullComponents.ThrowIfNull(RadioBehaviour);

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
        // 1. On calcule l'�cart
        float distance = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
        float hue;
        float signalQuality; // NOUVEAU : On pr�pare la variable de qualit� du son

        if (distance <= zoneTolerance)
        {
            hue = 0.33f; // Vert
            signalQuality = 1f; // Son parfait (100%)
        }
        else
        {
            // On calcule le d�grad� (0 = proche du vert, 1 = rouge vif)
            float t = Mathf.InverseLerp(zoneTolerance, maxDistanceForRed, distance);
            hue = Mathf.Lerp(0.33f, 0f, t);

            // NOUVEAU : La qualit� du son est l'inverse de la couleur (Si t=1, le signal est � 0)
            signalQuality = 1f - t;
        }

        // On envoie la qualit� du son � la radio !
        RadioBehaviour.AntennaSignalQuality = signalQuality;

        // On applique la couleur
        if (m_materialInstance != null)
        {
            m_materialInstance.color = Color.HSVToRGB(hue, 1f, 1f);
        }
    }
}