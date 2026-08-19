using UnityEngine;

/// <summary>
/// Shows/hides the quest log UI when the toggle key is pressed.
/// Mirrors Inventory_Toggle's pattern but controls its own panel and
/// key, so the quest log can open independently of the inventory.
/// </summary>
public class Quest_Log_Toggle : MonoBehaviour
{
    [SerializeField] private GameObject questLogUI; // drag your Quest_Log_Panel here
    [SerializeField] private KeyCode toggleKey = KeyCode.L;

    public static bool IsQuestLogOpen { get; private set; }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            bool isOpening = !questLogUI.activeSelf;
            questLogUI.SetActive(isOpening);
            IsQuestLogOpen = isOpening;

            // Free the cursor while reading the log, same as the inventory.
            // Only re-lock on close if the inventory isn't also open -
            // otherwise closing one panel would yank the cursor away
            // while the other is still open.
            if (isOpening)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (!Inventory_Toggle.IsInventoryOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
