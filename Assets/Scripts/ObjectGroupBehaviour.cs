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

    public GameObject Add(GameObject gameObject)
    {
        GameObject go = Instantiate(gameObject, transform);
        UpdateAlign();
        return go;
    }

    public void Remove(int index)
    {
        Destroy(transform.GetChild(index).gameObject);
        UpdateAlign();
    }

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
        
        foreach(Transform child in transform)
        {
            child.transform.position = transform.position + Position + Vector3.Scale(offset, axis) * order;

            if(child.TryGetComponent<Mesh>(out Mesh component))
            {
                offset += Vector3.Scale(component.bounds.size + child.transform.localScale + cOffset, axis);
            }
            else
            {
                offset += Vector3.Scale(child.transform.localScale + cOffset, axis);
            }

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