using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ShowText : MonoBehaviour
{
    [System.Serializable]
    public class GroupeTexte
    {
        public GameObject[] TextToShow;
    }
    public GroupeTexte[] GroupOfTexte;

    public void ActivateGroupsOfText (int IndexTextToShow)
    {

        foreach (GroupeTexte groupe in GroupOfTexte)
        {
            foreach (GameObject Text in groupe.TextToShow)
            {
                Text.SetActive(false);
            }
        }

        if (IndexTextToShow >= 0 && IndexTextToShow < GroupOfTexte.Length)
        {
            foreach (GameObject texte in GroupOfTexte[IndexTextToShow].TextToShow)
            {
                texte.SetActive(true);
            }
        }

        
    }
}
