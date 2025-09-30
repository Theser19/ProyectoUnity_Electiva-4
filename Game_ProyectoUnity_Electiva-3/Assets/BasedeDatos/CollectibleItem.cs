using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleItem : MonoBehaviour
{
    [Header("Datos del Item")]
    public string itemId;     // Ej: "1"
    public string nombre;     // Ej: "Llanta"
    public string imagen;     // Ej: "Llanta" (sin extensión es mejor)

    private bool collected = false; // Evitar recolección múltiple

    private void Start()
    {
        // Asegurar que el collider es un trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // Validar que los campos estén completos
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogError($"❌ {gameObject.name}: itemId está vacío");
        }
        if (string.IsNullOrEmpty(nombre))
        {
            Debug.LogError($"❌ {gameObject.name}: nombre está vacío");
        }
        if (string.IsNullOrEmpty(imagen))
        {
            Debug.LogError($"❌ {gameObject.name}: imagen está vacío");
        }

        Debug.Log($"📦 CollectibleItem '{nombre}' listo para recoger (ID: {itemId}, Imagen: {imagen})");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Evitar recolección múltiple
        if (collected)
        {
            Debug.LogWarning($"⚠️ {nombre} ya fue recogido, ignorando...");
            return;
        }

        // Detectar si el jugador recogió el objeto
        if (other.CompareTag("Player"))
        {
            Debug.Log($"🎯 Jugador tocó: {nombre}");

            if (InventoryManager.Instance != null)
            {
                collected = true; // Marcar como recogido ANTES de agregarlo

                bool added = InventoryManager.Instance.AgregarItem(itemId, nombre, imagen);

                if (added)
                {
                    Debug.Log($"✅ {nombre} recogido y agregado al inventario");
                    // Destruir el objeto del mundo
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning($"⚠️ No se pudo agregar {nombre} (inventario lleno?)");
                    collected = false; // Permitir intentarlo de nuevo
                }
            }
            else
            {
                Debug.LogError("❌ No se encontró InventoryManager en la escena");
            }
        }
    }

    // Para debugging: mostrar el área de recolección
    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawWireCube(transform.position + col.bounds.center - transform.position, col.bounds.size);
        }
    }
}