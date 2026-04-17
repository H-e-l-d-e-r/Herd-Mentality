using UnityEngine;

public class UILibraryManager : MonoBehaviour
{
    [Header("Initialization")]
    public GameObject UIVinylPrefab;
    public Transform LibraryContent; // Le Layout Group de ta bibliothèque de base

    void Start()
    {
        // On génère la bibliothèque une seule fois au lancement du jeu
        VinylObject[] unlocked = GameManager.Instance.UnlockedVinyls;

        foreach (VinylObject vinyl in unlocked)
        {
            // Instancie le Prefab et force l'échelle à 1 pour éviter les bugs d'affichage
            GameObject uiElement = Instantiate(UIVinylPrefab, LibraryContent);
            uiElement.transform.localScale = Vector3.one;
            uiElement.GetComponent<UIVinylItem>().Setup(vinyl);
        }
    }
}