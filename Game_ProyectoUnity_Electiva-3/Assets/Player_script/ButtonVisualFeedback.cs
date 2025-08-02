using UnityEngine;
using UnityEngine.UI;
using SimpleInputNamespace;

public class ButtonVisualFeedback : MonoBehaviour
{
    [Header("Configuración del Botón")]
    public string nombreBoton = "Fire3"; // Nombre del botón en SimpleInput

    [Header("Estados Visuales")]
    public RawImage imagenBoton; // CAMBIADO: Ahora usa RawImage en lugar de Image
    public Color colorNormal = Color.white;
    public Color colorPresionado = Color.yellow;
    public Color colorCansado = Color.red; // Para cuando el jugador está cansado

    [Header("Efectos Adicionales")]
    public bool usarEscala = true;
    public float escalaNormal = 1.0f;
    public float escalaPresionada = 1.2f;

    [Header("Efectos de Transparencia")]
    public bool usarTransparencia = true;
    public float alfaNormal = 0.7f;
    public float alfaPresionado = 1.0f;
    public float alfaCansado = 0.4f;

    [Header("Referencias")]
    public Player_Run playerRun; // Referencia al script de correr

    private Vector3 escalaOriginal;
    private bool estaPresionado = false;

    void Start()
    {
        if (imagenBoton == null)
            imagenBoton = GetComponent<RawImage>(); // CAMBIADO: Busca RawImage

        escalaOriginal = transform.localScale;

        // Buscar automáticamente el Player_Run si no está asignado
        if (playerRun == null)
            playerRun = FindFirstObjectByType<Player_Run>();
    }

    void Update()
    {
        // Detectar si el botón está siendo presionado
        bool botonPresionado = SimpleInput.GetButton(nombreBoton);

        // Detectar si el jugador está cansado (descansando)
        bool estaCansado = playerRun != null && playerRun.tiempoCorriendo >= playerRun.maxTiempoCorrer;

        // Aplicar efectos visuales
        if (estaCansado)
        {
            AplicarEstadoCansado();
        }
        else if (botonPresionado && !estaPresionado)
        {
            AplicarEstadoPresionado();
            estaPresionado = true;
        }
        else if (!botonPresionado && estaPresionado)
        {
            AplicarEstadoNormal();
            estaPresionado = false;
        }
    }

    void AplicarEstadoNormal()
    {
        // Cambiar color
        if (imagenBoton != null)
        {
            Color nuevoColor = colorNormal;
            if (usarTransparencia)
                nuevoColor.a = alfaNormal;
            imagenBoton.color = nuevoColor;
        }

        // Cambiar escala
        if (usarEscala)
        {
            transform.localScale = escalaOriginal * escalaNormal;
        }
    }

    void AplicarEstadoPresionado()
    {
        // Cambiar color
        if (imagenBoton != null)
        {
            Color nuevoColor = colorPresionado;
            if (usarTransparencia)
                nuevoColor.a = alfaPresionado;
            imagenBoton.color = nuevoColor;
        }

        // Cambiar escala
        if (usarEscala)
        {
            transform.localScale = escalaOriginal * escalaPresionada;
        }

        // Efecto de vibración (solo en móvil)
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    void AplicarEstadoCansado()
    {
        // Cambiar color a rojo/cansado
        if (imagenBoton != null)
        {
            Color nuevoColor = colorCansado;
            if (usarTransparencia)
                nuevoColor.a = alfaCansado;
            imagenBoton.color = nuevoColor;
        }

        // Escala normal pero más pequeña para indicar que no está disponible
        if (usarEscala)
        {
            transform.localScale = escalaOriginal * 0.9f;
        }
    }
}