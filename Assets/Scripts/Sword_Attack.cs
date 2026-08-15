using UnityEngine;

/// <summary>
/// Handles left-click attacking for a weapon. Add this directly to the
/// weapon prefab (alongside its Animator) - it only runs while the
/// weapon is actually equipped and spawned in the scene.
/// </summary>
public class Weapon_Attack : MonoBehaviour
{
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

    private void Awake()
    {
        if (weaponAnimator == null)
        {
            weaponAnimator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        // Don't swing while managing the inventory
        if (Inventory_Toggle.IsInventoryOpen)
        {
            return;
        }

        if (Input.GetKeyDown(attackKey))
        {
            weaponAnimator.SetTrigger("Attack");
        }
    }
}