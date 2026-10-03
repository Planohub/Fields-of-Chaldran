using System;
using UnityEngine;

namespace Chaldran
{
    [Serializable]
    public sealed class DirectoryStoryStep
    {
        public DirectoryBeat beat;
        public string objective;
        public DialogueDefinition dialogue;
    }

    [CreateAssetMenu(menuName = "Fields of Chaldran/Directory Story")]
    public sealed class DirectoryStoryDefinition : ScriptableObject
    {
        public string questTitle;
        public DirectoryStoryStep[] steps;
        public string completedObjective;
        public bool IsValid
        {
            get
            {
                if (steps == null || steps.Length != StoryProgress.StepCount || string.IsNullOrWhiteSpace(questTitle)
                    || string.IsNullOrWhiteSpace(completedObjective)) return false;
                for (int i = 0; i < steps.Length; i++)
                    if (steps[i] == null || (int)steps[i].beat != i || string.IsNullOrWhiteSpace(steps[i].objective)
                        || steps[i].dialogue == null || !steps[i].dialogue.IsValid) return false;
                return true;
            }
        }

        public string Objective(StoryProgress progress)
        {
            return progress.Complete ? completedObjective : steps[progress.CompletedSteps].objective;
        }
    }
}
