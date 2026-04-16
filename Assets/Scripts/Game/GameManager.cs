using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool HasInstance => s_instance != null;
    public static GameManager Instance => s_instance;
    
    public int CurrentDay => m_currentDay;

    public VinylObject[] UnlockedVinyls
    {
        get
        {
            List<VinylObject> vinyls = new();
            foreach(CollectibleObject collectible in m_unlockedCollectibles)
            {
                if(collectible.GetType() == typeof(VinylObject))
                {
                    vinyls.Add(collectible as VinylObject);
                }
            }

            return vinyls.ToArray();
        }
    }

    public CollectibleObject[] UnlockedCollectibles => m_unlockedCollectibles;

    [ReadOnlyAttribute]
    public GameStatistics Statistics;

    [SerializeField]
    private CollectibleObject[] m_unlockedCollectibles;

    [SerializeField]
    private int[] m_unlockedSequences;

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
        if(s_instance == null || s_instance == this)
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