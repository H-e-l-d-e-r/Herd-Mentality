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
    public UIVinylDropZone[] VinylsDropZones;

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
        m_targetVinyls = new List<VinylObject>();
        m_items = new List<UIVinylItem>();

        // create vinyls
        foreach (VinylObject vinyl in GameManager.Instance.UnlockedVinyls)
        {
            m_items.Add(InstantiateItem(vinyl));
        }

        // register drop zone hooks
        foreach(UIVinylDropZone drop in VinylsDropZones)
        {
            drop.OnDropEvent.AddListener((_) => DropZoneCallback(drop));
        }

        UIToolkit.OpenCanvas(PreparationCanvas);
        
        // force update
        DropZoneCallback(null);
    }

    // ajoute une sequence a la liste des sequences trouvees
    public void AddSequence(SequenceObject seq)
    {
        Debug.Assert(seq);

        GameObject @object = Instantiate(CodePrefab, CodeContent);
        if (@object.TryGetComponent<TMP_Text>(out TMP_Text tmp))
        {
            tmp.text = seq.ToString();
        }
    }

    // ajoute une sequence a la fiche de memo
    public void AddSongMemo(VinylObject obj)
    {
        Debug.Assert(obj);

        GameObject @object = Instantiate(MemoPrefab, MemoContent);
        if (@object.TryGetComponent<TMP_Text>(out TMP_Text tmp))
        {
            tmp.text = obj.ToString();
        }
    }

    // valide la preparation du joueur
    public void ValidatePreparation()
    {
        UIToolkit.CloseCanvas(PreparationCanvas);
        
        // on enqueue dans le manager toutes les sequences validee
        foreach (SequenceObject seq in Manager.FindSequences(m_targetVinyls.ToArray()))
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
        // avoid race concurrency
        if(m_items == null)
        {
            return null;    
        }

        foreach (UIVinylItem item in m_items)
        {
            if(item.Vinyl == vinyl)
            {
                return item.transform.gameObject;
            }
        }

        return null;
    }

    void DropZoneCallback(UIVinylDropZone _)
    {
        m_targetVinyls.Clear();
        CodeContent.RemoveAllChildren();

        // link vinyl to drop zone
        foreach(UIVinylDropZone drop in VinylsDropZones)
        {
            if(drop.Vinyl != null)
            {
                m_targetVinyls.Add(drop.Vinyl);            
            }
        }

        // update found sequences
        foreach (SequenceObject seq in Manager.FindSequences(m_targetVinyls.ToArray()))
        {
            AddSequence(seq);
        }
    }

    // create a new ui vinyl item
    UIVinylItem InstantiateItem(VinylObject vinyl)
    {
        GameObject instance = Instantiate(UIVinylPrefab, LibraryContent);
        instance.transform.localScale = Vector3.one;
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;

        if(instance.TryGetComponent(out UIVinylItem component))
        {
            component.Vinyl = vinyl;
            return component;
        }

        return null;
    }
}