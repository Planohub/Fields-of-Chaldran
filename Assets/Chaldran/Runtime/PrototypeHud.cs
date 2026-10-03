using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaldran
{
    public sealed class PrototypeHud : MonoBehaviour
    {
        private PrototypeRun run;
        private TMP_FontAsset font;
        private RectTransform root, worldRect, essenceFill, resonanceFill, enemyFill;
        private TextMeshProUGUI essenceText, resonanceText, objective, prompt, message, ability, enemyText, overlayTitle, overlayBody;
        private GameObject overlay;
        private GameObject enemyPanel;
        private Image arrivalCurtain;
        private GameObject dialogueOverlay;
        private TextMeshProUGUI dialogueSpeaker, dialogueBody, dialogueControls;
        private float arrivalUntil;
        private RenderTexture buffer;
        private readonly List<FloatingNumber> numbers = new List<FloatingNumber>();
        private readonly Color panelColor = new Color(0.025f, 0.04f, 0.075f, 0.94f);
        private readonly Color ink = new Color(0.86f, 0.93f, 0.95f);

        private sealed class FloatingNumber
        {
            public TextMeshProUGUI Text;
            public Vector3 Position;
            public float Age;
        }

        public void Initialize(PrototypeRun owner, TMP_FontAsset fontAsset)
        {
            run = owner;
            font = fontAsset;
            GameObject canvasObject = new GameObject("Trial HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            canvasObject.transform.SetParent(transform, false);
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
            root = canvasObject.GetComponent<RectTransform>();
            Panel("Screen background", root, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Color.black, true);

            worldRect = Rect("World image", root, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            AspectRatioFitter aspect = worldRect.gameObject.AddComponent<AspectRatioFitter>();
            aspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            aspect.aspectRatio = 16f / 9f;
            buffer = new RenderTexture(run.Presentation.bufferWidth, run.Presentation.bufferHeight, 24)
                { name = "Chaldran presentation buffer", filterMode = FilterMode.Point };
            buffer.Create();
            RawImage view = worldRect.gameObject.AddComponent<RawImage>();
            view.texture = buffer;
            view.raycastTarget = false;
            run.WorldCamera.targetTexture = buffer;

            RectTransform stats = Panel("Player status", root, Vector2.up, Vector2.up, new Vector2(24, -24), new Vector2(306, 152), panelColor);
            Text("Game title", stats, new Vector2(14, -12), new Vector2(278, 26), 20, "FIELDS OF CHALDRAN", ink);
            Text("Location", stats, new Vector2(14, -42), new Vector2(278, 20), 13,
                run.Presentation.displayName, run.Presentation.hudAccent);
            essenceFill = Bar(stats, new Vector2(14, -73), new Vector2(278, 25), new Color(0.61f, 0.2f, 0.26f));
            resonanceFill = Bar(stats, new Vector2(14, -109), new Vector2(278, 25), new Color(0.08f, 0.49f, 0.66f));
            essenceText = Text("Essence", stats, new Vector2(22, -76), new Vector2(260, 20), 15, "", ink);
            resonanceText = Text("Resonance", stats, new Vector2(22, -112), new Vector2(260, 20), 15, "", ink);

            RectTransform directive = Panel("Directive", root, Vector2.one, Vector2.one, new Vector2(-24, -24), new Vector2(326, 152), panelColor);
            Text("Directive heading", directive, new Vector2(14, -12), new Vector2(298, 24), 16,
                run.IsOverland ? run.StoryDefinition.questTitle : "CURRENT DIRECTIVE", new Color(0.89f, 0.73f, 0.44f));
            objective = Text("Objective", directive, new Vector2(14, -44), new Vector2(298, 66), 17, "", ink);
            Text("Override", directive, new Vector2(14, -112), new Vector2(298, 26), 13,
                $"OVERRIDE {run.Stats.Vitals.Override:0}  /  CRIT {run.Stats.Vitals.CritChance:P0}  /  PEN {run.Stats.Vitals.Penetration:P0}", new Color(0.5f, 0.72f, 0.78f));

            RectTransform enemy = Panel("Enemy status", root, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -24), new Vector2(240, 76), panelColor);
            enemyPanel = enemy.gameObject;
            enemyPanel.SetActive(!run.IsOverland);
            Text("Enemy heading", enemy, new Vector2(12, -10), new Vector2(216, 22), 15, "CLOCKWORK SENTINEL", new Color(0.93f, 0.73f, 0.44f));
            enemyFill = Bar(enemy, new Vector2(12, -37), new Vector2(216, 13), new Color(0.78f, 0.46f, 0.18f));
            enemyText = Text("Enemy health", enemy, new Vector2(12, -53), new Vector2(216, 18), 12, "", ink);

            RectTransform controls = Panel("Controls", root, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 18), new Vector2(710, 62), panelColor);
            Text("Controls text", controls, new Vector2(14, -9), new Vector2(682, 46), 15,
                "WASD move   SHIFT sprint   MOUSE aim   LMB strike   RMB hold block\nE use   Q burst   ESC pause   C checkpoint   F5 new trial   ENTER retry", ink);
            prompt = CenterText("Interaction prompt", new Vector2(0.5f, 0), new Vector2(0, 100), new Vector2(760, 34), 21, new Color(0.55f, 1f, 0.88f));
            message = CenterText("Notice", new Vector2(0.5f, 0), new Vector2(0, 148), new Vector2(860, 50), 18, ink);
            RectTransform skill = Panel("Ability", root, Vector2.right, Vector2.right, new Vector2(-24, 18), new Vector2(214, 62), panelColor);
            ability = Text("Ability status", skill, new Vector2(12, -9), new Vector2(190, 46), 16, "", new Color(1f, 0.76f, 0.43f));

            overlay = Panel("Paused or finished", root, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero,
                new Color(0.02f, 0.03f, 0.06f, 0.92f), true).gameObject;
            overlayTitle = CenterText("Overlay title", new Vector2(0.5f, 0.5f), new Vector2(0, 52), new Vector2(900, 70), 38, new Color(0.82f, 0.94f, 0.9f), overlay.transform);
            overlayBody = CenterText("Overlay instructions", new Vector2(0.5f, 0.5f), new Vector2(0, -38), new Vector2(820, 100), 22, ink, overlay.transform);
            overlay.SetActive(false);
            dialogueOverlay = Panel("Dialogue shade", root, Vector2.zero, Vector2.zero, Vector2.zero,
                Vector2.zero, new Color(0f, 0f, 0f, 0.25f), true).gameObject;
            RectTransform conversation = Panel("Conversation", dialogueOverlay.transform,
                new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 105), new Vector2(940, 292), panelColor);
            dialogueSpeaker = Text("Speaker", conversation, new Vector2(24, -22), new Vector2(892, 28), 22, "", run.Presentation.hudAccent);
            dialogueBody = Text("Dialogue text", conversation, new Vector2(24, -65), new Vector2(892, 160), 22, "", ink);
            dialogueControls = Text("Dialogue controls", conversation, new Vector2(24, -248), new Vector2(892, 28), 16, "", new Color(0.89f, 0.73f, 0.44f));
            dialogueOverlay.SetActive(false);
            if (run.IsOverland)
            {
                arrivalCurtain = Panel("Directory arrival", root, Vector2.zero, Vector2.zero, Vector2.zero,
                    Vector2.zero, Color.black, true).GetComponent<Image>();
                arrivalUntil = Time.unscaledTime + 0.8f;
            }
            run.Stats.Changed += RefreshStats;
            run.Changed += RefreshObjective;
            RefreshStats();
            RefreshObjective();
        }

        private void RefreshStats()
        {
            PlayerVitals values = run.Stats.Vitals;
            Fill(essenceFill, values.Essence / values.MaxEssence);
            Fill(resonanceFill, values.Resonance / values.MaxResonance);
            essenceText.text = $"ESSENCE  {values.Essence:0} / {values.MaxEssence:0}";
            resonanceText.text = $"RESONANCE  {values.Resonance:0} / {values.MaxResonance:0}";
        }

        private void RefreshObjective() { objective.text = run.Objective; }

        private void Update()
        {
            if (run == null || root == null) return;
            bool talking = run.Dialogue.IsOpen;
            dialogueOverlay.SetActive(talking);
            if (talking)
            {
                DialogueSession session = run.Dialogue.Session;
                DialogueLine line = run.Dialogue.Current.lines[session.LineIndex];
                dialogueSpeaker.text = line.speaker;
                dialogueBody.text = line.text;
                dialogueControls.text = $"{session.LineIndex + 1} / {session.LineCount}    E or ENTER: next    ESC: close";
            }
            prompt.text = talking ? "" : run.IsOverland && run.Story.CompletedSteps == 0 ? "[E] Take in the changed world"
                : run.Interactor.Target != null ? run.Interactor.Target.Prompt : "";
            message.text = Time.unscaledTime < run.MessageUntil ? run.Message : "";
            ability.text = !run.Progress.HasWeapon ? "Q  ASHEN BURST\nRecover your weapon"
                : run.Combat.BurstCooldownRemaining > 0f ? $"Q  ASHEN BURST\n{run.Combat.BurstCooldownRemaining:0.0}s cooldown"
                : $"Q  ASHEN BURST\nReady / {run.Balance.burstCost:0} Resonance";
            if (run.Sentinel != null)
            {
                Fill(enemyFill, run.Sentinel.Essence / run.Sentinel.MaxEssence);
                enemyText.text = run.Sentinel.IsAlive ? $"{run.Sentinel.Essence:0} / {run.Sentinel.MaxEssence:0} ESSENCE" : "CONTAINMENT UNIT DISABLED";
            }
            bool visible = run.IsTransitioning || run.IsPaused || !run.Stats.Vitals.IsAlive || (!run.IsOverland && run.Progress.Complete);
            overlay.SetActive(visible);
            if (visible)
            {
                overlayTitle.text = run.IsTransitioning ? "QUARANTINE RELEASED"
                    : (!run.IsOverland && run.Progress.Complete) ? "PERMISSION GRANTED" : run.IsPaused ? "SIMULATION SUSPENDED" : "DIVINE SIGNAL LOST";
                overlayBody.text = run.IsTransitioning ? "Your personal containment has ended.\nThe User Directory is resolving."
                    : (!run.IsOverland && run.Progress.Complete) ? "Quarantine Trial complete.\nPress ENTER to retry."
                    : run.IsPaused ? "Press ESC to resume.\nC resumes your checkpoint; F5 starts a new trial."
                    : run.IsOverland ? "The divine signal has faded.\nPress ENTER to restore the overland checkpoint."
                    : "The Engine has reasserted containment.\nPress ENTER to retry the trial.";
            }
            if (arrivalCurtain != null)
            {
                float alpha = Mathf.Clamp01((arrivalUntil - Time.unscaledTime) / 0.8f);
                arrivalCurtain.color = new Color(0, 0, 0, alpha);
                arrivalCurtain.gameObject.SetActive(alpha > 0f);
            }
            for (int i = numbers.Count - 1; i >= 0; i--)
            {
                FloatingNumber number = numbers[i];
                number.Age += Time.deltaTime;
                if (number.Age > 0.75f) { Destroy(number.Text.gameObject); numbers.RemoveAt(i); continue; }
                Vector3 viewport = run.WorldCamera.WorldToViewportPoint(number.Position);
                number.Text.rectTransform.anchorMin = number.Text.rectTransform.anchorMax = new Vector2(viewport.x, viewport.y);
                number.Text.rectTransform.anchoredPosition = new Vector2(0, 24f + number.Age * 40f);
                number.Text.alpha = 1f - number.Age / 0.75f;
            }
        }

        public Vector2 PointerToWorld(Vector2 screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(worldRect, screen, null, out Vector2 local);
            Rect rect = worldRect.rect;
            float x = Mathf.Clamp01((local.x - rect.xMin) / Mathf.Max(1f, rect.width));
            float y = Mathf.Clamp01((local.y - rect.yMin) / Mathf.Max(1f, rect.height));
            return run.WorldCamera.ViewportToWorldPoint(new Vector3(x, y, -run.WorldCamera.transform.position.z));
        }

        public void DamageNumber(Vector3 position, float amount, string prefix, Color color)
        {
            if (numbers.Count >= 16) { Destroy(numbers[0].Text.gameObject); numbers.RemoveAt(0); }
            TextMeshProUGUI label = CenterText("Damage", new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(140, 28), 20, color, worldRect);
            label.text = prefix + Mathf.CeilToInt(amount);
            numbers.Add(new FloatingNumber { Text = label, Position = position });
        }

        private void OnDestroy()
        {
            if (run != null && run.Stats != null) run.Stats.Changed -= RefreshStats;
            if (run != null) run.Changed -= RefreshObjective;
            if (buffer != null) { buffer.Release(); Destroy(buffer); }
        }

        private static void Fill(RectTransform fill, float value)
        {
            fill.anchorMax = new Vector2(Mathf.Clamp01(value), 1);
        }

        private RectTransform Bar(Transform parent, Vector2 position, Vector2 size, Color color)
        {
            RectTransform backing = Panel("Bar background", parent, Vector2.up, Vector2.up, position, size, new Color(0.1f, 0.13f, 0.19f));
            return Panel("Bar fill", backing, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, color, true);
        }

        private RectTransform Panel(string name, Transform parent, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size, Color color, bool stretch = false)
        {
            RectTransform rect = Rect(name, parent, anchor, stretch ? Vector2.one : anchor, position, size);
            rect.pivot = pivot;
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private static RectTransform Rect(string name, Transform parent, Vector2 minimum, Vector2 maximum, Vector2 position, Vector2 size)
        {
            GameObject item = new GameObject(name, typeof(RectTransform));
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = minimum;
            rect.anchorMax = maximum;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private TextMeshProUGUI Text(string name, Transform parent, Vector2 position, Vector2 size, float fontSize, string value, Color color)
        {
            RectTransform rect = Rect(name, parent, Vector2.up, Vector2.up, position, size);
            rect.pivot = Vector2.up;
            TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.fontSize = fontSize;
            text.color = color;
            text.text = value;
            text.raycastTarget = false;
            text.alignment = TextAlignmentOptions.TopLeft;
            return text;
        }

        private TextMeshProUGUI CenterText(string name, Vector2 anchor, Vector2 position, Vector2 size, float fontSize, Color color, Transform parent = null)
        {
            TextMeshProUGUI text = Text(name, parent != null ? parent : root, Vector2.zero, size, fontSize, "", color);
            text.rectTransform.anchorMin = text.rectTransform.anchorMax = anchor;
            text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            text.rectTransform.anchoredPosition = position;
            text.alignment = TextAlignmentOptions.Center;
            return text;
        }
    }
}
