using UnityEngine;
using UnityEngine.UI;

public class RadioStateComponent : RadioComponentBehaviour<int>
{
    [Header("Parameters")]
    [Range(1, 5)]
    public int MaxState = 1;

    [Header("Components")]
    public Button Switch;

    void Start()
    {
        NullComponents.ThrowIfNull(Switch);

        // register callbacks
        Switch.onClick.AddListener(UpdateBehaviour);

        UpdateBehaviour();
    }

    void UpdateBehaviour()
    {
        SetValue((Value + 1) % MaxState);
    }
}
