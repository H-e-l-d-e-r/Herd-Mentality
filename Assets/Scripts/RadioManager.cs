using UnityEngine;
using UnityEngine.InputSystem;

public class RadioManager : MonoBehaviour
{
    [Header("Camera")]
    public Camera Camera;
    public CameraAnchor ModuleAnchor;
    public CameraAnchor DjingAnchor;
    public CameraAnchor[] anchors;
    private int m_currentAnchorIndex = 0;

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
        // ANCHORS?! Ca fait beaucoup la non? ;)
        if (m_swictCooldown > Mathf.Epsilon)
        {
            m_swictCooldown -= Time.deltaTime;
            return;
        }

        
        if (m_switchCameraInput.ReadValue<float>() > 0.1f)
        {
            
            if (anchors.Length > 0)
            {
                
                m_currentAnchorIndex = (m_currentAnchorIndex + 1) % anchors.Length;

                
                anchors[m_currentAnchorIndex].Focus(Camera);

               
                m_swictCooldown = k_switchCooldown;
            }
        }
    }
}
