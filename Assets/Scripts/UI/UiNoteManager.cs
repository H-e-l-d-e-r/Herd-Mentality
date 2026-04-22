using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public RectTransform Grid;

    [Header("button")]
    public Button BtnCode;
    public Button BtnOuvrirCarnet;
    public Button BtnNext;
    public Button BtnPrevious;

    private List<CollectibleObject> m_spawnText;
    private int m_page = 0;
    private int m_indexGauche;
    private int m_indexDroite;
         
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_indexGauche = m_page * 2;
        m_indexDroite = m_page * 2 + 1;

        CreateText();

        //BtnNext.onClick.AddListener(NextPage);
        //BtnPrevious.onClick.AddListener(PreviusPage);
        BtnOuvrirCarnet.onClick.AddListener(CreateText);
        BtnCode.onClick.AddListener(CreateCode);

        MettreAJourBoutons();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // creer le texte a afichier depuis l'instance 
    void CreateText()
    {
        //int index = (int)Mathf.Clamp(m_page, 0, GameManager.Instance.UnlockedCollectibles.Length - 1);
        CollectibleObject[] collectible = GameManager.Instance.UnlockedCollectibles;
        BtnOuvrirCarnet.onClick.AddListener(() => { OnButtonClick(collectible); });

        if (collectible == null || GameManager.Instance.UnlockedCollectibles.Length == 0) return;

        // Page gauche
        
        if (m_indexGauche < collectible.Length)
        {
            NoteDisplay[0].text = collectible[m_indexGauche].Description;
            NoteDisplay[0].gameObject.SetActive(true);
        }
        else
        {
            NoteDisplay[0].text = "";
            NoteDisplay[0].gameObject.SetActive(false);
        }
        //page droite
        
        if (m_indexDroite < collectible.Length)
        {
            NoteDisplay[1].text = collectible[m_indexDroite].Description;
            NoteDisplay[1].gameObject.SetActive(true);
        }
        else
        {
            NoteDisplay[1].text = "";
            NoteDisplay[1].gameObject.SetActive(false);
        }

        //m_spawnText.Add(collectible);   
        // bz ton pere la tchoing page a la con de carnet 
    }
    // pour afficher prochaine page 
    
    public void NextPage()
    {
        int totalPages = Mathf.CeilToInt(GameManager.Instance.UnlockedCollectibles.Length / 2f);
        if (m_page < totalPages - 1)
        {
            m_page++;
            CreateText();
            MettreAJourBoutons();
        }
    }
    //pour afficher Previus page
    public void PreviusPage()
    {
        if (m_page > 0)
        {
            m_page--;
            CreateText();
            MettreAJourBoutons();
        }
    }
    
    void CreateCode()
    {
        RadioSequenceObject[] sequence = GameManager.Instance.UnlockedSequences;

        if (sequence == null || GameManager.Instance.UnlockedSequences.Length == 0) return;

        
        if (m_indexGauche < sequence.Length)
        {
            CodeDescriptionDisplay[0].text = sequence[m_indexGauche].ToString();
            CodeDescriptionDisplay[0].gameObject.SetActive(true);

        }
        else
        {
            CodeDescriptionDisplay[0].text = "";
            CodeDescriptionDisplay[0].gameObject.SetActive(false);
        }
        
        if (m_indexDroite < sequence.Length)
        {
            CodeSequenceDisplay[0].text = sequence[m_indexDroite].name;
            CodeSequenceDisplay[0].gameObject.SetActive(true);
        }
        else
        {
            CodeSequenceDisplay[0].text = "";
            CodeSequenceDisplay[0].gameObject.SetActive(false);
        }



    }
    void MettreAJourBoutons()
    {
        CollectibleObject[] collectibles = GameManager.Instance.UnlockedCollectibles;

        if (collectibles == null || collectibles.Length == 0)
        {
            BtnNext.gameObject.SetActive(false);
            BtnPrevious.gameObject.SetActive(false);
            return;
        }

        int totalPages = Mathf.CeilToInt(collectibles.Length / 2f);
        BtnPrevious.gameObject.SetActive(m_page > 0);
        BtnNext.gameObject.SetActive(m_page < totalPages - 1);
    }
    // logique de click sur le bouton 
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

}
