using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI quantityText;
    public Button useButton;

    private int slotIndex;
    private bool hasItem = false;

    private void Start()
    {
        // Asegurar estado inicial
        if (icon != null)
        {
            icon.enabled = false;
        }

        if (useButton != null)
        {
            useButton.interactable = false;
        }

        if (quantityText != null)
        {
            quantityText.text = "";
        }
    }

    public void SetItem(Sprite sprite, int quantity)
    {
        if (sprite == null)
        {
            Debug.LogWarning($"⚠️ SetItem llamado con sprite null en slot {slotIndex}");
            ClearSlot();
            return;
        }

        if (icon != null)
        {
            icon.sprite = sprite;
            icon.enabled = true;
            icon.color = Color.white; // Asegurar que sea visible
        }
        else
        {
            Debug.LogError($"❌ Icon es null en slot {slotIndex}");
        }

        if (quantityText != null)
        {
            quantityText.text = quantity.ToString();
        }

        if (useButton != null)
        {
            useButton.interactable = true;
        }

        hasItem = true;
        Debug.Log($"✅ Slot {slotIndex} configurado con sprite '{sprite.name}' (cantidad: {quantity})");
    }

    public void ClearSlot()
    {
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        if (quantityText != null)
        {
            quantityText.text = "";
        }

        if (useButton != null)
        {
            useButton.interactable = false;
        }

        hasItem = false;
    }

    // Configurar el índice del slot
    public void SetIndex(int index)
    {
        slotIndex = index;

        if (useButton != null)
        {
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(() => {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.OnUseSlot(slotIndex);
                }
            });
        }

        Debug.Log($"📍 Slot {index} configurado");
    }

    public bool HasItem()
    {
        return hasItem;
    }

    public int GetIndex()
    {
        return slotIndex;
    }
}