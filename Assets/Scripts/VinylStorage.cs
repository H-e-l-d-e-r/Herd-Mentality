using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class VinylStorage : MonoBehaviour
{
    [Header("Game Objects")]
    public CameraAnchor Camera;
    public GameObject VinylStaticInstance;
    public GameObject VinylDragInstance;
    public ObjectGroupBehaviour Storage;

    // On remplace le vinyle unique par un tableau pour en mettre plein.... Comme dans ta daronne
    [Header("Vinyl Collection")]
    public VinylObject[] Vinyls;
    private int m_currentVinylIndex = 0; // Pour savoir lequel on pioche...Si ta plusieurs daronne S.O Mael

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

        foreach (VinylObject vinyl in Vinyls)
        {
            VinylRecord instance = Storage.Add(VinylStaticInstance).GetComponent<VinylRecord>();
            instance.Vinyl = vinyl;
        }
    }

    void Update()
    {
        // uniquement lorsque la camera est focus sur la table
        if (Camera.IsCameraAttached)
        {
            UpdatePickupVinyl();
            UpdateDragBehaviour();
        }
    }

    void UpdatePickupVinyl() // On pouvait spawn le vinyle depuis le recorder aussi, Plus mtn! ;) (*0*) <-- urètre de Laink
    {
        if (m_dragInput.ReadValue<float>() > 0.1f)
        {
            // On ne lance le Raycast que si on ne tient rien pour plus de stabilite 
            // THANK YOU CLAUDE.AI YOU ARE MY BEST FRIEND YOU ARE THE BEST CODER YOU ARE THE LEGEND
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
                    else
                    {
                        // no op    
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

                // CORRECTION ICI : On invoque l'événement d'abord !
                OnUnselectVinyl?.Invoke(m_vinylInstance.Vinyl);

                // Et on oublie la référence APRÈS.
                m_vinylInstance = null;
            }
        }
    }

    // On met a jour le comportement des disques lorsqu'ils sont drag par le joueur
    void UpdateDragBehaviour()
    {
        if (m_isHolding && m_vinylInstance)
        {
            Ray ray = Camera.Camera.ScreenPointToRay(m_positionInput.ReadValue<Vector2>());
            Vector3 position = ray.GetPoint(m_distance);
            m_vinylInstance.SetObjectPosition(new Vector3(position.x, transform.position.y, position.z));
        }
    }

    void EnableDisks()
    {

    }

    // J'ai fait ca pour qu'il puisse se destroy quand il collide avec le storage
    private void OnTriggerStay(Collider other)
    {
        VinylRecord draggable = other.GetComponent<VinylRecord>();

        // On refuse de detruire le vinyle s'il est celui qu'on vient juste de faire spawn 
        // (Comme en Amerique :/ Fuck les Pro-life allez adopter au lieu de faire chier)
        if (draggable != null && !draggable.IsDragged && draggable != m_lastSpawnedVinyl)
        {
            draggable.DestroyObject();
        }
    }

    // Quand le vinyle sort completement de la zone, il perd son immunite (Comme dans Koh-Lanta)
    // Si tu le ramenes dans le bac il sera detruit (Et la sentence est irrevocable magueule )
    private void OnTriggerExit(Collider other)
    {
        VinylRecord draggable = other.GetComponent<VinylRecord>();

        if (draggable != null && draggable == m_lastSpawnedVinyl)
        {
            m_lastSpawnedVinyl = null;
        }
    }
}