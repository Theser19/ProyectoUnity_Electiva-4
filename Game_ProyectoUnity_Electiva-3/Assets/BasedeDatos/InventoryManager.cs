using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    FirebaseFirestore db;
    public int maxSlots = 3;
    public List<InventoryItem> items = new List<InventoryItem>();

    // Cola para guardar items que se recojan antes de autenticar
    private Queue<PendingItem> pendingItems = new Queue<PendingItem>();
    private bool isProcessingQueue = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Solo DontDestroyOnLoad si es objeto raíz
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        // Suscribirse al evento de autenticación
        AuthManager.OnAuthCompleted += OnAuthenticationReady;

        // Si ya está autenticado al iniciar, procesar cola inmediatamente
        if (AuthManager.IsReady())
        {
            OnAuthenticationReady();
        }
    }

    void OnDestroy()
    {
        AuthManager.OnAuthCompleted -= OnAuthenticationReady;
    }

    void OnAuthenticationReady()
    {
        Debug.Log("✅ InventoryManager: Autenticación lista, procesando items pendientes...");
        ProcessPendingItems();
    }

    [System.Serializable]
    public class InventoryItem
    {
        public string id;
        public string nombre;
        public string imagen;
        public int cantidad;
        public Sprite sprite;
    }

    // Clase auxiliar para items en cola
    private class PendingItem
    {
        public string itemId;
        public string nombre;
        public string imagen;
    }

    public bool AgregarItem(string itemId, string nombre, string imagen)
    {
        // Si no está autenticado, agregar a la cola
        if (!AuthManager.IsReady())
        {
            Debug.LogWarning($"⏳ Usuario no autenticado todavía. Encolando '{nombre}'...");
            pendingItems.Enqueue(new PendingItem
            {
                itemId = itemId,
                nombre = nombre,
                imagen = imagen
            });
            return false;
        }

        // Verificar inventario lleno
        if (items.Count >= maxSlots)
        {
            Debug.LogWarning($"⚠️ Inventario lleno ({items.Count}/{maxSlots}), no se puede agregar: {nombre}");
            return false;
        }

        // Cargar sprite desde Resources (sin la extensión del archivo)
        string spritePath = "Items/" + System.IO.Path.GetFileNameWithoutExtension(imagen);
        Sprite sprite = Resources.Load<Sprite>(spritePath);

        if (sprite == null)
        {
            Debug.LogError($"❌ No se encontró el sprite en Resources/{spritePath}");
            Debug.LogError($"   Buscado: Resources/{spritePath}");
            Debug.LogError($"   Asegúrate de que el archivo exista y sea un Sprite (no Texture2D)");
        }
        else
        {
            Debug.Log($"✅ Sprite cargado correctamente: {spritePath}");
        }

        // Crear nuevo item
        InventoryItem nuevoItem = new InventoryItem
        {
            id = itemId,
            nombre = nombre,
            imagen = imagen,
            cantidad = 1,
            sprite = sprite
        };

        items.Add(nuevoItem);

        // Guardar en Firestore
        SaveItemToFirestore(itemId, nombre, imagen);

        Debug.Log($"✅ Item agregado al inventario: {nombre} (Total: {items.Count}/{maxSlots})");

        // Actualizar UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateInventoryUI();
        }

        return true;
    }

    void SaveItemToFirestore(string itemId, string nombre, string imagen)
    {
        if (!AuthManager.IsReady())
        {
            Debug.LogError("❌ No se puede guardar en Firestore: usuario no autenticado");
            return;
        }

        string userId = AuthManager.user.UserId;

        DocumentReference itemRef = db
            .Collection("Usuarios")
            .Document(userId)
            .Collection("Inventario")
            .Document(itemId);

        itemRef.SetAsync(new
        {
            id = itemId,
            nombre = nombre,
            imagen = imagen,
            cantidad = 1,
            timestamp = FieldValue.ServerTimestamp
        }).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"❌ Error al guardar item en Firestore: {task.Exception}");
            }
            else
            {
                Debug.Log($"☁️ Item '{nombre}' guardado en Firestore");
            }
        });
    }

    void ProcessPendingItems()
    {
        if (isProcessingQueue) return;
        isProcessingQueue = true;

        Debug.Log($"📦 Procesando {pendingItems.Count} items pendientes...");

        while (pendingItems.Count > 0)
        {
            PendingItem pending = pendingItems.Dequeue();
            AgregarItem(pending.itemId, pending.nombre, pending.imagen);
        }

        isProcessingQueue = false;
        Debug.Log("✅ Todos los items pendientes fueron procesados");
    }

    public void UsarItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count)
        {
            Debug.LogWarning($"⚠️ Índice de slot inválido: {slotIndex}");
            return;
        }

        InventoryItem item = items[slotIndex];
        item.cantidad--;

        Debug.Log($"🔧 Usando item: {item.nombre} (cantidad restante: {item.cantidad})");

        if (item.cantidad <= 0)
        {
            Debug.Log($"🗑️ Item eliminado del inventario: {item.nombre}");
            RemoveItemFromFirestore(item.id);
            items.RemoveAt(slotIndex);
        }
        else
        {
            // Actualizar cantidad en Firestore
            UpdateItemQuantityInFirestore(item.id, item.cantidad);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateInventoryUI();
        }
    }

    void RemoveItemFromFirestore(string itemId)
    {
        if (!AuthManager.IsReady()) return;

        string userId = AuthManager.user.UserId;

        db.Collection("Usuarios")
            .Document(userId)
            .Collection("Inventario")
            .Document(itemId)
            .DeleteAsync();
    }

    void UpdateItemQuantityInFirestore(string itemId, int newQuantity)
    {
        if (!AuthManager.IsReady()) return;

        string userId = AuthManager.user.UserId;

        db.Collection("Usuarios")
            .Document(userId)
            .Collection("Inventario")
            .Document(itemId)
            .UpdateAsync("cantidad", newQuantity);
    }

    public List<InventoryItem> GetItems()
    {
        return items;
    }

    // Método para debug
    public void PrintInventoryStatus()
    {
        Debug.Log($"📊 Estado del Inventario:");
        Debug.Log($"   Items: {items.Count}/{maxSlots}");
        Debug.Log($"   Pendientes: {pendingItems.Count}");
        Debug.Log($"   Autenticado: {AuthManager.IsReady()}");
    }
}