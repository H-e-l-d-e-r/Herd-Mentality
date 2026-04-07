using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;
    public int CurrentDay = 0;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void DayUpdate()
    {
        CurrentDay++;
        Debug.Log($"jour : {CurrentDay}");
    }
}
