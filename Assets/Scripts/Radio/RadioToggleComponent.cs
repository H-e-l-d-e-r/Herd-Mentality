using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RadioToggleComponent : RadioComponentBehaviour<bool> 
{
    [Header("Components")]
    public Button Toggle;
    public bool truetest = true;
    public bool falsetest = false;


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
        m_value = !m_value;

        // call events
        if (m_value)
        {
            OnToggleTrue.Invoke();
            print(truetest);
            
        }
        else
        {
           
            OnToggleFalse.Invoke();
            print(falsetest);
        }
    }
}