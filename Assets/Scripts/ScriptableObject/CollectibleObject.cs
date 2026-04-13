using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Collectible", menuName = "Herd Mentality/Collectible")]
public class CollectibleObject : ScriptableObject
{
    [Header("Meta")]
    public string Name;

    [TextArea]
    public string Description;

    public UndergroundGroups Like;
    public UndergroundGroups Dislike;

    [Serializable]
    public struct UndergroundGroups
    {
        public bool YoungLetterists;
        public bool SquatRoskoff;
        public bool Scilas;
    }
}
