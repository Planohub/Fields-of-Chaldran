using UnityEngine;

namespace Chaldran
{
    public sealed class PrototypeVisuals
    {
        private readonly Sprite[] sprites = new Sprite[16];
        private readonly Material material;
        private readonly Transform root;

        public PrototypeVisuals(Texture2D atlas, Material spriteMaterial, Transform parent)
        {
            material = spriteMaterial;
            root = parent;
            for (int i = 0; i < sprites.Length; i++)
                sprites[i] = Sprite.Create(atlas, new Rect((i % 8) * 16, (1 - i / 8) * 16, 16, 16),
                    new Vector2(0.5f, 0.5f), 16f, 0, SpriteMeshType.FullRect);
        }

        public Sprite GetSprite(int index) { return sprites[index]; }

        public GameObject Make(string name, Vector2 position, int spriteIndex, Vector2 scale, int order = 100)
        {
            GameObject item = new GameObject(name);
            item.transform.SetParent(root, false);
            item.transform.position = position;
            item.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            SpriteRenderer renderer = item.AddComponent<SpriteRenderer>();
            renderer.sprite = sprites[spriteIndex];
            renderer.sharedMaterial = material;
            renderer.sortingOrder = order;
            return item;
        }

        public void Effect(string name, Vector2 position, int spriteIndex, float scale, float angle, Color color, float lifetime)
        {
            GameObject effect = Make(name, position, spriteIndex, Vector2.one * scale, 900);
            effect.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            effect.GetComponent<SpriteRenderer>().color = color;
            effect.AddComponent<TransientSpriteEffect>().Initialize(lifetime);
        }

        public void Dispose()
        {
            foreach (Sprite sprite in sprites) Object.Destroy(sprite);
        }
    }

}
