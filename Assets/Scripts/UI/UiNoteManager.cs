using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UiNoteManager : MonoBehaviour
{

    [Header("note")]
    public GameObject[] NoteDontDisplay;
    public TMP_Text[] NoteDisplay;

    [Header("code")]
    public GameObject[] ImageDisplay;
    public TMP_Text[] CodeDescriptionDisplay;
    public TMP_Text[] CodeSequenceDisplay;
    public GameObject[] CodeDontDisplay;
    public RectTransform Grid;

    [Header("button")]
    public Button BtnCode;
    public Button BtnNotes;
    public Button BtnNext;
    public Button BtnPrevious;

    private List<CollectibleObject> m_spawnText;
    private bool m_ongletNoteActif = true;
    private int m_pageNote = 0;
    private int m_pageCode = 0;
    private int m_indexGauche;
    private int m_indexDroite;
         
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateNote();
        CreateCode();

        BtnNext.onClick.AddListener(NextPage);
        BtnPrevious.onClick.AddListener(PreviousPage);
        BtnNotes.onClick.AddListener(() => ChangerOnglet(true));
        BtnCode.onClick.AddListener(() => ChangerOnglet(false));

        MettreAJourBoutons();

        ChangerOnglet(m_ongletNoteActif);
    }
    void ChangerOnglet(bool ongletNotes)
    {
        m_ongletNoteActif = ongletNotes;

        // Switch les panels
        
        {
            foreach (GameObject hideText in NoteDontDisplay)
            {
                hideText.SetActive(m_ongletNoteActif);
            }
        }
        {
            foreach (GameObject hideCode in CodeDontDisplay)
            {
                hideCode.SetActive(!m_ongletNoteActif);
            }
        }

        AfficherOngletActif();
        MettreAJourBoutons();
    }
    void AfficherOngletActif()
    {
        if (m_ongletNoteActif)
            CreateNote();
        else
            CreateCode();
    }

    // creer le texte a afichier depuis l'instance 
    void CreateNote()
    {
        //int index = (int)Mathf.Clamp(m_page, 0, GameManager.Instance.UnlockedCollectibles.Length - 1);
        CollectibleObject[] collectibles = GameManager.Instance.UnlockedCollectibles;
        //BtnOuvrirCarnet.onClick.AddListener(() => { OnButtonClick(collectibles); });

        if (collectibles == null || collectibles.Length == 0)
        {
            NoteDisplay[0].text = "Aucune note pour le moment...";
            NoteDisplay[1].text = "";
            return;
        }

        int indexGauche = m_pageNote * 2;
        int indexDroite = m_pageNote * 2 + 1;

        NoteDisplay[0].text = indexGauche < collectibles.Length ? collectibles[indexGauche].Description : "";
        NoteDisplay[0].gameObject.SetActive(indexGauche < collectibles.Length);

        NoteDisplay[1].text = indexDroite < collectibles.Length ? collectibles[indexDroite].Description : "";
        NoteDisplay[1].gameObject.SetActive(indexDroite < collectibles.Length);
    }
    // pour afficher prochaine page 

    void CreateCode()
    {

        SequenceObject[] sequences = GlobalGameSettings.Instance.Sequences;

        if (sequences == null || sequences.Length == 0)
        {
            CodeDescriptionDisplay[0].text = "Aucune séquence débloquée...";
            CodeSequenceDisplay[0].text = "";
            return;
        }

        int indexGauche = m_pageCode * 2;
        int indexDroite = m_pageCode * 2 + 1;

        // Page gauche
        if (indexGauche < sequences.Length)
        {
            CodeDescriptionDisplay[0].text = sequences[indexGauche].ToString();
            CodeSequenceDisplay[0].text = sequences[indexGauche].name;
            CodeDescriptionDisplay[0].gameObject.SetActive(true);
            CodeSequenceDisplay[0].gameObject.SetActive(true);
        }
        else
        {
            CodeDescriptionDisplay[0].gameObject.SetActive(false);
            CodeSequenceDisplay[0].gameObject.SetActive(false);
        }

        // Page droite
        if (indexDroite < sequences.Length)
        {
            CodeDescriptionDisplay[1].text = sequences[indexDroite].ToString();
            CodeSequenceDisplay[1].text = sequences[indexDroite].name;
            CodeDescriptionDisplay[1].gameObject.SetActive(true);
            CodeSequenceDisplay[1].gameObject.SetActive(true);
        }
        else
        {
            CodeDescriptionDisplay[1].gameObject.SetActive(false);
            CodeSequenceDisplay[1].gameObject.SetActive(false);
        }
    }
    public void NextPage()
    {
        if (m_ongletNoteActif)
        {
            int total = Mathf.CeilToInt(GameManager.Instance.UnlockedCollectibles.Length / 2f);
            if (m_pageNote < total - 1) m_pageNote++;
        }
        else
        {
            int total = Mathf.CeilToInt(GlobalGameSettings.Instance.Sequences.Length / 2f);
            if (m_pageCode < total - 1) m_pageCode++;
        }

        AfficherOngletActif();
        MettreAJourBoutons();
    }

    public void PreviousPage()
    {
        if (m_ongletNoteActif)
        {
            if (m_pageNote > 0) m_pageNote--;
        }
        else
        {
            if (m_pageCode > 0) m_pageCode--;
        }

        AfficherOngletActif();
        MettreAJourBoutons();
    }


    void MettreAJourBoutons()
    {
        int longueur = m_ongletNoteActif
            ? GameManager.Instance.UnlockedCollectibles.Length
            : GlobalGameSettings.Instance.Sequences.Length;

        int pageActuelle = m_ongletNoteActif ? m_pageNote : m_pageCode;
        int totalPages = Mathf.CeilToInt(longueur / 2f);

        BtnPrevious.gameObject.SetActive(pageActuelle > 0);
        BtnNext.gameObject.SetActive(pageActuelle < totalPages - 1);
    
    }

    /* logique de click sur le bouton 
    public void OnButtonClick(CollectibleObject[] collectible)
    {
        foreach (TMP_Text show in NoteDisplay)
        {
            show.gameObject.SetActive(true);
        }


        foreach (GameObject hide in NoteDontDisplay)
        {
            hide.gameObject.SetActive(false);
        }

    }
    */

}