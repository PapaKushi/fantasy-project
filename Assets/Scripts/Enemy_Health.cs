using System;
using UnityEngine;

/// <summary>
/// Basic health for an enemy. Call TakeDamage() when it's hit; the
/// object destroys itself once health reaches zero. maxHealth comes
/// from the assigned EnemyData asset rather than being hardcoded per
/// prefab, so different enemy types can share this same script.
/// </summary>
public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private EnemyData data;

    private int maxHealth;
    private int currentHealth;

    public event Action OnDamaged;

    private void Awake()
    {
        maxHealth = data != null ? data.maxHealth : 30;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage ({currentHealth}/{maxHealth} remaining).");

        OnDamaged?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (data != null && QuestManager.instance != null)
        {
            QuestManager.instance.ReportEnemyKilled(data.enemyID);
        }

        Destroy(gameObject);
    }
}