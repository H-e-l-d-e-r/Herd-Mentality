using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// UI tool for selecting an element from an enum. T must be an enum
/// </summary>
/// <typeparam name="T"></typeparam>
public class UISelector<T> : MonoBehaviour where T : struct, IConvertible
{   
    /// <summary>
    /// Return the current selected enum
    /// </summary>
    /// <value></value>
    public T Current
    {
        get => m_current;
        set => Select(value);
    }

    /// <summary>
    /// return the index of the current selected enum
    /// </summary>
    /// <value></value>
    public int Index
    {
        get => Array.IndexOf(GetValues(), m_current);
    }

    /// <summary>
    /// Return the number of elements
    /// </summary>
    /// <returns></returns>
    public int Count => Enum.GetValues(typeof(T)).Length;

    [Header("Components")]
    public RectTransform Viewport;
    public GameObject Template;
    public Button Left;
    public Button Right;

    public string[] NameOverloads;

    [Header("Events")]
    public UnityEvent<T> OnChange;

    private Dictionary<T, GameObject> m_values;
    private T m_current;

    void Start()
    {
        NullComponents.ThrowIfNull(Viewport);
        NullComponents.ThrowIfNull(Template);
        NullComponents.ThrowIfNull(Left);
        NullComponents.ThrowIfNull(Right);

        // try to remove unused children
        Viewport.RemoveAllChildren();

        // create the dictionnary containing all elements
        m_values = new Dictionary<T, GameObject>();

        // loop over each enum elements
        T[] Items = GetValues();
        for(int i = 0; i < Items.Length; i++)
        {
            // instiate an enum object
            GameObject @object = Instantiate(Template);
            string asString = Items[i].ToString();

            // check for names overloadings
            if(i < NameOverloads.Length && !string.IsNullOrEmpty(NameOverloads[i]))
            {
                asString = NameOverloads[i];
            }
            
            // update properties
            @object.name = asString;
            @object.transform.SetParent(Viewport, false);
            @object.SetActive(false);

            // update text
            if(@object.TryGetComponent<TMP_Text>(out TMP_Text text))
            {
                text.SetText(asString);
            }

            // add this to the collection
            m_values.Add(Items[i], @object);
        }

        // focus the first element
        Select(Items.First());

        // set callbacks
        Left.onClick.AddListener(Previous);
        Right.onClick.AddListener(Next);
    }

    void OnEnable()
    {
        // check for enum type
        if (!typeof(T).IsEnum)
        {
            Debug.LogError("T must be an enum!");
        }
    }

    /// <summary>
    /// Select an element
    /// </summary>
    /// <param name="value"></param>
    public void Select(T value)
    {
        if(m_values.TryGetValue(m_current, out GameObject @current))
        {
            @current.SetActive(false);
        }

        if(m_values.TryGetValue(value, out GameObject @object))
        {
            @object.SetActive(true);
            m_current = value;
        }

        OnChange.Invoke(value);
    }

    public void Select(int index)
    {
        T[] Items = GetValues();
        if(index < 0 || index >= Items.Length)
        {
            // out of bounds
            Debug.LogException(new IndexOutOfRangeException());
            return;    
        }

        Select(Items[index]);
    }

    public T[] GetValues()
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
    }

    void Next() => Select((int)Mathf.Repeat(Index + 1, Count));
    void Previous() => Select((int)Mathf.Repeat(Index - 1, Count));
}
