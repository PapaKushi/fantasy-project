using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles moving item data into, out of, and between inventory slots.
/// Slot UI (icon/text) is kept in sync here so callers only ever
/// need to hand this manager an Inventory_UI_SlotHandler and an Item.
/// </summary>
public class Inventory_Manager : MonoBehaviour
{
    [Header("Slots")]
    [Tooltip("All inventory slots this manager can place picked-up items into.")]
    [SerializeField] private List<Inventory_UI_SlotHandler> slots;

    /// <summary>
    /// Adds as much of the held item's count onto an existing stack
    /// as fits under maxStack. Returns how many units were actually
    /// absorbed, so the caller can keep any leftover in hand.
    /// </summary>
    public int StackInInventory(Inventory_UI_SlotHandler currentSlot, Item item)
    {
        if (currentSlot.item == null)
        {
            Debug.LogWarning("StackInInventory called on an empty slot.");
            return 0;
        }

        int spaceAvailable = currentSlot.item.maxStack - currentSlot.item.itemCount;
        int amountToMove = Mathf.Min(spaceAvailable, item.itemCount);

        currentSlot.item.itemCount += amountToMove;
        currentSlot.itemCountText.text = currentSlot.item.itemCount.ToString();

        return amountToMove;
    }

    /// <summary>
    /// Places an item into a slot, replacing whatever (if anything)
    /// was there before, and updates the slot's display.
    /// </summary>
    public void PlaceInInventory(Inventory_UI_SlotHandler currentSlot, Item item)
    {
        currentSlot.item = item;
        currentSlot.icon.sprite = item.itemIcon;
        currentSlot.itemCountText.text = item.itemCount.ToString();
        currentSlot.icon.gameObject.SetActive(true);
    }

    /// <summary>
    /// Empties a slot and resets its display back to "no item" state.
    /// </summary>
    public void ClearItemSlot(Inventory_UI_SlotHandler currentSlot)
    {
        currentSlot.item = null;
        currentSlot.icon.sprite = null;
        currentSlot.itemCountText.text = string.Empty;
        currentSlot.icon.gameObject.SetActive(false);
    }

    /// <summary>
    /// Adds a world-picked-up item into the inventory: first tries to
    /// stack onto an existing matching, non-full slot, then falls back
    /// to the first empty slot. Returns true if the item was placed
    /// somewhere (so the caller knows whether to remove it from the world),
    /// false if the inventory is completely full.
    /// </summary>
    public bool AddItem(Item newItem)
    {
        // Captured before the stacking loop below mutates newItem.itemCount,
        // so quest reporting reflects the full original pickup amount.
        int originalAmount = newItem.itemCount;

        // First pass: try to stack onto a matching slot that has room
        foreach (Inventory_UI_SlotHandler slot in slots)
        {
            if (slot.item != null && slot.item.itemID == newItem.itemID && slot.item.itemCount < slot.item.maxStack)
            {
                int amountMoved = StackInInventory(slot, newItem);
                newItem.itemCount -= amountMoved;

                if (newItem.itemCount <= 0)
                {
                    ReportPickupToQuests(newItem.itemID, originalAmount);
                    return true; // fully absorbed into this stack
                }
                // otherwise keep looping in case another matching slot has room
            }
        }

        // Second pass: drop any remainder into the first empty slot
        foreach (Inventory_UI_SlotHandler slot in slots)
        {
            if (slot.item == null)
            {
                PlaceInInventory(slot, newItem);
                ReportPickupToQuests(newItem.itemID, originalAmount);
                return true;
            }
        }

        // No matching slot with room and no empty slot left
        return false;
    }

    private void ReportPickupToQuests(string itemID, int amount)
    {
        if (QuestManager.instance != null)
        {
            QuestManager.instance.ReportItemCollected(itemID, amount);
        }
    }
}