using UnityEngine;

public static class UI_Manager
{
    // Fonction ultra flexible : tu lui donnes un objet (Canvas/Panel), elle l'�teint.
    public static void CloseCanvas(GameObject canvasToClose)
    {
        if (canvasToClose != null)
        {
            canvasToClose.SetActive(false);
        }
        else
        {
            Debug.LogWarning(" Tu as oubli� d'assigner le Canvas � fermer dans le bouton !");
        }
    }

    // Bonus : l'inverse, pour l'ouvrir !
    public static void OpenCanvas(GameObject canvasToOpen)
    {
        if (canvasToOpen != null)
        {
            canvasToOpen.SetActive(true);
        }
    }

    // Re-bonus : Un bouton qui fait les deux (Ouvre si c'est ferm�, ferme si c'est ouvert)
    public static void ToggleCanvas(GameObject canvasToToggle)
    {
        if (canvasToToggle != null)
        {
            // LE MOUCHARD : Il va afficher le nom EXACT de ce qu'il �teint dans la console
            Debug.Log("Le script UI_Manager vient de basculer l'objet : " + canvasToToggle.name);

            canvasToToggle.SetActive(!canvasToToggle.activeSelf);
        }
    }
}