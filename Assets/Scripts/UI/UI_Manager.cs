using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- NOUVELLES FONCTIONS UNIVERSELLES POUR LE TEXTE ---

    /// <summary>
    /// Met à jour un texte avec une chaîne de caractères simple.
    /// </summary>
    public void UpdateText(TMP_Text textComponent, string newText)
    {
        if (textComponent != null)
        {
            textComponent.text = newText;
        }
        else
        {
            Debug.LogWarning("[UI_Manager] Tu as essayé de mettre à jour un texte, mais le composant TMP n'est pas assigné dans l'inspecteur !");
        }
    }

    /// <summary>
    /// Met à jour un texte directement avec un nombre (float ou int).
    /// Tu peux préciser le format : "0" (sans virgule), "F1" (1 chiffre après la virgule), etc.
    /// </summary>
    public void UpdateText(TMP_Text textComponent, float newValue, string format = "0")
    {
        if (textComponent != null)
        {
            textComponent.text = newValue.ToString(format);
        }
        else
        {
            Debug.LogWarning("[UI_Manager] Tu as essayé de mettre à jour un texte numérique, mais le composant TMP n'est pas assigné !");
        }
    }

    // --- FONCTIONS CLASSIQUES DES CANVAS ---

    public void CloseCanvas(GameObject canvasToClose)
    {
        if (canvasToClose != null)
        {
            canvasToClose.SetActive(false);
        }
    }

    public void OpenCanvas(GameObject canvasToOpen)
    {
        if (canvasToOpen != null)
        {
            canvasToOpen.SetActive(true);
        }
    }

    public void ToggleCanvas(GameObject canvasToToggle)
    {
        if (canvasToToggle != null)
        {
            canvasToToggle.SetActive(!canvasToToggle.activeSelf);
        }
    }
}