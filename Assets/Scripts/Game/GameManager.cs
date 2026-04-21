using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool HasInstance => s_instance != null;
    public static GameManager Instance => s_instance;
    public int CurrentDay => m_currentDay;

    // MAGIE LINQ : On trie instantan�ment les Vinyles ET les S�quences sans aucune boucle !
    public VinylObject[] UnlockedVinyls => m_unlockedCollectibles.OfType<VinylObject>().ToArray();
    public RadioSequenceObject[] UnlockedSequences => m_unlockedCollectibles.OfType<RadioSequenceObject>().ToArray();

    public CollectibleObject[] UnlockedCollectibles => m_unlockedCollectibles;

    [ReadOnlyAttribute]
    public GameStatistics Statistics;

    [SerializeField]
    private CollectibleObject[] m_unlockedCollectibles;

    private static GameManager s_instance;

    [SerializeField]
    [ReadOnly]
    private int m_currentDay = 0;

    void Start()
    {
        ResetAppreciations();
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
}