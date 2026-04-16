using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayDisplay : MonoBehaviour
{
    public TMP_Text dayText;
    public string PrefixeDay;

    void Update()
    {
        //dayText.text = $"{PrefixeDay} {GameManager.Instance.CurrentDay}";
    }
}
