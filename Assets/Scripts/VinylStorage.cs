using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class VinylStorage : MonoBehaviour
{
    [Header("Game Objects")]
    public CameraAnchor Camera;
    public GameObject VinylInstance;

    [Header("Vinyl")]
    public RadioVinyl Vinyl;

    [Header("Inputs")]
    public InputActionReference DragInput;
    public InputActionReference PositionInput;

    private InputAction m_dragInput;
    private InputAction m_positionInput;

    [SerializeField]
    private bool m_isHolding;
    private float m_distance;
    private DraggableBehaviour m_vinylInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NullComponents.ThrowIfNull(Camera);
        NullComponents.ThrowIfNull(VinylInstance);

        m_dragInput = InputActionReference.Create(DragInput);
        m_positionInput = InputActionReference.Create(PositionInput);
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.IsCameraAttached)
        {
            UpdatePickupVinyl();
            DragInstance();
        }
    }

    void UpdatePickupVinyl()
    {
        if (m_dragInput.ReadValue<float>() > 0.1f)
        {
            Ray ray = Camera.Camera.ScreenPointToRay(m_positionInput.ReadValue<Vector2>());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                m_isHolding = true;
                m_distance = hit.distance;
                
                if (!m_vinylInstance)
                {
                    m_vinylInstance = Instantiate(VinylInstance, transform).GetComponent<DraggableBehaviour>();
                    m_vinylInstance.Vinyl = Vinyl;
                }
            }
        }
        else
        {
            m_isHolding = false;
        }
    }

    void DragInstance()
    {
        if (m_isHolding && m_vinylInstance)
        {
            Ray ray = Camera.Camera.ScreenPointToRay(m_positionInput.ReadValue<Vector2>());
            Vector3 position = ray.GetPoint(m_distance);

            m_vinylInstance.SetObjectPosition(new Vector3(position.x, transform.position.y, position.z));
        }
    }
}
