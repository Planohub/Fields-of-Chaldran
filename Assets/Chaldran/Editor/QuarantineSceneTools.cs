using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Chaldran.Editor
{
    [CustomEditor(typeof(PrototypeBootstrap))]
    public sealed class QuarantineSceneTools : UnityEditor.Editor
    {
        [MenuItem("Fields of Chaldran/Open Quarantine Trial")]
        public static void OpenTrial()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene("Assets/Chaldran/Scenes/QuarantinePrototype.unity");
        }

        [MenuItem("Fields of Chaldran/Open Overland Preview")]
        public static void OpenOverland()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(JourneyStore.OverlandScene);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.HelpBox("Press Play to assemble and run the trial. Balance data, pixel art, sounds, and input bindings are checked in. The original SampleScene remains available.", MessageType.Info);
            if (GUILayout.Button("Select prototype balance"))
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<PrototypeBalance>("Assets/Chaldran/Data/QuarantineBalance.asset");
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawLayout(PrototypeBootstrap bootstrap, GizmoType type)
        {
            if (Application.isPlaying) return;
            if (bootstrap.gameObject.scene.path == JourneyStore.OverlandScene)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(25f, 17f, 0f));
                Marker(new Vector3(-9,-5), "Directory arrival", Color.cyan);
                Marker(new Vector3(-6,-4), "Checkpoint waystone", Color.yellow);
                Marker(new Vector3(-1,-4), "Damaged directory record", Color.yellow);
                Marker(new Vector3(7,4), "Library route marker", Color.green);
                Marker(new Vector3(7,6), "Compressed library threshold", Color.cyan);
                return;
            }
            Gizmos.color = new Color(0.3f, 0.65f, 0.75f);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(25f, 17f, 0f));
            Marker(new Vector3(-9, -5), "Avatar spawn", Color.cyan);
            Marker(new Vector3(-7, -5), "Weapon anomaly", Color.cyan);
            Marker(new Vector3(-9, 0), "Restoration relay", Color.green);
            Marker(new Vector3(0, -0.5f), "Clockwork sentinel", new Color(1f, 0.65f, 0.25f));
            Marker(new Vector3(7, 4), "Read-Only barrier", Color.red);
            Marker(new Vector3(7, 6), "Exit terminal", Color.cyan);
        }

        private static void Marker(Vector3 position, string label, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(position, 0.5f);
            Handles.Label(position + Vector3.up * 0.7f, label);
        }
    }
}
