using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Sequence", menuName = "Herd Mentality/Sequence")]
public class RadioSequenceObject : CollectibleObject
{
    public VinylObject[] Blocs;    
}