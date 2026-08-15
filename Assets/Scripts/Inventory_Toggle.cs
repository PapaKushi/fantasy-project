using UnityEngine;

/// <summary>
/// Shows/hides the inventory UI when the toggle key is pressed, and
/// exposes IsInventoryOpen so other scripts (e.g. FirstPersonController)
/// can check whether the inventory is currently open and react accordingly.
/// </summary>
public class Inventory_Toggle : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUI; // drag your Inventory panel/Canvas here
    [SerializeField] private KeyCode toggleKey = KeyCode.I; // or Tab, B, whatever you prefer

    // True whenever the inventory panel is open. Other scripts read this
    // (e.g. to stop the camera from rotating) rather than needing a
    // direct reference to this object.
    public static bool IsInventoryOpen { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            bool isOpening = !inventoryUI.activeSelf;
            inventoryUI.SetActive(isOpening);
            IsInventoryOpen = isOpening;

            // Free the cursor while managing items, and re-lock it
            // for normal first-person look when the inventory closes.
            Cursor.lockState = isOpening ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isOpening;
        }
    }
}