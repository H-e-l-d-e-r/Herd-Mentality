using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILibraryManager : MonoBehaviour
{
    public RadioManager Manager;
    public VinylStorage VinylStorage;
    
    [Header("Initialization")]
    public GameObject UIVinylPrefab;
    public GameObject UICodePrefab;

    public Transform PreparationCanvas; // Le Layout Group de ta biblioth�que de base
    public Transform LibraryContent; // Le Layout Group de ta biblioth�que de base
    public Transform CodeContent; // Le Layout Group de ta biblioth�que de base

    public UIDropZone[] VinylsDropZones;

    private List<VinylObject> m_targetVinyls;

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

        foreach (VinylObject vinyl in unlocked)
        {
            // Instancie le Prefab et force l'�chelle � 1 pour �viter les bugs d'affichage
            GameObject uiElement = Instantiate(UIVinylPrefab, LibraryContent);
            uiElement.transform.localScale = Vector3.one;
            uiElement.GetComponent<UIVinylItem>().Setup(vinyl);
        }

        foreach(UIDropZone drop in VinylsDropZones)
        {
            drop.OnDropEvent.AddListener(() => DropZoneCallback(drop));
        }

        UI_Manager.OpenCanvas(PreparationCanvas.gameObject);
    }

    public void ValidatePreparation()
    {
        UI_Manager.CloseCanvas(PreparationCanvas.gameObject);
        
        foreach (RadioSequenceObject seq in Manager.FindSequences(m_targetVinyls.ToArray()))
        {
            Manager.EnqueueSequence(seq);
        }
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
            Debug.Log(seq.ToString());

            TMP_Text instantiateText = Instantiate(UICodePrefab, CodeContent).GetComponent<TMP_Text>();
            instantiateText.text = seq.ToString();
        }
    }
}