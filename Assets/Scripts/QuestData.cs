using UnityEngine;

/// <summary>
/// The kind of objective a quest tracks. Add new values here as you
/// need more quest types (e.g. TalkToNPC, ReachLocation) - QuestManager
/// only needs a new Report method and a matching case to support one.
/// </summary>
public enum QuestType
{
    CollectItem,
    KillEnemy
}

/// <summary>
/// Defines one quest: what it's called, what it asks for, and how much.
/// targetID means different things depending on questType:
/// - CollectItem -> matches an Item's itemID
/// - KillEnemy   -> matches an EnemyData's enemyID
/// </summary>
[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
public class QuestData : ScriptableObject
{
    public string questID;
    public string questName;

    [TextArea]
    public string description;

    public QuestType questType;
    public string targetID;
    public int requiredAmount = 1;
}
