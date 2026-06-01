using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool HasInstance => s_instance != null;
    public static GameManager Instance => s_instance;
    public int CurrentDay => m_currentDay;

    public VinylObject[] UnlockedVinyls => m_unlockedCollectibles.OfType<VinylObject>().ToArray();
    public SequenceObject[] UnlockedSequences => m_unlockedCollectibles.OfType<SequenceObject>().ToArray();
    public DialogueTable[] UnlockedDialogues => m_unlockedCollectibles.OfType<DialogueTable>().ToArray();

    public CollectibleObject[] UnlockedCollectibles => m_unlockedCollectibles.ToArray();

    [ReadOnlyAttribute]
    public GameStatistics Statistics;

    [SerializeField]
    private List<CollectibleObject> m_unlockedCollectibles;

    private static GameManager s_instance;

    [SerializeField]
    [ReadOnly]
    private int m_currentDay = 0;

    void Start()
    {
        ResetAppreciations();

        Dialogue.Instance.OnDialogueStartEvent += () =>
        {
            AddCollectible(Dialogue.FindDialogueTable(Dialogue.Instance.Current));
        };
    }

    void OnEnable()
    {
        Singletonize();
    }

    public int NextDay()
    {
        return m_currentDay++;
    }

    void ResetAppreciations()
    {
        Statistics.AprYoungLetterists = 100;
        Statistics.AprSquatRoskoff = 100;
        Statistics.AprScilas = 100;
    }

    void Singletonize()
    {
        if (s_instance == null || s_instance == this)
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCollectible(CollectibleObject collectible)
    {
        if (m_unlockedCollectibles.Contains(collectible))
        {
            return;
        }

        m_unlockedCollectibles.Add(collectible);
    }

    public void End()
    {
        SceneManager.LoadScene("EndScreen");
    }
}