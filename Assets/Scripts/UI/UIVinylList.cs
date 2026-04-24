
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Audio;

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
    private Transform m_rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_spawnedUI = new();
        m_rectTransform = null;

        CreateVinyl();
    }

    // Update is called once per frame
    void Update()
    {
        RotateVinyl();
    }

    public void RotateVinyl()
    {
        if(m_rectTransform == null)
        {
            return;    
        }

        float increment = vitesseRotation * Time.deltaTime;
        m_rectTransform.eulerAngles = new Vector3(
            m_rectTransform.eulerAngles.x,
            m_rectTransform.eulerAngles.y,
            m_rectTransform.eulerAngles.z + increment
        );
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
        if(m_rectTransform == record.gameObject.transform)
        {
            AudioSource.Stop();
            m_rectTransform = null;
            return;
        } 

        AudioSource.Stop();

        SetAudio(record);
        AudioSource.Play();     

        TextLyrics.text = record.Vinyl.Description;
        TextLyrics.gameObject.SetActive(true);

        m_rectTransform = record.gameObject.transform;
        
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
