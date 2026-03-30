using System;

namespace DialogueSystem
{
    [Flags]
    public enum DialogueBehavior
    {
        WaitForInput,
        AutoSkip,
        EndOfDialogue,
        
        // default is wait for input
        None = WaitForInput,
    }
}