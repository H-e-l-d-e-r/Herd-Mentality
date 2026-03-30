using System;

using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "Dialogue Table", menuName = "Dialogue/Dialogue Table")]
    public class DialogueTable : ScriptableObject
    {
        public string Name;
        public DialogueTableEntry[] Entries;

        [Serializable]
        public class DialogueTableEntry
        {
            public DialogueActor Actor;

            [TextArea]
            public string Text;
            public DialogueBehavior Behavior;
        }

        /// <summary>
        /// Process the entry array and convert it into a command linked list.
        /// </summary>
        /// <returns></returns>
        public DialogueCommand ToCommands()
        {
            if(Entries.Length == 0)
            {
                return null;
            }

            DialogueCommand root = DialogueCommand.CreateRoot(Name, Entries[0]);
            DialogueCommand next = root;
            for (int i = 1; i < Entries.Length; i++)
            {
                next.AddChild(Entries[i]);
                next = next.Next;
            }

            return root;
        }

        public static implicit operator DialogueCommand(DialogueTable table) => table.ToCommands();
    }
}