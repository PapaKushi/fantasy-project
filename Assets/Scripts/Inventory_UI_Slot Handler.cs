using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles a single inventory slot's data and display, responds to
/// left/right clicks by delegating to the Inventory_Mouse_Manager,
/// and shows/hides a tooltip on hover via Inventory_Tooltip.
///
/// If attachPoint is assigned, this slot also acts as an equip slot:
/// whenever its item changes, the item's worldPrefab is spawned on (or
/// removed from) that attach point, so e.g. a weapon slot can pop the
/// sword model into the player's hand.
/// </summary>
public class Inventory_UI_SlotHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Slot Data")]
    [SerializeField] private Item _item; // backing field shown in the Inspector

    /// <summary>
    /// The item currently in this slot (null if empty). Setting this
    /// refreshes the slot's display, and if this is an equip slot
    /// (attachPoint assigned), also spawns/despawns the equipped
    /// item's world prefab.
    /// </summary>
    public Item item
    {
        get => _item;
        set
        {
            _item = value;
            RefreshSlotDisplay();
            RefreshEquipVisual();
        }
    }

    [Header("Slot Restriction")]
    [Tooltip("All = a normal inventory slot that accepts any item type. " +
             "Any other value = an equipment slot that only accepts that exact item type.")]
    public ItemType acceptedType = ItemType.All;

    [Header("Equip Visual (optional)")]
    [Tooltip("Only set this for equipment slots. The transform the item's " +
             "worldPrefab gets spawned under when equipped (e.g. a hand or head socket).")]
    [SerializeField] private Transform attachPoint;

    // Currently spawned equip visual, if any, so it can be cleaned up
    // before spawning a new one or when the slot is emptied.
    private GameObject spawnedVisual;

    [Header("UI References")]
    public Image icon;                         // Image component that displays the item's sprite
    public TextMeshProUGUI itemCountText;      // Text component that displays the item's stack count

    [Header("References")]
    public Inventory_Manager inventoryManager; // Manager this slot reports to for stacking/placing/clearing

    private void Awake()
    {
        // Route the initial Inspector-assigned value through the property
        // so display and equip visuals are set up correctly on start too.
        item = item != null ? item.Clone() : null;
    }

    /// <summary>
    /// Whether this slot is allowed to hold the given item. Slots with
    /// acceptedType == All accept anything; any other acceptedType only
    /// accepts an exact matching item type.
    /// </summary>
    public bool CanAccept(Item candidateItem)
    {
        if (candidateItem == null)
        {
            return true; // an "empty hand" is always a valid thing to hold
        }

        return acceptedType == ItemType.All || candidateItem.itemType == acceptedType;
    }

    /// <summary>
    /// Updates the icon and count text to match the current item,
    /// or hides them if the slot is empty.
    /// </summary>
    private void RefreshSlotDisplay()
    {
        bool hasItem = item != null;

        // Only show the icon when the slot actually has an item
        icon.gameObject.SetActive(hasItem);
        icon.sprite = hasItem ? item.itemIcon : null;
        itemCountText.text = hasItem ? item.itemCount.ToString() : string.Empty;
    }

    /// <summary>
    /// Spawns the current item's worldPrefab on attachPoint, replacing
    /// whatever was spawned before. Does nothing if this slot isn't
    /// set up as an equip slot (no attachPoint assigned).
    /// </summary>
    private void RefreshEquipVisual()
    {
        if (attachPoint == null)
        {
            return; // not an equip slot - nothing to do
        }

        if (spawnedVisual != null)
        {
            Destroy(spawnedVisual);
            spawnedVisual = null;
        }

        if (item != null && item.worldPrefab != null)
        {
            spawnedVisual = Instantiate(item.worldPrefab, attachPoint);
            spawnedVisual.transform.localPosition = Vector3.zero;
            spawnedVisual.transform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Routes clicks to the mouse manager: right-click picks up a
    /// single item from the stack, left-click picks up/places/swaps
    /// the whole stack currently held by the mouse.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        bool isRightClick = eventData.button == PointerEventData.InputButton.Right;

        if (isRightClick)
        {
            // Nothing to pick up from an empty slot
            if (item == null)
            {
                return;
            }

            Inventory_Mouse_Manager.instance.PickupFromStack(this);
            return;
        }

        // Left click: pick up, place, or swap the held item with this slot
        Inventory_Mouse_Manager.instance.UpdateHeldItem(this);
    }

    /// <summary>
    /// Shows the tooltip when the cursor enters a slot that has an item.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            Inventory_Tooltip.instance.Show(item);
        }
    }

    /// <summary>
    /// Hides the tooltip when the cursor leaves the slot.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        Inventory_Tooltip.instance.Hide();
    }
}