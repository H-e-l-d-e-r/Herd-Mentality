using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UiNoteManager : MonoBehaviour
{
    public CanvasGroup Self;

    [Header("Carnet Elements")]
    public TMP_Text TitleLeft;
    public TMP_Text TitleRight;

    public TMP_Text TextLeft;
    public TMP_Text TextRight;

    public Button ButtonLeft;
    public Button ButtonRight;

    public bool HasNext
    {
        get => m_currentElement + 2 < GameManager.Instance.UnlockedCollectibles.Length;
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
            m_currentElement = Mathf.Max(0, Mathf.Min(value, GameManager.Instance.UnlockedCollectibles.Length));
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

        Debug.Log($"switch to {CurrentElement}");
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
        // we save it so that we don't need to calculate it two times!
        CollectibleObject[] collectibles = GameManager.Instance.UnlockedCollectibles;
        
        // if it is available
        if(index < collectibles.Length)
        {
            collectible = collectibles[index];
            return true;
        }

        collectible = null;
        return false;
    }

}