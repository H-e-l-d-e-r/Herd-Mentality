using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public SerializableDictionary<string, string> MapHashMap;
    public TMP_Text Text;
    public string Prefixe;
    private string m_SelectIndex;

    public void SelectDestination (string select)
    {
        if (MapHashMap.TryGetValue(select, out string mapName))
        {
            m_SelectIndex = mapName;
            Text.text = $"{Prefixe} {select}";
            return; 
        }
        m_SelectIndex = string.Empty;
    }

    public void GoToScene()
    {
        if (m_SelectIndex != string.Empty)
        {
            SceneManager.LoadScene(m_SelectIndex);
        }
    }
    
  
}
