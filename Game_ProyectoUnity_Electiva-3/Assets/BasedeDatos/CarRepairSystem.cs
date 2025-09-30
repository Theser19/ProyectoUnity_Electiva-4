using UnityEngine;
using TMPro; // Para usar TextMeshPro

public class CarRepairSystem : MonoBehaviour
{
    [Header("Configuración")]
    public int llantasNecesarias = 4;
    private int llantasColocadas = 0;
    private bool carroReparado = false; // estado del carro

    [Header("Referencias de Modelos")]
    public GameObject carroDanado;          // modelo inicial
    public GameObject carroReparadoPrefab;  // modelo final (renombrado para evitar ambigüedad)
    public Transform spawnPoint;            // opcional si prefieres instanciar

    [Header("UI")]
    public TextMeshProUGUI textoProgreso;   // texto del progreso

    [Header("Feedback")]
    public AudioSource sonidoReparacion;
    public AudioSource sonidoCompleto;

    private void Start()
    {
        ActualizarProgresoUI();

        // aseguramos que el carro reparado inicie oculto
        if (carroReparadoPrefab != null)
            carroReparadoPrefab.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !carroReparado)
        {
            Debug.Log("🚗 Jugador entró a la zona de reparación del carro.");
            IntentarReparar();
        }
    }

    public void IntentarReparar()
    {
        if (!InventoryManager.Instance.TieneItem("Llanta"))
        {
            Debug.LogWarning("⚠️ No tienes llantas en el inventario.");
            return;
        }

        bool consumida = InventoryManager.Instance.UsarItemPorNombre("Llanta");

        if (consumida)
        {
            llantasColocadas++;
            Debug.Log($"🔧 Llantas colocadas: {llantasColocadas}/{llantasNecesarias}");

            if (sonidoReparacion != null)
                sonidoReparacion.Play();

            ActualizarProgresoUI();

            if (llantasColocadas >= llantasNecesarias)
            {
                RepararCarro();
            }
        }
    }

    void RepararCarro()
    {
        carroReparado = true;
        Debug.Log("✅ Carro completamente reparado.");

        // ocultar carro dañado y mostrar reparado
        if (carroDanado != null)
            carroDanado.SetActive(false);

        if (carroReparadoPrefab != null)
            carroReparadoPrefab.SetActive(true);

        // si prefieres instanciar en vez de activar:
        /*
        if (carroReparadoPrefab != null && spawnPoint != null)
        {
            Instantiate(carroReparadoPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        */

        if (sonidoCompleto != null)
            sonidoCompleto.Play();

        ActualizarProgresoUI();
    }

    void ActualizarProgresoUI()
    {
        if (textoProgreso != null)
        {
            if (!carroReparado)
                textoProgreso.text = $"Llantas instaladas: {llantasColocadas}/{llantasNecesarias}";
            else
                textoProgreso.text = "✅ Carro Reparado";
        }
    }
}
