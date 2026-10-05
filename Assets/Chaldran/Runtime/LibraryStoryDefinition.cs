using UnityEngine;

namespace Chaldran
{
    [CreateAssetMenu(menuName = "Fields of Chaldran/Library Story")]
    public sealed class LibraryStoryDefinition : ScriptableObject
    {
        public string questTitle;
        public string[] objectives;
        public string completedObjective;
        public DialogueDefinition arrival, contact, anchor, route;

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(questTitle) || string.IsNullOrWhiteSpace(completedObjective)
                    || objectives == null || objectives.Length != LibraryProgress.StepCount) return false;
                foreach (string objective in objectives)
                    if (string.IsNullOrWhiteSpace(objective)) return false;
                foreach (DialogueDefinition page in new[] { arrival, contact, anchor, route })
                    if (page == null || !page.IsValid) return false;
                return true;
            }
        }

        public string Objective(LibraryProgress state)
        {
            return state.Complete ? completedObjective : objectives[state.CompletedSteps];
        }

        public DialogueDefinition DialogueFor(LibraryBeat beat)
        {
            switch (beat)
            {
                case LibraryBeat.Arrival: return arrival;
                case LibraryBeat.OracleContact: return contact;
                case LibraryBeat.AnchorInterrupted: return anchor;
                case LibraryBeat.RouteReached: return route;
                default: return null; // Observing the spatial loop is a gameplay event.
            }
        }
    }
}
