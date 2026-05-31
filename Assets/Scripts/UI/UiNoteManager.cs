using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UiNoteManager : MonoBehaviour
{
    public CanvasGroup Self;
    public InternalCollectibleType Mode;

    [Header("Carnet Elements")]
    public TMP_Text TitleLeft;
    public TMP_Text TitleRight;

    public TMP_Text TextLeft;
    public TMP_Text TextRight;

    public Button ButtonLeft;
    public Button ButtonRight;

    public CollectibleObject[] Objects
    {
        get
        {
            return Mode switch {
                InternalCollectibleType.Collectible => GameManager.Instance.UnlockedCollectibles,
                InternalCollectibleType.Dialogue => GameManager.Instance.UnlockedDialogues,
                InternalCollectibleType.Vinyl => GameManager.Instance.UnlockedVinyls,
                InternalCollectibleType.Sequence => GameManager.Instance.UnlockedSequences,
                _ => GameManager.Instance.UnlockedCollectibles
            };
        }
    }

    public bool HasNext
    {
        get => m_currentElement + 2 < Objects.Length;
    }

    public bool HasPrevious
    {
        get => m_currentElement > 0;
    }

    public bool IsActive => Self.alpha == 1.0f;

    public int CurrentElement
    {
        get => m_currentElement;
        set
        {
            m_currentElement = Mathf.Max(0, Mathf.Min(value, Objects.Length));
        }
    }

    private static string k_EMPTY = "Empty Diary";

    [SerializeField]
    [ReadOnly]
    private int m_currentElement;
         
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Poll();

        ButtonLeft.onClick.AddListener(() =>
        {
            CurrentElement = CurrentElement - 2;
            Poll();
        });

        ButtonRight.onClick.AddListener(() =>
        {
            CurrentElement = CurrentElement + 2;
            Poll();
        });
    }

    /// <summary>
    /// Update Carnet's content
    /// </summary>
    public void Poll()
    {
        DisplayCollectibles();
        DisplayArrows();
    }

    public void Show()
    {
        Self.alpha = 1.0f;
        Self.blocksRaycasts = true;

        Poll();
    }

    public void Hide()
    {
        Self.alpha = 0.0f;
        Self.blocksRaycasts = false;
    }

    public void SetMode(InternalCollectibleType mode)
    {
        Mode = mode;
        m_currentElement = 0;
        Poll();
    }

    public void SetMode(int id) => SetMode((InternalCollectibleType)id);

    // creer le texte a afichier depuis l'instance 
    void DisplayCollectibles()
    {
        TitleLeft.SetText(k_EMPTY);
        TitleRight.SetText(string.Empty);

        TextLeft.SetText(string.Empty);
        TextRight.SetText(string.Empty);

        TitleLeft.gameObject.SetActive(true);
        TitleRight.gameObject.SetActive(true);
        
        TextLeft.gameObject.SetActive(true);
        TextRight.gameObject.SetActive(true);
        
        if (TryGetCollectible(m_currentElement, out CollectibleObject left))
        {
            TitleLeft.SetText(left.Name);
            TextLeft.SetText(left.Description);
        }

        if (TryGetCollectible(m_currentElement + 1, out CollectibleObject right))
        {
            TitleRight.SetText(right.Name);
            TextRight.SetText(right.Description);
        }
    }

    void DisplayArrows()
    {
        ButtonLeft.gameObject.SetActive(HasPrevious);
        ButtonRight.gameObject.SetActive(HasNext);
    }

    bool TryGetCollectible(int index, out CollectibleObject collectible)
    {
        // if it is available
        if(index < Objects.Length)
        {
            collectible = Objects[index];
            return true;
        }

        collectible = null;
        return false;
    }

    [Serializable]
    public enum InternalCollectibleType
    {
        Collectible = 0,
        Vinyl = 1,
        Dialogue = 2,
        Sequence =3    
    }

}