using System;
using DialogueSystem;
using Unity.Profiling;
using UnityEngine;

// [CreateAssetMenu(fileName = "Quest Object", menuName = "Herd Mentality/Quest Object")]
// public class QuestObject : CollectibleObject
// {
//     [Header("Quest")]
//     public VinylObject[] ConstraintVinyles = new VinylObject[4];
// 
//     public DialogueTable IntroductionTable;
//     public DialogueTable EndTable;
//     public QuestObject Next;
// }

[Serializable]
public class QuestObject
{
    public VinylObject Vinyl;
    public float Frequence;
    public float Orientation;

    public QuestObject(VinylObject vinyl, float freq, float orientation)
    {
        Vinyl = vinyl;
        Frequence = freq;
        Orientation = orientation;
    }

    public static bool operator ==(QuestObject a, QuestObject b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        return a.Equals(b);
    }   

    public static bool operator !=(QuestObject a, QuestObject b) => !(a == b);

    public override bool Equals(object obj)
    {
        Debug.Log("Check quest"); 

        if(obj == null || (obj as QuestObject).Vinyl == null || Vinyl == null) return false;
        if(obj.GetType() != typeof(QuestObject)) return false;
        if(Vinyl.Name != (obj as QuestObject).Vinyl.Name) return false;

        if (Mathf.Abs(Frequence - (obj as QuestObject).Frequence) > GlobalGameSettings.Instance.FrequenceTreshold) return false;
        if (Mathf.Abs(Orientation - (obj as QuestObject).Orientation) > GlobalGameSettings.Instance.OrientationTreshold) return false;

        Debug.Log("validate quest"); 
        return true;
    }

    public override int GetHashCode()
    {
        return Vinyl != null ? Vinyl.GetHashCode() : 0;
    }
}