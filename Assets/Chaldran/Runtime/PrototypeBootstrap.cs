using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

namespace Chaldran
{
    [DefaultExecutionOrder(-100)]
    public sealed class PrototypeBootstrap : MonoBehaviour
    {
        [SerializeField] private PrototypeBalance balance;
        [SerializeField] private PresentationProfile presentation;
        [SerializeField] private bool overland;
        [SerializeField] private DirectoryStoryDefinition directoryStory;
        [SerializeField] private LibraryStoryDefinition libraryStory;
        [SerializeField] private TMP_FontAsset font;
        [SerializeField] private Material spriteMaterial;
        [SerializeField] private InputActionAsset playerControls;
        private Tile floorTile, wallTile;

        private void Awake()
        {
            if (balance == null || presentation == null || !presentation.IsValid || font == null || spriteMaterial == null || playerControls == null
                || presentation.tier != (overland ? PresentationTier.Overland : PresentationTier.Quarantine)
                || (libraryStory != null && (!overland || !libraryStory.IsValid))
                || (overland && libraryStory == null && (directoryStory == null || !directoryStory.IsValid)))
            {
                Debug.LogError("Chaldran prototype is missing a required asset. Open a checked-in prototype scene.", this);
                enabled = false;
                return;
            }
            PrototypeVisuals visuals = new PrototypeVisuals(presentation, spriteMaterial, transform);
            PrototypeRun run = gameObject.AddComponent<PrototypeRun>();
            run.Initialize(balance, visuals, presentation, overland, directoryStory, libraryStory);
            run.Dialogue = gameObject.AddComponent<PrototypeDialogue>();
            run.Dialogue.Initialize(run);
            run.Audio = gameObject.AddComponent<PrototypeAudio>();
            run.Audio.Initialize(presentation);
            BuildWorld(visuals);

            GameObject player = visuals.Make("Awakened avatar", new Vector2(-9, libraryStory != null ? 0 : -5), 2, Vector2.one);
            player.SetActive(false);
            player.layer = WorldQuery.ActorLayer;
            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            player.AddComponent<CircleCollider2D>().radius = 0.32f;
            player.AddComponent<PrototypeYSort>();
            run.Stats = player.AddComponent<PlayerStats>();
            run.Stats.Initialize(balance, run);
            JourneyCheckpoint checkpoint = null;
            if (overland)
            {
                checkpoint = JourneyStore.TakePending();
                if (checkpoint == null) JourneyStore.TryLoad(out checkpoint);
                JourneyLocation expected = run.IsLibrary ? JourneyLocation.OracleLibrary : JourneyLocation.UserDirectory;
                if (checkpoint != null && (!checkpoint.IsValid || checkpoint.location != expected)) checkpoint = null;
                run.Progress.RestoreCompletedTutorial();
                if (run.IsLibrary) run.Story.Restore(StoryProgress.StepCount);
                if (checkpoint != null)
                {
                    run.Story.Restore(checkpoint.storyStep);
                    if (run.IsLibrary) run.Library.Restore(checkpoint.libraryStep);
                    run.Stats.Vitals.LoadResources(checkpoint.essence, checkpoint.resonance);
                    body.position = new Vector2(checkpoint.x, checkpoint.y);
                    player.transform.position = new Vector2(checkpoint.x, checkpoint.y);
                }
            }
            run.Motor = player.AddComponent<PlayerMotor2D>();
            run.Motor.Initialize(run);
            run.Combat = player.AddComponent<PlayerCombat>();
            run.Combat.Initialize(run);
            run.Interactor = player.AddComponent<PlayerInteractor>();
            run.Interactor.Initialize(run);
            player.AddComponent<AvatarPresentation>().Initialize(run);

            GameObject cameraObject = new GameObject("Trial camera", typeof(Camera), typeof(AudioListener), typeof(UniversalAdditionalCameraData));
            cameraObject.transform.SetParent(transform, false);
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0, -1.75f, -10f);
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 6.75f;
            camera.aspect = 16f / 9f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = presentation.background;
            PrototypeCamera2D follow = cameraObject.AddComponent<PrototypeCamera2D>();
            follow.Target = player.transform;
            follow.PixelsPerUnit = presentation.cellPixels;
            run.WorldCamera = camera;

            run.Hud = gameObject.AddComponent<PrototypeHud>();
            run.Hud.Initialize(run, font);
            if (run.IsLibrary)
            {
                BuildLibraryInteractions(run);
                player.AddComponent<PlayerInputRouter>().Initialize(run, playerControls);
                player.SetActive(true);
                run.ShowMessage(checkpoint != null ? "ARCHIVE CHECKPOINT RESTORED. " + run.Objective
                    : "MOUNT /oracle/archive: granted. Route integrity unresolved.", 6f);
                return;
            }
            if (overland)
            {
                SpawnInteractable(run, "Directory waystone", new Vector2(-6, -4), 8, TrialInteraction.Checkpoint)
                    .AddComponent<StoryObjectiveMarker>().Initialize(run, DirectoryBeat.Waystone);
                SpawnInteractable(run, "Restoration spring", new Vector2(-9, 0), 5, TrialInteraction.RestorationRelay);
                SpawnInteractable(run, "Damaged directory record", new Vector2(-1, -4), 6, TrialInteraction.SignalRecord)
                    .AddComponent<StoryObjectiveMarker>().Initialize(run, DirectoryBeat.SignalRecord);
                SpawnInteractable(run, "Library route marker", new Vector2(7, 4), 4, TrialInteraction.Landmark)
                    .AddComponent<StoryObjectiveMarker>().Initialize(run, DirectoryBeat.RouteMarker);
                SpawnInteractable(run, "Compressed library threshold", new Vector2(7, 6), 8, TrialInteraction.LibraryThreshold)
                    .AddComponent<StoryObjectiveMarker>().Initialize(run, DirectoryBeat.LibraryThreshold);
                player.AddComponent<PlayerInputRouter>().Initialize(run, playerControls);
                player.SetActive(true);
                run.ShowMessage(checkpoint != null ? "User Directory restored. " + run.Objective
                    : "Your personal quarantine has ended. The User Directory stretches ahead.", 7f);
                return;
            }
            SpawnInteractable(run, "Suppressed weapon anomaly", new Vector2(-7, -5), 4, TrialInteraction.Anomaly);
            SpawnInteractable(run, "Restoration relay", new Vector2(-9, 0), 5, TrialInteraction.RestorationRelay);
            GameObject gate = SpawnInteractable(run, "Read-Only barrier", new Vector2(7, 4), 7, TrialInteraction.Barrier, new Vector2(3, 1));
            GameObject blocker = new GameObject("Barrier collision");
            blocker.transform.SetParent(gate.transform, false);
            blocker.layer = WorldQuery.SolidLayer;
            BoxCollider2D barrier = blocker.AddComponent<BoxCollider2D>();
            barrier.size = Vector2.one;
            gate.GetComponent<PrototypeInteractable>().Initialize(run, TrialInteraction.Barrier, barrier);
            SpawnInteractable(run, "Exit terminal", new Vector2(7, 6), 8, TrialInteraction.Exit);

            GameObject sentinel = visuals.Make("Clockwork sentinel", new Vector2(0, -0.5f), 3, Vector2.one * 1.4f);
            sentinel.layer = WorldQuery.ActorLayer;
            Rigidbody2D sentinelBody = sentinel.AddComponent<Rigidbody2D>();
            sentinelBody.gravityScale = 0f;
            sentinelBody.mass = 8f;
            sentinelBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            sentinelBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            sentinelBody.interpolation = RigidbodyInterpolation2D.Interpolate;
            sentinel.AddComponent<CircleCollider2D>().radius = 0.35f;
            sentinel.AddComponent<PrototypeYSort>();
            run.Sentinel = sentinel.AddComponent<SentinelEnemy>();
            run.Sentinel.Initialize(run);
            player.AddComponent<PlayerInputRouter>().Initialize(run, playerControls);
            player.SetActive(true);
            run.ShowMessage("Approach the cyan anomaly and press E. Your weapon is waiting.", 6f);
        }

        private void BuildWorld(PrototypeVisuals visuals)
        {
            GameObject gridObject = new GameObject("Trial tile grid", typeof(Grid));
            gridObject.transform.SetParent(transform, false);
            gridObject.transform.position = new Vector3(-12.5f, -8.5f, 0f);
            Tilemap ground = Tilemap("Ground", gridObject.transform, 0);
            Tilemap walls = Tilemap("Containment walls", gridObject.transform, 10);
            ground.GetComponent<TilemapRenderer>().sharedMaterial = spriteMaterial;
            walls.GetComponent<TilemapRenderer>().sharedMaterial = spriteMaterial;
            floorTile = ScriptableObject.CreateInstance<Tile>();
            floorTile.sprite = visuals.GetSprite(libraryStory != null ? 14 : 0);
            if (libraryStory != null) floorTile.color = new Color(0.55f, 0.65f, 0.85f);
            wallTile = ScriptableObject.CreateInstance<Tile>();
            wallTile.sprite = visuals.GetSprite(libraryStory != null ? 12 : 1);
            for (int x = -12; x <= 12; x++)
            for (int y = -8; y <= 8; y++)
            {
                Vector3Int cell = new Vector3Int(x + 12, y + 8, 0);
                ground.SetTile(cell, floorTile);
                bool boundary = x == -12 || x == 12 || y == -8 || y == 8;
                bool partition = !overland && y == 4 && (x < 6 || x > 8);
                if (!boundary && !partition) continue;
                walls.SetTile(cell, wallTile);
                Solid("Containment collision", new Vector2(x, y), Vector2.one);
            }
            if (libraryStory != null)
            {
                // One physical corridor loops until its anchor is interrupted.
                foreach (int y in new[] { -2, 2 })
                for (int x = -11; x <= 2; x++)
                {
                    visuals.Make("Archive corridor wall", new Vector2(x, y), 12, Vector2.one, 110);
                    Solid("Corridor collision", new Vector2(x, y), Vector2.one);
                }
                for (int y = -7; y <= 7; y++)
                {
                    if (y >= -1 && y <= 1) continue;
                    visuals.Make("Restricted archive partition", new Vector2(3, y), 12, Vector2.one, 110);
                    Solid("Archive partition collision", new Vector2(3, y), Vector2.one);
                }
                foreach (float x in new[] { -8f, -5f, -2f, 6f, 9f })
                foreach (float y in new[] { -4f, 4f })
                {
                    GameObject shelf = visuals.Make("Graybox archive shelf", new Vector2(x, y), 12, new Vector2(2f, 0.9f));
                    shelf.AddComponent<PrototypeYSort>();
                    Solid("Shelf collision", new Vector2(x, y), new Vector2(2f, 0.8f));
                }
                GameObject book = visuals.Make("Repeated red index book", new Vector2(-7, 0.9f), 6, Vector2.one * 0.55f);
                book.GetComponent<SpriteRenderer>().color = new Color(1f, 0.3f, 0.35f);
                return;
            }
            if (overland)
            {
                foreach (Vector2 position in new[] { new Vector2(-7, 2), new Vector2(-3, 4), new Vector2(3, 2), new Vector2(8, -3), new Vector2(-2, -5) })
                {
                    GameObject tree = visuals.Make("Directory tree", position, 1, Vector2.one * 2.3f);
                    tree.AddComponent<PrototypeYSort>();
                    Solid("Tree trunk collision", position + Vector2.down * 0.55f, new Vector2(0.65f, 0.5f));
                }
                for (int x = -10; x <= 9; x++) visuals.Make("Old directory path", new Vector2(x, -4), 14, Vector2.one, 1);
                for (int y = -3; y <= 6; y++) visuals.Make("Library approach", new Vector2(7, y), 14, Vector2.one, 1);
                foreach (float x in new[] { 5.5f, 8.5f })
                {
                    GameObject stone = visuals.Make("Library threshold stone", new Vector2(x, 6), 12, Vector2.one * 1.5f);
                    stone.AddComponent<PrototypeYSort>();
                    Solid("Threshold stone collision", new Vector2(x, 6), new Vector2(0.65f, 0.75f));
                }
                return;
            }
            foreach (Vector2 position in new[] { new Vector2(-4, -1), new Vector2(4, -1) })
            {
                GameObject pillar = visuals.Make("Containment pillar", position, 12, Vector2.one);
                pillar.AddComponent<PrototypeYSort>();
                Solid("Pillar collision", position, Vector2.one * 0.85f);
            }
            foreach (Vector2 position in new[] { new Vector2(-10, -6), new Vector2(10, -6), new Vector2(-10, 2), new Vector2(10, 2) })
                visuals.Make("Floor rune", position, 14, Vector2.one, 1);
        }

        private GameObject SpawnInteractable(PrototypeRun run, string name, Vector2 position, int index, TrialInteraction kind, Vector2? scale = null)
        {
            GameObject item = run.Visuals.Make(name, position, index, scale ?? Vector2.one);
            item.layer = WorldQuery.InteractableLayer;
            CircleCollider2D trigger = item.AddComponent<CircleCollider2D>();
            trigger.radius = 0.45f;
            trigger.isTrigger = true;
            item.AddComponent<PrototypeInteractable>().Initialize(run, kind);
            item.AddComponent<PrototypeYSort>();
            return item;
        }

        private void BuildLibraryInteractions(PrototypeRun run)
        {
            SpawnInteractable(run, "Archive checkpoint terminal", new Vector2(-9, -0.9f), 8, TrialInteraction.Checkpoint);
            SpawnInteractable(run, "Restricted Oracle channel", new Vector2(-5, 0), 8, TrialInteraction.OracleChannel)
                .AddComponent<StoryObjectiveMarker>().InitializeLibrary(run, LibraryBeat.OracleContact);
            SpawnInteractable(run, "Loop anchor binding", new Vector2(-1, 0), 4, TrialInteraction.LoopAnchor)
                .AddComponent<StoryObjectiveMarker>().InitializeLibrary(run, LibraryBeat.AnchorInterrupted);
            SpawnInteractable(run, "Resolved archive landing", new Vector2(6, 0), 8, TrialInteraction.ArchiveLanding)
                .AddComponent<StoryObjectiveMarker>().InitializeLibrary(run, LibraryBeat.RouteReached);
            GameObject portal = run.Visuals.Make("Virtualized route", new Vector2(1.5f, 0), 7, new Vector2(0.7f, 2.7f));
            portal.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.65f, 1f, 0.35f);
            portal.layer = WorldQuery.InteractableLayer;
            BoxCollider2D trigger = portal.AddComponent<BoxCollider2D>(); trigger.isTrigger = true;
            portal.AddComponent<LibraryLoopPortal>().Initialize(run, new Vector2(-8.5f, 0f));
            portal.AddComponent<StoryObjectiveMarker>().InitializeLibrary(run, LibraryBeat.LoopObserved);
            GameObject gate = run.Visuals.Make("Anchor-owned route barrier", new Vector2(3, 0), 7, new Vector2(0.7f, 3f));
            GameObject solid = new GameObject("Archive route collision");
            solid.transform.SetParent(gate.transform, false); solid.layer = WorldQuery.SolidLayer;
            BoxCollider2D blocker = solid.AddComponent<BoxCollider2D>();
            gate.AddComponent<LibraryRouteGate>().Initialize(run, blocker);
        }

        private void Solid(string name, Vector2 position, Vector2 size)
        {
            GameObject item = new GameObject(name);
            item.transform.SetParent(transform, false);
            item.transform.position = position;
            item.layer = WorldQuery.SolidLayer;
            item.AddComponent<BoxCollider2D>().size = size;
        }

        private static Tilemap Tilemap(string name, Transform parent, int order)
        {
            GameObject item = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
            item.transform.SetParent(parent, false);
            item.GetComponent<TilemapRenderer>().sortingOrder = order;
            return item.GetComponent<Tilemap>();
        }

        private void OnDestroy()
        {
            if (floorTile != null) Destroy(floorTile);
            if (wallTile != null) Destroy(wallTile);
        }
    }
}
