using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Populates the quest log with one Quest_Log_Entry_UI per active
/// quest, rebuilding the list whenever QuestManager.OnQuestsUpdated
/// fires (quest accepted, or any progress changed).
/// </summary>
public class Quest_Log_UI : MonoBehaviour
{
    [SerializeField] private Transform entryContainer; // parent the entries get instantiated under
    [SerializeField] private GameObject entryPrefab;   // prefab with a Quest_Log_Entry_UI component

    private readonly List<GameObject> spawnedEntries = new List<GameObject>();

    private void OnEnable()
    {
        QuestManager.instance.OnQuestsUpdated += Refresh;
        Refresh(); // show current state immediately when the panel opens
    }

    private void OnDisable()
    {
        QuestManager.instance.OnQuestsUpdated -= Refresh;
    }

    private void Refresh()
    {
        // Clear out the old entries before rebuilding - simplest
        // approach; fine for a quest log's typical size. Could be
        // optimized to reuse/pool entries later if the list ever gets
        // large enough for this to matter.
        foreach (GameObject entry in spawnedEntries)
        {
            Destroy(entry);
        }
        spawnedEntries.Clear();

        foreach (QuestProgress progress in QuestManager.instance.ActiveQuests)
        {
            GameObject entryObject = Instantiate(entryPrefab, entryContainer);
            entryObject.GetComponent<Quest_Log_Entry_UI>().Setup(progress);
            spawnedEntries.Add(entryObject);
        }
    }
}
