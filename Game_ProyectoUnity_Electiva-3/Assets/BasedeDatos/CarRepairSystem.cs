using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using Firebase.Firestore;

public class CarRepairSystem : MonoBehaviour
{
    [Header("Configuración")]
    public int llantasNecesarias = 4;
    private int llantasColocadas = 0;
    private bool carroReparado = false;

    [Header("Referencias de Modelos")]
    public GameObject carroDanado;
    public GameObject carroReparadoPrefab;
    public Transform spawnPoint;

    [Header("UI")]
    public TextMeshProUGUI textoProgreso;

    [Header("Feedback")]
    public AudioSource sonidoReparacion;
    public AudioSource sonidoCompleto;

    [Header("Cambio de Escena")]
    public string nombreEscenaVictoria = "Victoria";
    public float tiempoEspera = 3f; // tiempo de espera antes de cargar escena

    // 🔥 Firebase
    private FirebaseFirestore db;

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        ActualizarProgresoUI();

        if (carroReparadoPrefab != null)
            carroReparadoPrefab.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !carroReparado)
        {
            Debug.Log("Jugador entró a la zona de reparación del carro.");
            IntentarReparar();
        }
    }

    public void IntentarReparar()
    {
        if (!InventoryManager.Instance.TieneItem("Llanta"))
        {
            Debug.LogWarning("No tienes llantas en el inventario.");
            return;
        }

        bool consumida = InventoryManager.Instance.UsarItemPorNombre("Llanta");

        if (consumida)
        {
            llantasColocadas++;
            Debug.Log($"Llantas colocadas: {llantasColocadas}/{llantasNecesarias}");

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
        Debug.Log("Carro completamente reparado.");

        // Ocultar carro dañado y mostrar reparado
        if (carroDanado != null)
            carroDanado.SetActive(false);

        if (carroReparadoPrefab != null)
            carroReparadoPrefab.SetActive(true);

        if (sonidoCompleto != null)
            sonidoCompleto.Play();

        ActualizarProgresoUI();

        // OBTENER REFERENCIA AL TIMER ANTES DE LA COROUTINE
        GameTimer timer = Object.FindFirstObjectByType<GameTimer>();
        StartCoroutine(GuardarYCambiarEscena(timer));
    }

    IEnumerator GuardarYCambiarEscena(GameTimer timer)
    {
        if (timer != null)
        {
            Debug.Log("Deteniendo timer y guardando victoria en Firebase...");
            timer.DetenerYGuardarVictoria();
        }
        else
        {
            Debug.LogError("GameTimer no encontrado!");
        }

        Debug.Log($"Esperando {tiempoEspera} segundos para completar guardado...");
        yield return new WaitForSeconds(tiempoEspera);

        Debug.Log($"Cargando escena: {nombreEscenaVictoria}");
        SceneManager.LoadScene(nombreEscenaVictoria);
    }

    void ActualizarProgresoUI()
    {
        if (textoProgreso != null)
        {
            if (!carroReparado)
                textoProgreso.text = $"Llantas instaladas: {llantasColocadas}/{llantasNecesarias}";
            else
                textoProgreso.text = "Carro Reparado! Escapando...";
        }
    }
}
