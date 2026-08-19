using UnityEngine;

/// <summary>
/// TEMPORARY testing script - grants one or more quests automatically
/// when the scene starts, since there's no quest-giver NPC yet.
/// Delete this (and remove it from whatever object it's on) once real
/// quest-givers exist.
/// </summary>
public class Debug_Quest_Giver : MonoBehaviour
{
    [SerializeField] private QuestData[] questsToGrant;

    private void Start()
    {
        if (QuestManager.instance == null)
        {
            Debug.LogWarning("Debug_Quest_Giver: no QuestManager found in the scene.");
            return;
        }

        foreach (QuestData quest in questsToGrant)
        {
            if (quest != null)
            {
                QuestManager.instance.AcceptQuest(quest);
            }
        }
    }
}
