using System;
using UnityEngine;

namespace Chaldran
{
    [Serializable]
    public sealed class DialogueLine
    {
        public string speaker;
        [TextArea(3, 6)] public string text;
    }

    [CreateAssetMenu(menuName = "Fields of Chaldran/Dialogue")]
    public sealed class DialogueDefinition : ScriptableObject
    {
        public DialogueLine[] lines;
        public bool IsValid
        {
            get
            {
                if (lines == null || lines.Length < 1 || lines.Length > 64) return false;
                foreach (DialogueLine line in lines)
                    if (line == null || string.IsNullOrWhiteSpace(line.text) || line.text.Length > 500) return false;
                return true;
            }
        }
    }
}
