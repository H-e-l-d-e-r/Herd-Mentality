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

    public override bool Equals(object obj)
    {
        if(obj.GetType() != typeof(QuestObject)) return false;
        if(Vinyl != (obj as QuestObject).Vinyl) return false;
        if(!(Frequence > (obj as QuestObject).Frequence - GlobalGameSettings.Instance.FrequenceTreshold &&
            Frequence < (obj as QuestObject).Frequence + GlobalGameSettings.Instance.FrequenceTreshold)) return false;

        if(!(Orientation > (obj as QuestObject).Orientation - GlobalGameSettings.Instance.OrientationTreshold &&
            Orientation < (obj as QuestObject).Orientation + GlobalGameSettings.Instance.OrientationTreshold)) return false;
        Debug.Log("validate quest"); 
        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}