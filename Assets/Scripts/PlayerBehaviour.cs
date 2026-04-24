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

    private EntityBehaviour m_lastEntityHover;
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
        // clear focus
        if (m_lastEntityHover)
        {
            m_lastEntityHover.LostFocus();
            m_lastEntityHover = null;
        }

        // avoid inputs over dialogues
        if(!(Dialogue.Instance != null && Dialogue.Instance.IsPlaying))
        {
            UpdateCamera();
            UpdateRaycast();
        }
    }

    void UpdateRaycast()
    {
        bool canInteract = true;
        
        // cooldown
        if (m_interactCooldown > Mathf.Epsilon)
        {
            m_interactCooldown -= Time.deltaTime;
            canInteract = false;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(m_mousePositionInput.ReadValue<Vector2>());

        // trace raycast and check for any entity component
        if (Physics.Raycast(ray,out RaycastHit hitInfo) && hitInfo.transform.TryGetComponent(out EntityBehaviour behaviour))
        {
            m_lastEntityHover = behaviour;

            // when the mouse is over an entity
            behaviour.Focus();

            // when the play interact with it
            if (m_interactInput.ReadValue<float>() > 0.5f && canInteract) // ray cast 
            {
                behaviour.Interact();
                m_interactCooldown = GlobalGameSettings.Instance.GenericInputCooldown;
            }
        }
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
