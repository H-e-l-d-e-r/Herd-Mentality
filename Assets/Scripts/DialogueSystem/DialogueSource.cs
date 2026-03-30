using UnityEngine;

namespace DialogueSystem
{
    public class DialogueSource : MonoBehaviour
    {
        public DialogueTable Dialogue;

        private DialoguePtr m_ptr;

        void Start()
        {
            m_ptr = DialogueSystem.Dialogue.RegisterDialogue(Dialogue);
        }

        public void PlayDialogue()
        {
            DialogueSystem.Dialogue.PlayDialogue(m_ptr);
        }
    }    
}