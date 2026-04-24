using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class VinylStorage : MonoBehaviour
{
    [Header("Game Objects")]
    public RadioBehaviour RadioBehaviour;
    public CameraAnchor Camera;
    public GameObject VinylStaticInstance;
    public GameObject VinylDragInstance;
    public ObjectGroupBehaviour Storage;

    [Header("Vinyl Collection")]
    public bool LoadFromGameManager;

    [ReadOnly]
    public List<VinylObject> Vinyls = new List<VinylObject>();

    [Header("Inputs")]
    public InputActionReference DragInput;
    public InputActionReference PositionInput;

    [Header("Events")]
    public UnityEvent<VinylObject> OnSelectVinyl;
    public UnityEvent<VinylObject> OnDragVinyl;
    public UnityEvent<VinylObject> OnUnselectVinyl;

    private InputAction m_dragInput;
    private InputAction m_positionInput;

    private bool m_isHolding;
    private float m_distance;

    private VinylRecord m_vinylInstance;
    private VinylRecord m_lastSpawnedVinyl;

    void Start()
    {
        NullComponents.ThrowIfNull(Camera);
        NullComponents.ThrowIfNull(VinylDragInstance);

        m_dragInput = InputActionReference.Create(DragInput);
        m_positionInput = InputActionReference.Create(PositionInput);

        if (LoadFromGameManager)
        {
            Vinyls.Clear();
            Vinyls.AddRange(GameManager.Instance.UnlockedVinyls);

            foreach (VinylObject vinyl in GameManager.Instance.UnlockedVinyls)
            {
                VinylRecord instance = Storage.Add(VinylStaticInstance).GetComponent<VinylRecord>();
                instance.Vinyl = vinyl;
            }
        }
    }

    // Fonction appelée par l'UI (le Drag & Drop) pour ranger les disques physiques
    //public void UpdateProgrammation(List<VinylObject> programmedVinyls)
    //{
    //    Vinyls = programmedVinyls;
    //    Storage.transform.RemoveAllChildren();
    //
    //    foreach (VinylObject vinyl in Vinyls)
    //    {
    //        VinylRecord instance = Storage.Add(VinylStaticInstance).GetComponent<VinylRecord>();
    //        instance.Vinyl = vinyl;
    //    }
    //}

    void Update()
    {
        if (Camera.IsCameraAttached && RadioBehaviour.IsOn)
        {
            UpdatePickupVinyl();
            UpdateDragBehaviour();
        }
    }

    void UpdatePickupVinyl()
    {
        if (m_dragInput.ReadValue<float>() > 0.1f)
        {
            if (!m_isHolding)
            {
                Ray ray = Camera.Camera.ScreenPointToRay(m_positionInput.ReadValue<Vector2>());

                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.TryGetComponent(out VinylRecord record))
                {
                    if (hit.transform.tag == "Static Vinyl Record")
                    {
                        int index = hit.transform.GetSiblingIndex();

                        m_isHolding = true;
                        m_distance = hit.distance;

                        m_vinylInstance = Instantiate(VinylDragInstance, transform).GetComponent<VinylRecord>();
                        m_vinylInstance.Vinyl = record.Vinyl;
                        m_vinylInstance.IsDragged = true;

                        m_lastSpawnedVinyl = m_vinylInstance;

                        Storage.Remove(index);
                        OnSelectVinyl?.Invoke(record.Vinyl);
                    }
                    else if (hit.transform.tag == "Dynamic Vinyl Record")
                    {
                        m_isHolding = true;
                        m_distance = hit.distance;
                        m_vinylInstance = record;
                        m_vinylInstance.IsDragged = true;

                        OnDragVinyl?.Invoke(record.Vinyl);
                    }
                }
            }
        }
        else
        {
            m_isHolding = false;
            if (m_vinylInstance != null)
            {
                m_vinylInstance.IsDragged = false;
                OnUnselectVinyl?.Invoke(m_vinylInstance.Vinyl);
                m_vinylInstance = null;
            }
        }
    }

    void UpdateDragBehaviour()
    {
        if (m_isHolding && m_vinylInstance)
        {
            Ray ray = Camera.Camera.ScreenPointToRay(m_positionInput.ReadValue<Vector2>());
            Vector3 position = ray.GetPoint(m_distance);
            m_vinylInstance.SetObjectPosition(new Vector3(position.x, transform.position.y, position.z));
        }
    }

    // private void OnTriggerStay(Collider other)
    //{
    //VinylRecord draggable = other.GetComponent<VinylRecord>();

    // if (draggable != null && !draggable.IsDragged && draggable != m_lastSpawnedVinyl)
    //{
    // draggable.DestroyObject();
    //}
    //}

    private void OnTriggerExit(Collider other)
    {
        VinylRecord draggable = other.GetComponent<VinylRecord>();

        if (draggable != null && draggable == m_lastSpawnedVinyl)
        {
            m_lastSpawnedVinyl = null;
        }
    }
}