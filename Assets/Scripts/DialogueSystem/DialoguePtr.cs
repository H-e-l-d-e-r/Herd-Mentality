namespace DialogueSystem
{
    public struct DialoguePtr
    {
        public static DialoguePtr k_INVALID => new DialoguePtr(-1);

        public int Handle;
        public DialogueTable Origin;

        public DialoguePtr(int value)
        {
            Handle = value;
            Origin = null;
        }

        public DialoguePtr(int value, DialogueTable table)
        {
            Handle = value;
            Origin = table;
        }

        public static bool IsValid(DialoguePtr ptr)
        {
            return ptr > k_INVALID;
        }

        public static implicit operator int(DialoguePtr ptr) => ptr.Handle; 
    }
}