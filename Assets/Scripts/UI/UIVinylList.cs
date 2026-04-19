
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Audio;
using Unity.VisualScripting;
using UnityEditor;

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
    public AudioSource AudioSource;
    public float vitesseRotation;

    private List<VinylRecord> m_spawnedUI;
    private RectTransform m_rectTransform;
    private bool m_musicPlays;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_spawnedUI = new();
        CreateVinyl();
        m_musicPlays = false;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    void Awake()
    {
        m_rectTransform = Template.GetComponent<RectTransform>(); 
    }
    public void RotateVinyl()
    {
        m_rectTransform.Rotate(0f, 0f, +vitesseRotation);

    }
    public void Stop()
    {
        AudioSource.Stop();
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

    public void SetAudio(VinylRecord record)
    {
        AudioSource.clip = record.Vinyl.Clip;
        AudioSource.volume = record.Vinyl.Volume;
    }
    
    // display text + audio when button clicked 
    void OnButtonClick(VinylRecord record)
    {
        AudioSource.Stop();
        SetAudio(record);
        AudioSource.Play();

        m_musicPlays = true;        

        TextLyrics.text = record.Vinyl.Description;
        TextLyrics.gameObject.SetActive(true);
        //TextLyrics.text = record.Vinyl.Description;
        
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
        // pas ouf le code mais ca fonctionne 
    }

}
