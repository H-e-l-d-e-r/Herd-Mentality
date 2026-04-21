using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiNoteManager : MonoBehaviour
{
    public GameObject ZoneDisplay;
    public GameObject[] NoteDontDisplay;
    public TMP_Text[] NoteDisplay;
    public Button Button;

    private List<CollectibleObject> m_spawnText;
    private int m_page = 0;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_spawnText = new();

        CreateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // creer le texte a afichier depuis l'instance 
    void CreateText()
    {
        int index = (int)Mathf.Clamp(m_page, 0, GameManager.Instance.UnlockedCollectibles.Length - 1);
        CollectibleObject collectible = GameManager.Instance.UnlockedCollectibles[index];
        Button.onClick.AddListener(() => { OnButtonClick(collectible); });
        foreach (TMP_Text page in NoteDisplay)
        {
            page.text = collectible.Description;
        }

        m_spawnText.Add(collectible);   
        // bz ton pere la tchoing page a la con de carnet 
    }
    // pour afficher prochaine page 
    public void NextPage()
    {
        m_page++;
    }
    // pour afficher Previus page
    public void PreviusPage()
    {
        m_page--;
    }
    // logique de click sur le bouton 
    public void OnButtonClick(CollectibleObject collectible)
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
