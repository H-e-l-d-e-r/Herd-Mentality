using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayDisplay : MonoBehaviour
{
    public TMP_Text dayText;
    public string PrefixeDay;

    void OnEnable()
    {
        UpdateUI();
    }

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (GameManager.HasInstance)
        {
            dayText.text = $"{PrefixeDay} {GameManager.Instance.CurrentDay}";        
        }
    }
}
