using System;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class ObjectGroupBehaviour : MonoBehaviour, ICollection
{
    [Header("Direction")]
    public ObjectGroupAlignMode Mode;
    public bool InvertOrder;

    [Space]
    public float Padding;
    public Vector3 Position;

    public int Count => transform.childCount;
    public bool IsSynchronized => false;
    public object SyncRoot => false;

    void Start()
    {
        UpdateAlign();
    }

    // update for editor mode
    void OnGUI()
    {
        UpdateAlign();
    }

    /// <summary>
    /// Add a game object as a child of the transform.
    /// This game object will be constraint to the group behaviour.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns>The created game object</returns>
    public GameObject Add(GameObject gameObject)
    {
        GameObject go = Instantiate(gameObject, transform);
        UpdateAlign();
        return go;
    }

    /// <summary>
    /// Remove a child at a specific index from the collection.
    /// </summary>
    /// <param name="index"></param>
    public void Remove(int index)
    {
        Destroy(transform.GetChild(index).gameObject);
        UpdateAlign();
    }

    /// <summary>
    /// Try to remove a child by comparing it with another one.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns>true if the game object has successully been destroyed.</returns>
    public bool TryRemove(GameObject gameObject)
    {
        for (int i = 0; i < Count; i++)
        {
            if(gameObject == transform.GetChild(i))
            {
                Remove(i);
                return true;
            }
        }

        return false;
    }

    // update constraints
    private void UpdateAlign()
    {
        int order = InvertOrder ? -1 : 1;
        Vector3 cOffset = new Vector3(Padding, Padding, Padding);

        Vector3 axis = Mode switch
        {
            ObjectGroupAlignMode.Horizontal => Vector3.right,
            ObjectGroupAlignMode.Vertical => Vector3.back,
            ObjectGroupAlignMode.UpDown => Vector3.up,
            _ => Vector3.zero
        };

        Vector3 offset = Vector3.zero;
        Vector3 objectSize;
        
        foreach(Transform child in transform)
        {
            child.transform.position = transform.position + Position + Vector3.Scale(offset, axis) * order;
            
            if(child.TryGetComponent(out Mesh component))
            {
                objectSize = component.bounds.size + child.transform.localScale + cOffset;
            }
            else
            {
                objectSize = child.transform.localScale + cOffset;
            }

            offset += Vector3.Scale(objectSize, axis);
        }
    }

    public void CopyTo(Array array, int index){}
    public IEnumerator GetEnumerator()
    {
        return transform.GetEnumerator();
    }

    public enum ObjectGroupAlignMode
    {
        Horizontal,
        Vertical,
        UpDown
    }
}