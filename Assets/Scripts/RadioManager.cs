using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadioManager : MonoBehaviour
{
    [Header("Camera")]
    public Camera Camera;
    public CameraAnchor[] Anchors;

    [Header("Components")]
    public VinylRecordPlayer VinylPlayer;

    [Header("Inputs")]
    public InputActionReference SwitchCameraInput;

    // constants
    private float k_switchCooldown;

    // inputs
    private InputAction m_switchCameraInput;
    private float m_switchCooldown = 0.0f;

    // camera anchors
    private int m_currentAnchorIndex = 0;

    // les vinyles qui ont deja ete joues
    private Queue<VinylObject> m_playedVinyls;

    // les sequences que le joueur doit jouer
    private Queue<RadioSequenceObject> m_targetSequences;

    // les sequences qui ont ete valides
    private Queue<RadioSequenceObject> m_validatedSequences;

    private void Start()
    {
        m_switchCameraInput = InputActionReference.Create(SwitchCameraInput);
        
        k_switchCooldown = GlobalGameSettings.Instance.GenericInputCooldown;   
        m_switchCooldown = k_switchCooldown;

        m_playedVinyls = new Queue<VinylObject>();
        m_targetSequences = new Queue<RadioSequenceObject>();
        m_validatedSequences = new Queue<RadioSequenceObject>();

        if (Anchors.Length > 0) 
        {
            // On se focus sur la first cam
            Anchors[0].Focus(Camera);
        }
    }

    void OnEnable()
    {
        // reset
        ClearVinylQueue();
    }

    void OnDisable()
    {
        // reset
        ClearVinylQueue();
    }

    private void Update()
    {
        // camera movements
        UpdateCameraSwitch();
    }

    /// <summary>
    /// Vinyl enqueue wrapper.
    /// </summary>
    /// <param name="vinyl"></param>
    public void EnqueueVinyl(VinylObject vinyl)
    {
        if(m_playedVinyls == null)
        {
            return;
        }
        
        m_playedVinyls.Enqueue(vinyl);

        // apres, des qu'un vinyl est joue, on pourra check s'il 
        // permet de completer un objectif. 
    }

    /// <summary>
    /// Vinyl clearing wrapper.
    /// </summary>
    public void ClearVinylQueue()
    {
        if(m_playedVinyls == null)
        {
            return;
        }

        m_playedVinyls.Clear();
    }

    void UpdateCameraSwitch()
    {
        // ANCHORS?! Ca fait beaucoup la non? ;)
        // ahaha trop drole ta blague. non
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
