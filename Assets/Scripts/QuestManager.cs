using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime progress for one accepted quest. A plain class (not
/// ScriptableObject) since this is per-playthrough state, not a
/// reusable data asset - QuestData is the asset, this wraps it with
/// how far along the player currently is.
/// </summary>
public class QuestProgress
{
    public QuestData quest;
    public int currentAmount;

    public bool IsComplete => currentAmount >= quest.requiredAmount;

    public QuestProgress(QuestData quest)
    {
        this.quest = quest;
        currentAmount = 0;
    }
}

/// <summary>
/// Tracks all accepted quests and their progress. Other systems
/// (pickups, enemy deaths, etc) call the Report methods when a
/// relevant event happens - QuestManager checks active quests for a
/// match and updates progress, then fires OnQuestsUpdated so any UI
/// (e.g. Quest_Log_UI) can refresh.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private readonly List<QuestProgress> activeQuests = new List<QuestProgress>();

    public event Action OnQuestsUpdated;

    public IReadOnlyList<QuestProgress> ActiveQuests => activeQuests;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Adds a quest to the active list, if it isn't already active or
    /// already completed. Call this from wherever quests get handed
    /// out (an NPC, a quest board, a trigger volume, etc).
    /// </summary>
    public void AcceptQuest(QuestData quest)
    {
        if (quest == null || IsQuestActive(quest))
        {
            return;
        }

        activeQuests.Add(new QuestProgress(quest));
        OnQuestsUpdated?.Invoke();
    }

    public bool IsQuestActive(QuestData quest)
    {
        foreach (QuestProgress progress in activeQuests)
        {
            if (progress.quest == quest)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Call this whenever the player picks up an item, so any active
    /// CollectItem quests targeting that item can advance.
    /// </summary>
    public void ReportItemCollected(string itemID, int amount)
    {
        bool anyChanged = false;

        foreach (QuestProgress progress in activeQuests)
        {
            if (progress.IsComplete)
            {
                continue;
            }

            if (progress.quest.questType == QuestType.CollectItem && progress.quest.targetID == itemID)
            {
                progress.currentAmount = Mathf.Min(progress.currentAmount + amount, progress.quest.requiredAmount);
                anyChanged = true;
            }
        }

        if (anyChanged)
        {
            OnQuestsUpdated?.Invoke();
        }
    }

    /// <summary>
    /// Call this whenever an enemy dies, so any active KillEnemy
    /// quests targeting that enemy type can advance.
    /// </summary>
    public void ReportEnemyKilled(string enemyID)
    {
        bool anyChanged = false;

        foreach (QuestProgress progress in activeQuests)
        {
            if (progress.IsComplete)
            {
                continue;
            }

            if (progress.quest.questType == QuestType.KillEnemy && progress.quest.targetID == enemyID)
            {
                progress.currentAmount = Mathf.Min(progress.currentAmount + 1, progress.quest.requiredAmount);
                anyChanged = true;
            }
        }

        if (anyChanged)
        {
            OnQuestsUpdated?.Invoke();
        }
    }
}
