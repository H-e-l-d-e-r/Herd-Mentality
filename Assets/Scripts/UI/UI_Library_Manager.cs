using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILibraryManager : MonoBehaviour
{
    public RadioManager Manager;
    public VinylStorage VinylStorage;
    
    [Header("Vinyle Library")]
    public GameObject UIVinylPrefab;
    public GameObject UICodePrefab;

    public Transform PreparationCanvas; // Le Layout Group de ta biblioth�que de base
    public Transform LibraryContent; // Le Layout Group de ta biblioth�que de base

    [Header("Drop")]
    public UIDropZone[] VinylsDropZones;

    [Header("Sequences")]
    public GameObject CodePrefab;
    public Transform CodeContent; // Le Layout Group de ta biblioth�que de base
    public GameObject MemoPrefab;
    public Transform MemoContent;

    private List<VinylObject> m_targetVinyls;
    private List<UIVinylItem> m_items;

    void Start()
    {
        NullComponents.ThrowIfNull(Manager);
        NullComponents.ThrowIfNull(UIVinylPrefab);
        NullComponents.ThrowIfNull(UICodePrefab);
        NullComponents.ThrowIfNull(PreparationCanvas);
        NullComponents.ThrowIfNull(LibraryContent);
        NullComponents.ThrowIfNull(CodeContent);

        // On g�n�re la biblioth�que une seule fois au lancement du jeu
        VinylObject[] unlocked = GameManager.Instance.UnlockedVinyls;
        m_targetVinyls = new List<VinylObject>();
        m_items = new List<UIVinylItem>();

        foreach (VinylObject vinyl in unlocked)
        {
            // Instancie le Prefab et force l'�chelle � 1 pour �viter les bugs d'affichage
            GameObject uiElement = Instantiate(UIVinylPrefab, LibraryContent);
            uiElement.transform.localScale = Vector3.one;
            UIVinylItem item = uiElement.GetComponent<UIVinylItem>();
            item.Setup(vinyl);
            m_items.Add(item);
        }

        foreach(UIDropZone drop in VinylsDropZones)
        {
            drop.OnDropEvent.AddListener(() => DropZoneCallback(drop));
        }

        UI_Manager.OpenCanvas(PreparationCanvas.gameObject);
    }

    // ajoute une sequence a la liste des sequences trouvees
    public void AddSequence(RadioSequenceObject seq)
    {
        TMP_Text tmp = Instantiate(CodePrefab, CodeContent).GetComponent<TMP_Text>();
        if (tmp)
        {
            tmp.text = seq.ToString();
        }
    }

    // ajoute une sequence a la fiche de memo
    public void AddSongMemo(VinylObject obj)
    {
        TMP_Text tmp = Instantiate(MemoPrefab, MemoContent).GetComponent<TMP_Text>();
        if (tmp)
        {
            tmp.text = obj.ToString();
        }
    }

    // valide la preparation du joueur
    public void ValidatePreparation()
    {
        UI_Manager.CloseCanvas(PreparationCanvas.gameObject);
        
        // on enqueue dans le manager toutes les sequences validee
        foreach (RadioSequenceObject seq in Manager.FindSequences(m_targetVinyls.ToArray()))
        {
            Manager.EnqueueSequence(seq);
        }
        
        foreach (VinylObject vinyle in m_targetVinyls)
        {
            AddSongMemo(vinyle);
        }
    }

    public GameObject FindVinyle(VinylObject vinyl)
    {
        foreach (UIVinylItem item in m_items)
        {
            if(item.VinylData == vinyl)
            {
                return item.transform.gameObject;
            }
        }

        return null;
    }

    void DropZoneCallback(UIDropZone _)
    {
        m_targetVinyls.Clear();
        CodeContent.RemoveAllChildren();

        foreach(UIDropZone drop in VinylsDropZones)
        {
            m_targetVinyls.AddRange(drop.Vinyls);
        }

        foreach (RadioSequenceObject seq in Manager.FindSequences(m_targetVinyls.ToArray()))
        {
            AddSequence(seq);
        }
    }

    // retour l'index de la dernière drop zone qui contient un element.
    int LastFilledDropZone()
    {
        return 0;
    }
}