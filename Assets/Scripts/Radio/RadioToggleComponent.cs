using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RadioToggleComponent : RadioComponentBehaviour<bool>
{
    [Header("Components")]
    public Button Toggle;
    public Sprite OnSprite;
    public Sprite OffSprite;

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
            Toggle.image.sprite = OnSprite;
        }
        else
        {
            Toggle.image.sprite = OffSprite;
            OnToggleFalse.Invoke();
        }
    }
}