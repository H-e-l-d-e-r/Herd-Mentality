using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDropZone : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public VinylStorage PhysicalStorage; // Le lecteur 3D lié à cette zone
    public Transform ProgrammationContent; // Le Layout Group où les disques vont s'aligner

    private List<VinylObject> m_programmedVinyls = new List<VinylObject>();

    // Quand on lâche un objet au-dessus de cette zone
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null)
        {
            UIVinylItem item = droppedObject.GetComponent<UIVinylItem>();
            if (item != null)
            {
                // On l'attache à cette zone visuellement (false = on garde sa taille normale)
                droppedObject.transform.SetParent(ProgrammationContent, false);
                droppedObject.transform.localScale = Vector3.one;

                UpdatePhysicalStorage();
            }
        }
    }

    // Met à jour la liste et prévient la radio 3D
    public void UpdatePhysicalStorage()
    {
        m_programmedVinyls.Clear();

        // On regarde tous les vinyles actuellement dans notre dossier
        foreach (Transform child in ProgrammationContent)
        {
            UIVinylItem item = child.GetComponent<UIVinylItem>();
            if (item != null)
            {
                m_programmedVinyls.Add(item.VinylData);
            }
        }

        // On envoie la nouvelle playlist à la radio 3D
        if (PhysicalStorage != null)
        {
            PhysicalStorage.UpdateProgrammation(m_programmedVinyls);
        }
    }
}