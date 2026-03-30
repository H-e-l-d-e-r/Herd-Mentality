using System;
using DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This is a sample script that show how to call a dialogue sequence. 
/// </summary> <summary>
/// 
/// </summary>
public class DialogueCaller : MonoBehaviour 
{
    public DialogueTable Table;
    public DialogueSystemBehaviour DialogueSystem;

    public InputActionReference InputAction;

    private DialoguePtr m_diag;
    private InputAction m_input;

    public void Start()
    {
        m_diag = Dialogue.Instance.Find("Coucou");
        Dialogue.PlayDialogue(m_diag);
        
    }

    void OnEnable()
    {
        m_input = InputActionReference.Create(InputAction);
        m_input.Enable();
    }

    void OnDisable()
    {
        m_input.Disable();
    }

    public void Update()
    {
        if (m_input.ReadValue<float>() > float.Epsilon){
            Dialogue.PlayDialogue(m_diag);
        }
    }
}