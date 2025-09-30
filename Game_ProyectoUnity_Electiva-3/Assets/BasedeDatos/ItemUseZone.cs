using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem; // ← NUEVO

[RequireComponent(typeof(Collider))]
public class ItemUseZone : MonoBehaviour
{
    [Header("Configuración de la Zona")]
    [Tooltip("Nombre del item requerido (ej: 'Llave', 'Llanta')")]
    public string itemRequerido;

    [Tooltip("¿Qué pasa al usar el item?")]
    public UnityEngine.Events.UnityEvent onItemUsado;

    [Header("UI (Opcional)")]
    [Tooltip("Texto para mostrar cuando el jugador esté cerca")]
    public TextMeshProUGUI textoUI;

    [Tooltip("Botón para usar el item")]
    public Button botonUsar;

    private bool jugadorEnZona = false;
    private GameObject jugador;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        if (textoUI != null)
        {
            textoUI.gameObject.SetActive(false);
        }

        if (botonUsar != null)
        {
            botonUsar.gameObject.SetActive(false);
            botonUsar.onClick.AddListener(IntentarUsarItem);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = true;
            jugador = other.gameObject;

            Debug.Log($"Jugador entró en zona de uso: {itemRequerido}");

            // Intentar usar el item automáticamente
            IntentarUsarItem();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;
            jugador = null;

            Debug.Log($"Jugador salió de zona de uso: {itemRequerido}");
            OcultarUI();
        }
    }

    private void Update()
    {
        // Usar tecla E con el nuevo Input System
        if (jugadorEnZona && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            IntentarUsarItem();
        }
    }

    void MostrarUI()
    {
        if (textoUI != null)
        {
            textoUI.gameObject.SetActive(true);

            bool tieneItem = InventoryManager.Instance != null &&
                           InventoryManager.Instance.TieneItem(itemRequerido);

            if (tieneItem)
            {
                textoUI.text = $"Presiona E para usar {itemRequerido}";
                textoUI.color = Color.green;
            }
            else
            {
                textoUI.text = $"Necesitas: {itemRequerido}";
                textoUI.color = Color.red;
            }
        }

        if (botonUsar != null)
        {
            botonUsar.gameObject.SetActive(true);
        }
    }

    void OcultarUI()
    {
        if (textoUI != null)
        {
            textoUI.gameObject.SetActive(false);
        }

        if (botonUsar != null)
        {
            botonUsar.gameObject.SetActive(false);
        }
    }

    public void IntentarUsarItem()
    {
        if (!jugadorEnZona)
        {
            Debug.LogWarning("No estás en la zona de uso");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager no encontrado");
            return;
        }

        int slotIndex = InventoryManager.Instance.BuscarItemPorNombre(itemRequerido);

        if (slotIndex == -1)
        {
            Debug.LogWarning($"No tienes {itemRequerido} en el inventario");

            if (textoUI != null)
            {
                textoUI.text = $"No tienes {itemRequerido}!";
                textoUI.color = Color.red;
            }

            return;
        }

        InventoryManager.Instance.UsarItem(slotIndex);

        Debug.Log($"✅ Item {itemRequerido} usado correctamente");

        onItemUsado?.Invoke();

        OcultarUI();
    }

    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position + col.bounds.center - transform.position, col.bounds.size);
        }
    }
    // Método público para ser llamado desde un botón UI
    public void UsarItemDesdeBoton()
    {
        IntentarUsarItem();
    }
}