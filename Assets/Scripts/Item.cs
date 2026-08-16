using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Broad category an item belongs to. Misc covers anything that isn't
/// equippable (potions, materials, etc). "All" is not meant to be used
/// on an actual item - it exists so a slot's acceptedType can mean
/// "accept anything" (see Inventory_UI_SlotHandler.CanAccept).
/// </summary>
public enum ItemType
{
    All,
    Misc,
    Weapon,
    Helmet,
    Chest,
    Legs,
    Boots
}

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]

public class Item : ScriptableObject
{
    public string itemID;
    public int itemCount;
    public int maxStack;
    public Sprite itemIcon;
    public string displayName;
    public string description;
    public ItemType itemType = ItemType.Misc; // set this per item in the Inspector

    [Header("Equip Visual")]
    [Tooltip("Only used for equippable items (Weapon, Helmet, etc). " +
             "The 3D model spawned on the player when this item is equipped.")]
    public GameObject worldPrefab;

    [Header("Combat")]
    [Tooltip("Only used for weapon items. Damage dealt per hit.")]
    public int damage;
}

public static class ScriptableObjectExtension
{
    /// <summary>
    /// Creates and returns a clone of any given scriptable object.
    /// </summary>
    public static T Clone<T>(this T scriptableObject) where T : ScriptableObject
    {
        if (scriptableObject == null)
        {
            Debug.LogError($"ScriptableObject was null. Returning default {typeof(T)} object.");
            return (T)ScriptableObject.CreateInstance(typeof(T));
        }

        T instance = UnityEngine.Object.Instantiate(scriptableObject);
        instance.name = scriptableObject.name; // remove (Clone) from name
        return instance;
    }
}