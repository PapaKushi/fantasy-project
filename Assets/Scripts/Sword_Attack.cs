using UnityEngine;

/// <summary>
/// Handles left-click attacking for a weapon. Add this directly to the
/// weapon prefab (alongside its Animator) - it only runs while the
/// weapon is actually equipped and spawned in the scene.
///
/// weaponItem is assigned automatically by Inventory_UI_SlotHandler
/// right after this prefab is instantiated, so damage always matches
/// whatever Item this weapon represents.
/// </summary>
public class Weapon_Attack : MonoBehaviour
{
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

    [Header("Hit Detection")]
    [Tooltip("How far in front of the camera an attack can reach.")]
    [SerializeField] private float attackRange = 2.5f;

    // Set by Inventory_UI_SlotHandler when this weapon is equipped.
    [HideInInspector] public Item weaponItem;

    private Camera playerCamera;

    private void Awake()
    {
        if (weaponAnimator == null)
        {
            weaponAnimator = GetComponent<Animator>();
        }

        playerCamera = Camera.main;
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
            TryHitTarget();
        }
    }

    /// <summary>
    /// Raycasts from the center of the screen and damages whatever
    /// Enemy_Health it hits within range. Called immediately on click
    /// rather than synced to a specific animation frame - fine for a
    /// first pass, can be refined later with an Animation Event if the
    /// timing needs to line up more precisely with the swing.
    /// </summary>
    private void TryHitTarget()
    {
        if (weaponItem == null)
        {
            Debug.LogWarning("Weapon_Attack has no weaponItem assigned - can't deal damage.");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange))
        {
            Enemy_Health target = hit.collider.GetComponent<Enemy_Health>();
            if (target != null)
            {
                target.TakeDamage(weaponItem.damage);
            }
        }
    }
}