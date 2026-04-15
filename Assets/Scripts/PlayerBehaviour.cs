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
    void Start()
    {
        m_interactCooldown = GlobalGameSettings.Instance.GenericInputCooldown;
        m_interactInputAction = InputActionReference.Create(InteractInputAction);
        m_positionInput = InputActionReference.Create(PositionInput);
    }

    
    void Update()
    {
        UpdateRayCast();
    }

    private void UpdateRayCast ()
    {
        if (m_interactCooldown > Mathf.Epsilon)
        {
            m_interactCooldown -= Time.deltaTime;
            return;
        }
        if (m_interactInputAction.ReadValue<float>() > 0.5f) // ray cast 
        {
            Ray ray = Camera.main.ScreenPointToRay(m_positionInput.ReadValue<Vector2>());

            if (Physics.Raycast(ray,out RaycastHit hitInfo) && hitInfo.transform.TryGetComponent(out EntitesBehaviour behaviour))
            {
                behaviour.Interact();
            }
        }

        m_interactCooldown = GlobalGameSettings.Instance.GenericInputCooldown;
    }

}
