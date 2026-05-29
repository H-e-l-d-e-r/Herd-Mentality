using System;
using DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Cameras")]
    public Camera Camera;
    public CameraAnchor[] Anchors;
    public bool CanSwitchCamera = true; 

    // input references
    private InputAction m_interactInput;
    private InputAction m_mousePositionInput;
    private InputAction m_nextCameraInput;

    // cooldown
    private float m_interactCooldown;
    private float m_cameraCooldown;
    private int m_currentCamera;

    void Start()
    {
        NullComponents.ThrowIfNull(Camera);

        m_interactCooldown = GlobalGameSettings.Instance.GenericInputCooldown;
        m_cameraCooldown = GlobalGameSettings.Instance.GenericInputCooldown;

        m_interactInput = InputActionReference.Create(GlobalGameSettings.Instance.InteractInputAction);
        m_mousePositionInput = InputActionReference.Create(GlobalGameSettings.Instance.MousePositionInputAction);
        m_nextCameraInput = InputActionReference.Create(GlobalGameSettings.Instance.NextCameraInputAction);

        m_currentCamera = 0;

        ResetCamera();
    }

    void Update()
    {
        // avoid inputs over dialogues
        if(!(Dialogue.Instance != null && Dialogue.Instance.IsPlaying))
        {
            UpdateCamera();
        }
    }

    public void SetCamera(int index)
    {
        Anchors[Mathf.Clamp(index, 0, Anchors.Length - 1)].Focus(Camera);
    }

    void UpdateCamera()
    {
        if (!CanSwitchCamera)
        {
            return;
        }

        // cooldown
        if(m_cameraCooldown > Mathf.Epsilon)
        {
            m_cameraCooldown -= Time.deltaTime;
            return;
        }

        if(m_nextCameraInput.ReadValue<float>() > 0.1f)
        { 
            int index = (m_currentCamera++) % (Anchors.Length);
            Anchors[index].Focus(Camera);
        }

        m_cameraCooldown = GlobalGameSettings.Instance.GenericInputCooldown;
    }

    void ResetCamera()
    {
        if(Anchors.Length > 0)
        {
            Anchors[0].Focus(Camera);
        }
    }

}
