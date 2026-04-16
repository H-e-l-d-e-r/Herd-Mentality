using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    // Fonction ultra flexible : tu lui donnes un objet (Canvas/Panel), elle l'éteint.
    public void CloseCanvas(GameObject canvasToClose)
    {
        if (canvasToClose != null)
        {
            canvasToClose.SetActive(false);
        }
        else
        {
            Debug.LogWarning(" Tu as oublié d'assigner le Canvas à fermer dans le bouton !");
        }
    }

    // Bonus : l'inverse, pour l'ouvrir !
    public void OpenCanvas(GameObject canvasToOpen)
    {
        if (canvasToOpen != null)
        {
            canvasToOpen.SetActive(true);
        }
    }

    // Re-bonus : Un bouton qui fait les deux (Ouvre si c'est fermé, ferme si c'est ouvert)
    public void ToggleCanvas(GameObject canvasToToggle)
    {
        if (canvasToToggle != null)
        {
            // LE MOUCHARD : Il va afficher le nom EXACT de ce qu'il éteint dans la console
            Debug.Log("Le script UI_Manager vient de basculer l'objet : " + canvasToToggle.name);

            canvasToToggle.SetActive(!canvasToToggle.activeSelf);
        }
    }
}