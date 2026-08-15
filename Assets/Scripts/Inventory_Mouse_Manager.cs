using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Tracks whatever item is currently "held" by the mouse cursor and
/// handles moving it between inventory slots. Works together with
/// Inventory_UI_SlotHandler (which routes clicks here) and
/// Inventory_Manager (which actually mutates slot data/display).
/// </summary>
public class Inventory_Mouse_Manager : MonoBehaviour
{
    public static Inventory_Mouse_Manager instance;

    [Header("Held Item Display")]
    [SerializeField] private RectTransform heldItemTransform; // the empty object's RectTransform
    [SerializeField] private Image heldItemIcon;
    [SerializeField] private TextMeshProUGUI heldItemCountText;

    // Backing field for the currentlyHeldItem property below.
    private Item _currentlyHeldItem;

    /// <summary>
    /// The item stack currently "attached" to the cursor, or null if
    /// the player isn't holding anything right now. Setting this
    /// automatically refreshes the on-screen held-item icon, so
    /// every assignment anywhere in this file stays in sync without
    /// needing to remember to call RefreshHeldItemDisplay() manually.
    /// </summary>
    public Item currentlyHeldItem
    {
        get => _currentlyHeldItem;
        set
        {
            _currentlyHeldItem = value;
            RefreshHeldItemDisplay();
        }
    }

    private void Awake()
    {
        instance = this;
        RefreshHeldItemDisplay(); // start hidden if nothing is held
    }

    private void Update()
    {
        // Follow the mouse whenever something's being held
        if (currentlyHeldItem != null)
        {
            heldItemTransform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// Shows/hides the cursor-following icon and keeps its sprite
    /// and count text matched to currentlyHeldItem.
    /// </summary>
    private void RefreshHeldItemDisplay()
    {
        bool hasItem = currentlyHeldItem != null;

        heldItemIcon.gameObject.SetActive(hasItem);
        heldItemIcon.sprite = hasItem ? currentlyHeldItem.itemIcon : null;
        heldItemCountText.text = hasItem ? currentlyHeldItem.itemCount.ToString() : string.Empty;
    }

    /// <summary>
    /// Called on a left-click. Handles three cases:
    /// 1. Held item matches the slot's item -> merge stacks (up to maxStack).
    /// 2. Slot has a different item -> swap it with the held item.
    /// 3. Slot is empty -> drop the held item into it.
    /// A restricted slot (e.g. a weapon-only equipment slot) rejects
    /// anything CanAccept() doesn't allow and the click does nothing.
    /// </summary>
    public void UpdateHeldItem(Inventory_UI_SlotHandler currentSlot)
    {
        // Restricted slots (equipment) reject item types they don't accept.
        // Nothing happens - the held item stays in hand.
        if (!currentSlot.CanAccept(currentlyHeldItem))
        {
            return;
        }

        Item currentActiveItem = currentSlot.item;

        // Case 1: same item type in hand and in the slot -> stack them
        bool sameItemType = currentlyHeldItem != null
            && currentActiveItem != null
            && currentlyHeldItem.itemID == currentActiveItem.itemID;

        if (sameItemType)
        {
            int amountMoved = currentSlot.inventoryManager.StackInInventory(currentSlot, currentlyHeldItem);
            currentlyHeldItem.itemCount -= amountMoved;

            // Setting currentlyHeldItem (even to itself) below re-triggers
            // the display refresh so the held count reflects the leftover.
            currentlyHeldItem = currentlyHeldItem.itemCount <= 0 ? null : currentlyHeldItem;
            return;
        }

        // Case 2/3: different item (or nothing) in hand -> swap
        // Clear whatever's currently in the slot first...
        if (currentSlot.item != null)
        {
            currentSlot.inventoryManager.ClearItemSlot(currentSlot);
        }

        // ...then place whatever was in hand into the now-empty slot
        if (currentlyHeldItem != null)
        {
            currentSlot.inventoryManager.PlaceInInventory(currentSlot, currentlyHeldItem);
        }

        // Whatever was in the slot originally is now held by the mouse
        currentlyHeldItem = currentActiveItem;
    }

    /// <summary>
    /// Called on a right-click. Picks up a single item from the
    /// slot's stack (rather than the whole stack) and adds it to
    /// whatever the mouse is currently holding.
    /// </summary>
    public void PickupFromStack(Inventory_UI_SlotHandler currentSlot)
    {
        // Can't split a different item type onto what's already held
        if (currentlyHeldItem != null && currentlyHeldItem.itemID != currentSlot.item.itemID)
        {
            return;
        }

        // Start a new held stack (count 0) if the hand is currently empty
        if (currentlyHeldItem == null)
        {
            Item newHeldItem = currentSlot.item.Clone();
            newHeldItem.itemCount = 0;
            currentlyHeldItem = newHeldItem;
        }

        // Move one unit from the slot's stack to the held stack
        currentlyHeldItem.itemCount++;
        currentSlot.item.itemCount--;
        currentSlot.itemCountText.text = currentSlot.item.itemCount.ToString();

        // Re-trigger the display refresh now that the held count changed
        currentlyHeldItem = currentlyHeldItem;

        // If the slot's stack is now empty, clear it out entirely
        if (currentSlot.item.itemCount <= 0)
        {
            currentSlot.inventoryManager.ClearItemSlot(currentSlot);
        }
    }
}