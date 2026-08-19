using UnityEngine;
using TMPro;

/// <summary>
/// Displays one QuestProgress in the quest log list. Attach to the
/// quest log entry prefab alongside its text components.
/// </summary>
public class Quest_Log_Entry_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI progressText;

    public void Setup(QuestProgress progress)
    {
        nameText.text = progress.quest.questName;
        descriptionText.text = progress.quest.description;
        progressText.text = $"{progress.currentAmount}/{progress.quest.requiredAmount}";

        // Simple visual cue for a finished quest - tweak to taste
        // (strike-through, a checkmark icon, a different color, etc).
        if (progress.IsComplete)
        {
            progressText.text += " (Complete)";
        }
    }
}
