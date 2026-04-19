using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIDropZone : MonoBehaviour, IDropHandler
{
    public VinylObject[] Vinyls => m_programmedVinyls.ToArray();

    [Header("References")]
    // j'ai changé l'endroit où l'enregistrement des vinyls est fait
    // maintenant c'est dès que le joueur valide ça programmation
    //public VinylStorage PhysicalStorage; // Le lecteur 3D li� � cette zone
    public Transform ProgrammationContent; // Le Layout Group o� les disques vont s'aligner
    
    public int Capacity = 1;

    public UnityEvent OnDropEvent;

    private List<VinylObject> m_programmedVinyls = new List<VinylObject>();

    // Quand on l�che un objet au-dessus de cette zone
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject != null && m_programmedVinyls.Count < Capacity)
        {
            UIVinylItem item = droppedObject.GetComponent<UIVinylItem>();
            if (item != null)
            {
                // On l'attache � cette zone visuellement (false = on garde sa taille normale)
                droppedObject.transform.SetParent(ProgrammationContent, false);
                droppedObject.transform.localScale = Vector3.one;

                UpdatePhysicalStorage();
                
                OnDropEvent.Invoke();
            }
        }
    }

    // Met � jour la liste et pr�vient la radio 3D
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

        // On envoie la nouvelle playlist � la radio 3D
        // if (PhysicalStorage != null)
        // {
        //     PhysicalStorage.UpdateProgrammation(m_programmedVinyls);
        // }
    }
}