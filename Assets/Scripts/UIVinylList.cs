
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIVinylList : MonoBehaviour
{
    [Header("List")]
    public GameObject Template;
    public RectTransform Grid;

    [Header("Content")]
    public TMP_Text TextLyrics;
    public TMP_Text TextAppreciationYl;
    public TMP_Text TextAppreciationrSr;
    public TMP_Text TextAppreciationSc;

    private List<VinylRecord> m_spawnedUI;
    private AudioSource m_audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_spawnedUI = new();
        CreateVinyl();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateVinyl()
    {
        ClearVinyls();
        foreach (VinylObject vinyl in GameManager.Instance.UnlockedVinyls)
        {
            GameObject @object = Instantiate(Template, Grid);
            VinylRecord vinylRecord = @object.GetComponent<VinylRecord>();
            Button button = @object.GetComponent<Button>();
            button.onClick.AddListener(() => { OnButtonClick(vinylRecord); });

            vinylRecord.Vinyl = vinyl;
            m_spawnedUI.Add(vinylRecord);
        }
    }

    void ClearVinyls()
    {
        foreach(VinylRecord record in m_spawnedUI)
        {
            Destroy(record.gameObject);
        }

        m_spawnedUI.Clear();
    }
    
    public void Stop()
    {
        m_audioSource.Stop();
    }

    // display text when button clicked 
    void OnButtonClick(VinylRecord record)
    {
        Debug.Log("qmlskejf");
        TextLyrics.text = record.Vinyl.Description;
        TextLyrics.gameObject.SetActive(true);
        //TextLyrics.text = record.Vinyl.Description;

        m_audioSource = gameObject.AddComponent<AudioSource>();

        m_audioSource.Play();
        Debug.Log("fhzbvsbjsdhjsxbhjv fdjk");

        if (record.Vinyl.Like.YoungLetterists)
        {
            TextAppreciationYl.gameObject.SetActive(true);
            TextAppreciationrSr.gameObject.SetActive(false);
            TextAppreciationSc.gameObject.SetActive(false);
        } 
        else if (record.Vinyl.Like.SquatRoskoff)
        {
            TextAppreciationYl.gameObject.SetActive(false);
            TextAppreciationrSr.gameObject.SetActive(true);
            TextAppreciationSc.gameObject.SetActive(false);
        }
        else if (record.Vinyl.Like.Scilas)
        {
            TextAppreciationYl.gameObject.SetActive(false);
            TextAppreciationrSr.gameObject.SetActive(false);
            TextAppreciationSc.gameObject.SetActive(true);
        }

    }

}
