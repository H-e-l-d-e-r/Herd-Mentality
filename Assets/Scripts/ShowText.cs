using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShowText : MonoBehaviour
{
    public GameObject[] TextToShow;

    public void ActivateText(int IndexTextToShow)
    {

        foreach (GameObject Text in TextToShow) 
        {
            Text.SetActive(false);
        }
        if (IndexTextToShow >= 0 && IndexTextToShow < TextToShow.Length)
        {
            TextToShow[IndexTextToShow].SetActive(true);
        }

        
    }
}
