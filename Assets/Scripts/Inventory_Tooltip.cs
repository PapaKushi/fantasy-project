using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Shows a small tooltip near the cursor with an item's display name
/// and description. Call Show(item) / Hide() from wherever the pointer
/// enters/exits an item (e.g. Inventory_UI_SlotHandler).
/// </summary>
public class Inventory_Tooltip : MonoBehaviour
{
    public static Inventory_Tooltip instance;

    [Header("References")]
    [SerializeField] private GameObject tooltipRoot;       // the tooltip panel itself
    [SerializeField] private RectTransform tooltipTransform;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Positioning")]
    [SerializeField] private Vector2 cursorOffset = new Vector2(15f, -15f);

    private bool isVisible;

    private void Awake()
    {
        instance = this;
        tooltipRoot.SetActive(false);
    }

    private void Update()
    {
        if (isVisible)
        {
            tooltipTransform.position = (Vector2)Input.mousePosition + cursorOffset;
        }
    }

    /// <summary>
    /// Displays the tooltip with the given item's name/description.
    /// </summary>
    public void Show(Item item)
    {
        if (item == null)
        {
            return;
        }

        nameText.text = item.displayName;
        descriptionText.text = item.description;
        tooltipRoot.SetActive(true);
        isVisible = true;
    }

    /// <summary>
    /// Hides the tooltip.
    /// </summary>
    public void Hide()
    {
        tooltipRoot.SetActive(false);
        isVisible = false;
    }
}
