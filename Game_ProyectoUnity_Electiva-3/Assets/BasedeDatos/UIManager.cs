using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public InventorySlot[] slots;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Inicializar todos los slots vacíos
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].SetIndex(i);
                slots[i].ClearSlot();
            }
        }

        Debug.Log($"✅ UIManager inicializado con {slots.Length} slots");
    }

    public void UpdateInventoryUI()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("❌ InventoryManager.Instance es null");
            return;
        }

        var items = InventoryManager.Instance.GetItems();

        Debug.Log($"🔄 Actualizando UI del inventario: {items.Count} items");

        // Actualizar cada slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                Debug.LogWarning($"⚠️ Slot {i} es null en el array");
                continue;
            }

            if (i < items.Count)
            {
                var item = items[i];

                if (item.sprite == null)
                {
                    Debug.LogWarning($"⚠️ El item '{item.nombre}' no tiene sprite asignado");
                    slots[i].ClearSlot();
                }
                else
                {
                    Debug.Log($"   Slot {i}: {item.nombre} (cantidad: {item.cantidad})");
                    slots[i].SetItem(item.sprite, item.cantidad);
                }
            }
            else
            {
                // Limpiar slots vacíos
                slots[i].ClearSlot();
            }
        }
    }

    // Llamar cuando presionas un botón en el slot
    public void OnUseSlot(int index)
    {
        Debug.Log($"🔧 Usando slot {index}");

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.UsarItem(index);
        }
        else
        {
            Debug.LogError("❌ No se puede usar el item: InventoryManager es null");
        }
    }
}