using UnityEngine;

namespace Chaldran
{
    [CreateAssetMenu(menuName = "Fields of Chaldran/Presentation Profile")]
    public sealed class PresentationProfile : ScriptableObject
    {
        public PresentationTier tier;
        public string displayName;
        public Texture2D atlas;
        [Min(8)] public int cellPixels = 16;
        [Min(1)] public int columns = 8;
        [Min(1)] public int rows = 2;
        [Min(64)] public int bufferWidth = 256;
        [Min(36)] public int bufferHeight = 144;
        public Color background = new Color(0.015f, 0.025f, 0.04f);
        public Color hudAccent = new Color(0.42f, 0.78f, 0.8f);
        public AudioClip[] sounds;
        public AudioClip music;
        [Range(0f, 1f)] public float musicVolume = 0.12f;
        public int[] walkingFrames = { 2 };
        [Header("Dialogue")]
        public bool retroDialogue;
        [Range(0f, 200f)] public float charactersPerSecond;
        public AudioClip typingSound;
        [Range(0f, 1f)] public float typingVolume = 0.14f;
        public DialogueDefinition introduction;

        public bool IsValid => atlas != null && cellPixels >= 8 && columns >= 1 && rows >= 1
            && columns * rows >= 16 && atlas.width == columns * cellPixels
            && atlas.height == rows * cellPixels && bufferWidth >= 64 && bufferHeight >= 36
            && bufferWidth * 9L == bufferHeight * 16L;
    }
}
