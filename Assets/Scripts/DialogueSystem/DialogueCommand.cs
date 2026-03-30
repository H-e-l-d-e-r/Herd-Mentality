namespace DialogueSystem
{
    public class DialogueCommand
    {
        public bool IsRoot => Root == this;
        public DialogueState State => m_state;

        public string Name;
        public string Text;
        public DialogueActor Actor;

        public DialogueBehavior Behavior; 

        public DialogueCommand Root;
        public DialogueCommand Next;

        private DialogueState m_state;

        private DialogueCommand(DialogueCommand root, DialogueCommand next)
        {
            Name = string.Empty;
            Text = string.Empty;
            Behavior = DialogueBehavior.None;

            Actor = null;

            Root = root;
            Next = next;

            m_state = DialogueState.Hidden;
        }

        public void ProcessCommand(DialogueState newState)
        {
            m_state = newState;
        }

        /// <summary>
        /// Enqueue a new child
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public DialogueCommand AddChild(DialogueTable.DialogueTableEntry entry)
        {
            Next = new DialogueCommand(Root, null);
            Next.Actor = entry.Actor;
            Next.Text = entry.Text;
            Next.Behavior = entry.Behavior;
            Next.Name = Root.Name;

            return Next;
        }

        /// <summary>
        /// Clear all children
        /// </summary>
        public void Clear()
        {
            Next.Clear();
            Next = null;
        }

        /// <summary>
        /// Create an empty root command 
        /// </summary>
        /// <returns></returns>
        public static DialogueCommand CreateRoot(string name)
        {
            DialogueCommand self = new DialogueCommand(null, null);
            self.Root = self;
            self.Name = name;
            
            return self;
        }

        /// <summary>
        /// Create a new command from an entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static DialogueCommand CreateRoot(string name, DialogueTable.DialogueTableEntry entry)
        {
            DialogueCommand self = new DialogueCommand(null, null);
            self.Root = self;
            self.Name = name;
            self.Actor = entry.Actor;
            self.Text = entry.Text;
            self.Behavior = entry.Behavior;
            
            return self;
        }
    }
}