using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance => s_instance;
    public int CurrentDay => p_currentDay;

    private static DayManager s_instance;
    private int p_currentDay = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);            
        }
    }

    public void DayUpdate()
    {
        p_currentDay++;
        
        Debug.Log($"jour : {CurrentDay}");
    }
}
