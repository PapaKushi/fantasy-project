using System;
using UnityEngine;

/// <summary>
/// Basic health for the player. Enemies call TakeDamage() on this
/// when they attack. Fires OnDamaged/OnDied for UI (health bar, etc)
/// to hook into later.
/// </summary>
public class Player_Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public event Action<int, int> OnDamaged; // (currentHealth, maxHealth)
    public event Action OnDied;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        Debug.Log($"Player took {amount} damage ({currentHealth}/{maxHealth} remaining).");

        OnDamaged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            OnDied?.Invoke();
        }
    }
}