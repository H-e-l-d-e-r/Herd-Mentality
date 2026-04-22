using System;
using UnityEngine;

public class TextGroupBehaviour : MonoBehaviour
{
    public TextGroupContainer[] GroupOfTexte;
    
    public int CurrentGroup
    {
        get; private set;
    }

    public void ActivateGroupsOfText (int IndexTextToShow)
    {
        CurrentGroup = IndexTextToShow;

        foreach (TextGroupContainer groupe in GroupOfTexte)
        {
            foreach (GameObject Text in groupe.TextToShow)
            {
                Text.SetActive(false);
            }
        }

        if (IndexTextToShow >= 0 && IndexTextToShow < GroupOfTexte.Length)
        {
            foreach (GameObject text in GroupOfTexte[IndexTextToShow].TextToShow)
            {
                text.SetActive(true);
            }
        }
    }

    [Serializable]
    public struct TextGroupContainer
    {
        public GameObject[] TextToShow;
    }
}
