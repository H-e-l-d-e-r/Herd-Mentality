using UnityEngine;

public class UIVinylDropZone : UIDropBehaviour<UIVinylItem>
{
    public VinylObject Vinyl
    {
        get
        {
            if(Content == null)
            {
                return null;
            }

            if(Content.TryGetComponent<UIVinylItem>(out UIVinylItem item))
            {
                return item.Vinyl;
            }

            return null;
        }
    }

    public override void OnContentChange(UIVinylItem newValue)
    {
        //Vinyl = newValue.Vinyl;
    }

    /*public VinylObject[] Vinyls
    {
        get
        {
            VinylObject[] vinyles = new VinylObject[m_programmedVinyls.Count];
            for (int i = 0; i < m_programmedVinyls.Count; i++)
            {
                vinyles[i] = m_programmedVinyls[i].GetComponent<UIVinylItem>().VinylData;
            }

            return vinyles;
        }
    }

    [Header("References")]
    // j'ai changé l'endroit où l'enregistrement des vinyls est fait
    // maintenant c'est dès que le joueur valide ça programmation
    //public VinylStorage PhysicalStorage; // Le lecteur 3D li� � cette zone
    public Transform ProgrammationContent; // Le Layout Group o� les disques vont s'aligner
    public Transform ListContent;
    
    public int Capacity = 1;
    public bool IsDroppable = true;

    public UnityEvent OnDropEvent;

    private List<GameObject> m_programmedVinyls = new();

    void Start()
    {

    }

    // Quand on l�che un objet au-dessus de cette zone
    public void OnDrop(PointerEventData eventData)
    {
        if (IsDroppable)
        {
            SetContent(eventData.pointerDrag);
        }
    }

    public void SetContent(GameObject @object)
    {
        if (!@object)
        {
            return;
        }

        UIVinylItem item = @object.GetComponent<UIVinylItem>();
        if (item != null)
        {
            if (m_programmedVinyls != null && m_programmedVinyls.Count >= Capacity)
            {
                GameObject last = m_programmedVinyls.Last();
                last.transform.SetParent(ListContent, false);
                last.transform.localScale = Vector3.one;

                m_programmedVinyls.Remove(last);
            }

            // On l'attache � cette zone visuellement (false = on garde sa taille normale)
            @object.transform.SetParent(ProgrammationContent, false);
            @object.transform.localScale = Vector3.one;
            item.IsDraggable = IsDroppable;

            UpdatePhysicalStorage();

            OnDropEvent.Invoke();
        }
    }

    // Met � jour la liste et pr�vient la radio 3D
    public void UpdatePhysicalStorage()
    {
        m_programmedVinyls.Clear();

        // On regarde tous les vinyles actuellement dans notre dossier
        foreach (Transform child in ProgrammationContent)
        {
            m_programmedVinyls.Add(child.gameObject);
        }

        // On envoie la nouvelle playlist � la radio 3D
        // if (PhysicalStorage != null)
        // {
        //     PhysicalStorage.UpdateProgrammation(m_programmedVinyls);
        // }
    }*/
}