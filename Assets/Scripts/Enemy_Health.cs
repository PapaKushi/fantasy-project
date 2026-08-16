using System;
using UnityEngine;

/// <summary>
/// Basic health for a test enemy. Call TakeDamage() when it's hit;
/// the object destroys itself once health reaches zero.
///
/// Fires OnDamaged whenever it takes damage, so an AI script (e.g.
/// Boar_AI) can react - for example, switching from idle to chasing
/// the attacker the first time it's hit.
/// </summary>
public class Enemy_Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;

    public event Action OnDamaged;

    private void Awake()
    {
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
        Destroy(gameObject);
    }
}