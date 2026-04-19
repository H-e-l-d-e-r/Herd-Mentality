using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    public InputActionReference InteractInputAction;
    public InputActionReference PositionInput;

    private InputAction m_interactInputAction;
    private InputAction m_positionInput;
    private float m_interactCooldown;

    private EntityBehaviour m_lastEntityHover;

    void Start()
    {
        m_interactCooldown = GlobalGameSettings.Instance.GenericInputCooldown;
        m_interactInputAction = InputActionReference.Create(InteractInputAction);
        m_positionInput = InputActionReference.Create(PositionInput);
    }

    void Update()
    {
        // clear focus
        if (m_lastEntityHover)
        {
            m_lastEntityHover.LostFocus();
            m_lastEntityHover = null;
        }

        UpdateRaycast();
    }

    private void UpdateRaycast ()
    {
        bool canInteract = true;
        
        // cooldown
        if (m_interactCooldown > Mathf.Epsilon)
        {
            m_interactCooldown -= Time.deltaTime;
            canInteract = false;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(m_positionInput.ReadValue<Vector2>());

        // trace raycast and check for any entity component
        if (Physics.Raycast(ray,out RaycastHit hitInfo) && hitInfo.transform.TryGetComponent(out EntityBehaviour behaviour))
        {
            m_lastEntityHover = behaviour;

            // when the mouse is over an entity
            behaviour.Focus();

            // when the play interact with it
            if (m_interactInputAction.ReadValue<float>() > 0.5f && canInteract) // ray cast 
            {
                behaviour.Interact();
                m_interactCooldown = GlobalGameSettings.Instance.GenericInputCooldown;
            }
        }
    }

}
