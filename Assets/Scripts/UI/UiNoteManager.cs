using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UiNoteManager : MonoBehaviour
{
    public GameObject ZoneDisplay;
    public TMP_Text textDisplay;

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

    void CreateText()
    {
        int index = (int)Mathf.Clamp(m_page, 0, GameManager.Instance.UnlockedCollectibles.Length - 1);
        CollectibleObject collectible = GameManager.Instance.UnlockedCollectibles[index];
    }

    public void NextPage()
    {
        m_page++;
    }
    public void PreviusPage()
    {
        m_page--;
    }


}
