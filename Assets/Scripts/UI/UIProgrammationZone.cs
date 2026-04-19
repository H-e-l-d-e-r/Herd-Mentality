using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIProgrammationZone : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public VinylStorage PhysicalStorage; // Glisse ton objet 3D 
    public Transform ProgrammationContent; // Le Layout Group 

    [Header("Initialization")]
    public GameObject UIVinylPrefab; // Le Prefab  
    public Transform LibraryContent; // Le ScrollView 

    private List<VinylObject> m_programmedVinyls = new List<VinylObject>();

    void Start()
    {
        // 1.On peuple la biblio avec tout ce qui est débloqué
        VinylObject[] unlocked = GameManager.Instance.UnlockedVinyls;

        foreach (VinylObject vinyl in unlocked)
        {
            GameObject uiElement = Instantiate(UIVinylPrefab, LibraryContent);
            uiElement.GetComponent<UIVinylItem>().Setup(vinyl);
        }
    }

    // Quand on lache un item ui dans la zone
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
            UIVinylItem item = droppedObject.GetComponent<UIVinylItem>();
            if (item != null)
            {
                // On attache l'élément UI visuellement à notre liste
                droppedObject.transform.SetParent(ProgrammationContent);

                
                UpdatePhysicalStorage();
            }
        }
    }

    // Regarde tt les enfants et met a jour la liste
    public void UpdatePhysicalStorage()
    {
        m_programmedVinyls.Clear();

        foreach (Transform child in ProgrammationContent)
        {
            UIVinylItem item = child.GetComponent<UIVinylItem>();
            if (item != null)
            {
                m_programmedVinyls.Add(item.VinylData);
            }
        }

        // On envoie l'ordre au bac à vinyles 3DDDDDDD
        if (PhysicalStorage != null)
        {
            PhysicalStorage.UpdateProgrammation(m_programmedVinyls);
        }
    }
}