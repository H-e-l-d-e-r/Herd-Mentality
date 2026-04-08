using DialogueSystem;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class EntitesBehaviour : MonoBehaviour
{
    

    public virtual void OnInteract() { }
    
}
