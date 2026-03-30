using System;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue Actor", menuName = "Dialogue/Dialogue Actor")]
    public class DialogueActor : ScriptableObject
    {
        public string Name;
    }
}