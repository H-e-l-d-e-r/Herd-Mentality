using UnityEngine;
using UnityEngine.InputSystem;

public class RadioManager : MonoBehaviour
{
    [Header("Camera")]
    public Camera Camera;

    //public CameraAnchor ModuleAnchor;
    //public CameraAnchor DjingAnchor;

    public CameraAnchor[] Anchors;

    [Header("Inputs")]
    public InputActionReference SwitchCameraInput;

    private float k_switchCooldown;

    private InputAction m_switchCameraInput;
    private int m_currentAnchorIndex = 0;
    private float m_switchCooldown = 0.0f;

    private void Start()
    {
        //NullComponents.ThrowIfNull(ModuleAnchor);
        //NullComponents.ThrowIfNull(DjingAnchor);

        m_switchCameraInput = InputActionReference.Create(SwitchCameraInput);
        
        k_switchCooldown = GlobalGameSettings.Instance.GenericInputCooldown;
        Debug.Log(k_switchCooldown);
        
        m_switchCooldown = k_switchCooldown;

        //ModuleAnchor.Focus(Camera);
        if (Anchors.Length > 0) 
        {
            // On se focus sur la first cam
            Anchors[0].Focus(Camera);
        }
    }

    private void Update()
    {
        UpdateCameraSwitch();
    }

    void UpdateCameraSwitch()
    {
        // ANCHORS?! Ca fait beaucoup la non? ;)
        if (m_switchCooldown > Mathf.Epsilon)
        {
            m_switchCooldown -= Time.deltaTime;
            return;
        }

        
        if (m_switchCameraInput.ReadValue<float>() > 0.1f)
        {
            // On va vers le prochain anchor 
            m_currentAnchorIndex = (m_currentAnchorIndex + 1) % Anchors.Length;
            Anchors[m_currentAnchorIndex].Focus(Camera);

            // reset du cooldown
            m_switchCooldown = k_switchCooldown;
        }
    }
}
