using UnityEngine;
using SimpleInputNamespace;

public class FlashlightController : MonoBehaviour
{
    [Header("Configuración de la Linterna")]
    public Light luzLinterna; // La luz que se enciende/apaga
    public string nombreBoton = "Fire1"; // Botón para encender/apagar (Fire1 = clic izquierdo/botón UI)

    [Header("Configuración de la Luz")]
    public float intensidadEncendida = 2f;
    public float rangoEncendido = 10f;
    public Color colorLuz = Color.white;
    public float anguloSpot = 45f; // Para Spot Light

    [Header("Audio (Opcional)")]
    public AudioSource audioSource;
    public AudioClip sonidoEncender;
    public AudioClip sonidoApagar;

    [Header("Estados")]
    public bool estaEncendida = false;

    // Referencias internas
    private bool botonPresionadoAnterior = false;

    void Start()
    {
        // Si no hay luz asignada, buscar en el mismo objeto
        if (luzLinterna == null)
            luzLinterna = GetComponentInChildren<Light>();

        // Si no hay AudioSource asignado, buscar en el mismo objeto
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Configurar la luz inicial
        ConfigurarLuz();

        // Apagar la linterna al inicio
        ApagarLinterna();
    }

    void Update()
    {
        // Detectar presión del botón (solo cuando se presiona, no mientras se mantiene)
        bool botonPresionado = SimpleInput.GetButton(nombreBoton);

        if (botonPresionado && !botonPresionadoAnterior)
        {
            // Se acaba de presionar el botón
            AlternarLinterna();
        }

        botonPresionadoAnterior = botonPresionado;
    }

    void ConfigurarLuz()
    {
        if (luzLinterna == null) return;

        // Configurar propiedades de la luz
        luzLinterna.intensity = intensidadEncendida;
        luzLinterna.range = rangoEncendido;
        luzLinterna.color = colorLuz;

        // Si es una Spot Light, configurar el ángulo
        if (luzLinterna.type == LightType.Spot)
        {
            luzLinterna.spotAngle = anguloSpot;
        }
    }

    public void AlternarLinterna()
    {
        if (estaEncendida)
        {
            ApagarLinterna();
        }
        else
        {
            EncenderLinterna();
        }
    }

    public void EncenderLinterna()
    {
        estaEncendida = true;

        if (luzLinterna != null)
            luzLinterna.enabled = true;

        // Reproducir sonido de encender
        if (audioSource != null && sonidoEncender != null)
            audioSource.PlayOneShot(sonidoEncender);

        Debug.Log("Linterna encendida");
    }

    public void ApagarLinterna()
    {
        estaEncendida = false;

        if (luzLinterna != null)
            luzLinterna.enabled = false;

        // Reproducir sonido de apagar
        if (audioSource != null && sonidoApagar != null)
            audioSource.PlayOneShot(sonidoApagar);

        Debug.Log("Linterna apagada");
    }

    // Método público para llamar desde otros scripts
    public bool EstaEncendida()
    {
        return estaEncendida;
    }
}