using UnityEngine;

public class RotateWithSlider : MonoBehaviour
{
    [Header("Angle maximum (ex: 180 pour faire un demi-tour)")]
    public float maxAngle = 180f;

    // Cette fonction sera appelée automatiquement par le Slider
    public void UpdateRotationY(float sliderValue)
    {
        // On convertit la valeur du slider (0 à 1) en angle (-maxAngle à +maxAngle)
        float currentAngle = Mathf.Lerp(-maxAngle, maxAngle, sliderValue);

        // On applique la rotation sur l'axe Y tout en gardant les axes X et Z intacts
        transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, currentAngle, transform.localEulerAngles.z);
    }
}