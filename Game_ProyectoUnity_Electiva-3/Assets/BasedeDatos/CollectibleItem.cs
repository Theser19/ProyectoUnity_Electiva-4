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
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        if (string.IsNullOrEmpty(itemId))
            Debug.LogError($"❌ {gameObject.name}: itemId está vacío");
        if (string.IsNullOrEmpty(nombre))
            Debug.LogError($"❌ {gameObject.name}: nombre está vacío");
        if (string.IsNullOrEmpty(imagen))
            Debug.LogError($"❌ {gameObject.name}: imagen está vacío");

        Debug.Log($"📦 CollectibleItem '{nombre}' listo para recoger (ID: {itemId}, Imagen: {imagen})");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        if (other.CompareTag("Player"))
        {
            Debug.Log($"🎯 Jugador tocó: {nombre} (ID: {itemId})");

            if (InventoryManager.Instance == null)
            {
                Debug.LogError("❌ No se encontró InventoryManager en la escena");
                return;
            }

            // 🔹 Intentar agregar al inventario
            bool added = InventoryManager.Instance.AgregarItem(itemId, nombre, imagen);

            if (added)
            {
                collected = true; // ✅ Solo marcar si realmente se agregó
                Debug.Log($"✅ {nombre} recogido correctamente");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning($"⚠️ Inventario lleno, no se pudo recoger {nombre}. Esperando espacio...");
                // ❌ NO marcar como 'collected' para permitir reintentar después
            }
        }
    }

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
