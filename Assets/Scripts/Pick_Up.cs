using UnityEngine;
using TMPro;

public class Item_Pickup_Detector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TextMeshProUGUI promptText; // the "Pick Up X" UI text
    [SerializeField] private Inventory_Manager inventoryManager;

    [Header("Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    private WorldItem currentTarget;

    private void Update()
    {
        DetectItem();

        if (currentTarget != null && Input.GetKeyDown(pickupKey))
        {
            TryPickUp(currentTarget);
        }
    }

    private void DetectItem()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // center of screen
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, interactRange);

        WorldItem hitItem = hitSomething ? hit.collider.GetComponent<WorldItem>() : null;

        if (hitItem != null)
        {
            ShowPrompt(hitItem);
        }
        else
        {
            HidePrompt();
        }
    }

    private void ShowPrompt(WorldItem worldItem)
    {
        currentTarget = worldItem;
        promptText.text = $"Pick Up {worldItem.item.displayName}";
        promptText.gameObject.SetActive(true);
    }

    private void HidePrompt()
    {
        currentTarget = null;
        promptText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Attempts to add the world item to the inventory. Only removes
    /// it from the world if the inventory actually had room for it.
    /// </summary>
    private void TryPickUp(WorldItem worldItem)
    {
        // Clone before handing it to the inventory - without this, every
        // world instance that shares the same Item asset would end up
        // pointing at the exact same object, so changing one slot's
        // count (or clearing it) would silently affect every other slot
        // holding "the same" item.
        Item pickedItem = worldItem.item.Clone();

        bool wasAdded = inventoryManager.AddItem(pickedItem);

        if (wasAdded)
        {
            HidePrompt();
            Destroy(worldItem.gameObject);
        }
        else
        {
            // Inventory is full - leave the item in the world.
            // (Could show a "Inventory Full" message here later.)
            Debug.Log("Inventory full - could not pick up item.");
        }
    }
}