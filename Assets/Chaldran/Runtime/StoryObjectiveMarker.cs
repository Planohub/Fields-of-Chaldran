using UnityEngine;

namespace Chaldran
{
    // Uses the current tier's artwork; the marker is not an interaction collider.
    public sealed class StoryObjectiveMarker : MonoBehaviour
    {
        private PrototypeRun run;
        private DirectoryBeat beat;
        private LibraryBeat libraryBeat;
        private bool library;
        private GameObject marker;
        public void Initialize(PrototypeRun owner, DirectoryBeat target)
        {
            run = owner;
            beat = target;
            BuildMarker();
        }
        public void InitializeLibrary(PrototypeRun owner, LibraryBeat target)
        {
            run = owner; library = true; libraryBeat = target;
            BuildMarker();
        }
        private void BuildMarker()
        {
            marker = new GameObject("Current story objective", typeof(SpriteRenderer));
            marker.transform.SetParent(transform, false);
            marker.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            marker.transform.localScale = Vector3.one * 0.35f;
            SpriteRenderer sprite = marker.GetComponent<SpriteRenderer>();
            sprite.sprite = run.Visuals.GetSprite(6);
            sprite.sharedMaterial = GetComponent<SpriteRenderer>().sharedMaterial;
            sprite.color = new Color(1f, 0.86f, 0.45f);
            sprite.sortingOrder = 1000;
            Refresh();
        }

        private void LateUpdate() { Refresh(); }
        private void Refresh()
        {
            if (marker == null || run == null) return;
            marker.SetActive(library ? run.Library.CompletedSteps == (int)libraryBeat
                : run.Story.CompletedSteps == (int)beat || (beat == DirectoryBeat.Waystone && run.Story.CompletedSteps == 0)
                    || (beat == DirectoryBeat.LibraryThreshold && run.Story.Complete));
        }
    }
}
