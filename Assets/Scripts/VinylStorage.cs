using UnityEngine;
using UnityEngine.InputSystem;

public class VinylStorage : MonoBehaviour
{
    [Header("Game Objects")]
    public CameraAnchor Camera;
    public GameObject VinylInstance;

    // On remplace le vinyle unique par un tableau pour en mettre plein.... Comme dans ta daronne
    [Header("Vinyl Collection")]
    public RadioVinyl[] Vinyls;
    private int m_currentVinylIndex = 0; // Pour savoir lequel on pioche...Si ta plusieurs daronne S.O Mael

    [Header("Inputs")]
    public InputActionReference DragInput;
    public InputActionReference PositionInput;

    private InputAction m_dragInput;
    private InputAction m_positionInput;

    [SerializeField] private bool m_isHolding;
    private float m_distance;
    private DraggableBehaviour m_vinylInstance;

    //  On garde en mémoire le vinyle qui vient d'apparaître (Et oui on a pas encore Alzeihmer ici)
    private DraggableBehaviour m_lastSpawnedVinyl;

    void Start()
    {
        NullComponents.ThrowIfNull(Camera);
        NullComponents.ThrowIfNull(VinylInstance);
        m_dragInput = InputActionReference.Create(DragInput);
        m_positionInput = InputActionReference.Create(PositionInput);
    }

    void Update()
    {
        if (Camera.IsCameraAttached)
        {
            UpdatePickupVinyl();
            DragInstance();
        }
    }

    void UpdatePickupVinyl() // On pouvait spawn le vinyle depuis le recorder aussi, Plus mtn! ;) (*0*) <-- urètre de Laink
    {
        if (m_dragInput.ReadValue<float>() > 0.1f)
        {
            // On ne lance le Raycast que si on ne tient rien pour plus de stabilité THANK YOU CLAUDE.AI YOU ARE MY BEST FRIEND YOU ARE THE BEST CODER YOU ARE THE LEGEND
            if (!m_isHolding)
            {
                Ray ray = Camera.Camera.ScreenPointToRay(m_positionInput.ReadValue<Vector2>());
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // 1. SI ON CLIQUE SUR LE BAC (On en crée un nouveau)
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        // On vérifie s'il reste des vinyles dans la daronne (la liste)
                      
                        if (m_currentVinylIndex < Vinyls.Length)
                        {
                            m_isHolding = true;
                            m_distance = hit.distance;

                            m_vinylInstance = Instantiate(VinylInstance, transform).GetComponent<DraggableBehaviour>();

                            //  On assigne la musique depuis ta mère/....Enfin la liste quoi
                            m_vinylInstance.Vinyl = Vinyls[m_currentVinylIndex];

                            // On passe au suivant sans faire TOURNER LES SERVIETTES indéfiniment
                            m_currentVinylIndex++;

                            m_vinylInstance.IsDragged = true;

                            // On donne le totem d'immunité au vinyle : wALLAH BARDELLA SI JE T'ATTRAPE JE VAIS T'ENCULER
                            m_lastSpawnedVinyl = m_vinylInstance;
                        }
                    }
                    // 2. SI ON CLIQUE SUR UN VINYLE DÉJÀ AU SOL (On le ramasse)
                    else
                    {
                        DraggableBehaviour vinylAuSol = hit.collider.GetComponent<DraggableBehaviour>();
                        if (vinylAuSol != null)
                        {
                            m_isHolding = true;
                            m_distance = hit.distance;
                            m_vinylInstance = vinylAuSol;
                            m_vinylInstance.IsDragged = true;
                        }
                    }
                }
            }
        }
        else
        {
            m_isHolding = false;
            if (m_vinylInstance)
            {
                m_vinylInstance.IsDragged = false;
                m_vinylInstance = null;
            }
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

    private void OnTriggerStay(Collider other) // J'ai fait ca pour qu'il puisse se destroy quand il collide avec le storage
    {
        DraggableBehaviour draggable = other.GetComponent<DraggableBehaviour>();

        // On refuse de détruire le vinyle s'il est celui qu'on vient juste de faire spawn (Comme en Amérique :/ Fuck les Pro-life allez adopter au lieu de faire chier)
        if (draggable != null && !draggable.IsDragged && draggable != m_lastSpawnedVinyl)
        {
            draggable.DestroyObject();
        }
    }

    //  Quand le vinyle sort complètement de la zone, il perd son immunité (Comme dans Koh-Lanta)
    // Si tu le ramènes dans le bac il sera détruit (Et la sentence est irrévocable magueule )
    private void OnTriggerExit(Collider other)
    {
        DraggableBehaviour draggable = other.GetComponent<DraggableBehaviour>();

        if (draggable != null && draggable == m_lastSpawnedVinyl)
        {
            m_lastSpawnedVinyl = null;
        }
    }
}