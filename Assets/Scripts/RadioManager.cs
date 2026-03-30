using UnityEngine;
using UnityEngine.InputSystem;

public class RadioManager : MonoBehaviour
{
    [Header("Camera")]
    public Camera Camera;
    public CameraAnchor ModuleAnchor;
    public CameraAnchor DjingAnchor;

    [Header("Inputs")]
    public InputActionReference SwitchCameraInput;

    private float k_switchCooldown = 0.1f;

    private InputAction m_switchCameraInput;
    private float m_swictCooldown = 0.0f;

    private void Start()
    {
        NullComponents.ThrowIfNull(ModuleAnchor);
        NullComponents.ThrowIfNull(DjingAnchor);

        m_switchCameraInput = InputActionReference.Create(SwitchCameraInput);
        m_swictCooldown = k_switchCooldown;

        ModuleAnchor.Focus(Camera);
    }

    private void Update()
    {
        UpdateCameraSwitch();
    }

    void UpdateCameraSwitch()
    {
        if (m_swictCooldown > Mathf.Epsilon)
        {
            m_swictCooldown -= Time.deltaTime;
            return;
        }

        m_swictCooldown = k_switchCooldown;
        if (m_switchCameraInput.ReadValue<float>() > 0.1f)
        {
            if (!ModuleAnchor.IsCameraAttached)
            {
                ModuleAnchor.Focus(Camera);
            }
            else
            {
                DjingAnchor.Focus(Camera);
            }
        }
    }
}
