using UnityEngine;

using TMPro;

using System;
using System.Globalization;

/// <summary>
/// This static class contains tools functions.
/// </summary>
public static class UIToolkit
{
    // --- NOUVELLES FONCTIONS UNIVERSELLES POUR LE TEXTE ---

    /// <summary>
    /// Met à jour un texte avec une chaîne de caractères simple.
    /// </summary>
    public static void SetText(GameObject @object, string str)
    {
        // assertion de l'existence de l'objet
        Debug.Assert(@object);

        if (@object.TryGetComponent<TMP_Text>(out TMP_Text component))
        {
            component.SetText(str);
        } 
    }

    /// <summary>
    /// Met à jour un texte directement avec un nombre (float ou int).
    /// Tu peux préciser le format : "0" (sans virgule), "F1" (1 chiffre après la virgule), etc.
    /// </summary>
    public static void SetFormattedText(GameObject @object, IFormattable value, string format)
        => SetFormattedText<IFormattable>(@object, value, format);

    /// <summary>
    /// Met à jour un texte directement avec un nombre (float ou int).
    /// Tu peux préciser le format : "0" (sans virgule), "F1" (1 chiffre après la virgule), etc.
    /// </summary>
    public static void SetFormattedText<T>(GameObject @object, T value, string format) where T : IFormattable
    {
        Debug.Assert(@object);

        if (@object.TryGetComponent<TMP_Text>(out TMP_Text component))
        {
            component.SetText(value.ToString(format, CultureInfo.CurrentCulture));
        }
    }

    // --- FONCTIONS CLASSIQUES DES CANVAS ---

    public static void CloseCanvas(CanvasGroup canvas)
    {
        Debug.Assert(canvas);

        // set visibility to false
        canvas.alpha = 0.0f;
    }

    public static void CloseCanvas(Component canvas)
    {
        Debug.Assert(canvas);

        // set visibility to false
        canvas.gameObject.SetActive(false);
    }

    public static void OpenCanvas(CanvasGroup canvas)
    {
        Debug.Assert(canvas);

        // set visibiliyt to true
        canvas.alpha = 1.0f;
    }

    public static void OpenCanvas(Component canvas)
    {
        Debug.Assert(canvas);

        // set visibiliyt to true
        canvas.gameObject.SetActive(true);
    }

    public static void ToggleCanvas(CanvasGroup canvas)
    {
        Debug.Assert(canvas);

        // invert alpha
        canvas.alpha = Convert.ToSingle(!Convert.ToBoolean(canvas.alpha));
    }

    public static void ToggleCanvas(Component canvas)
    {
        Debug.Assert(canvas);

        // invert visibility
        canvas.gameObject.SetActive(canvas.gameObject.activeSelf);
    }
}