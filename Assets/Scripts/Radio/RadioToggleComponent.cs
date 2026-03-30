using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RadioToggleComponent : RadioComponentBehaviour<bool> 
{
    [Header("Components")]
    public Button Toggle;

    [Header("Events")]
    public UnityEvent OnToggleTrue;
    public UnityEvent OnToggleFalse;

    private void Start()
    {
        NullComponents.ThrowIfNull(Toggle);

        // register callbacks
        Toggle.onClick.AddListener(UpdateBehaviour);
        UpdateBehaviour();
    }

    void UpdateBehaviour()
    {
        SetValue(!Toggle);

        // call events
        if (Value)
        {
            OnToggleTrue.Invoke();
        }
        else
        {
            OnToggleFalse.Invoke();
        }
    }
}