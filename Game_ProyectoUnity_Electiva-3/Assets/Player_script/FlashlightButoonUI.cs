using UnityEngine;
using UnityEngine.UI;

public class FlashlightButoonUI : MonoBehaviour
{
    [Header("Referencias")]
    public FlashlightController linterna; // Referencia al controlador de linterna
    public RawImage imagenBoton; // La imagen del botón

    [Header("Estados Visuales del Botón")]
    public Color colorApagado = Color.gray;
    public Color colorEncendido = Color.yellow;

    [Header("Efectos Adicionales")]
    public bool usarEscala = true;
    public float escalaApagado = 1.0f;
    public float escalaEncendido = 1.1f;

    [Header("Efectos de Transparencia")]
    public bool usarTransparencia = true;
    public float alfaApagado = 0.6f;
    public float alfaEncendido = 1.0f;

    [Header("Efectos de Brillo (Opcional)")]
    public bool usarEfectoBrillo = true;
    public float velocidadBrillo = 2f; // Velocidad del efecto de parpadeo cuando está encendida

    private Vector3 escalaOriginal;
    private Color colorOriginal;
    private bool estadoAnterior = false;

    void Start()
    {
        // Buscar referencias automáticamente si no están asignadas
        if (linterna == null)
            linterna = FindFirstObjectByType<FlashlightController>();

        if (imagenBoton == null)
            imagenBoton = GetComponent<RawImage>();

        // Guardar valores originales
        escalaOriginal = transform.localScale;
        if (imagenBoton != null)
            colorOriginal = imagenBoton.color;

        // Aplicar estado inicial
        ActualizarEstadoVisual();
    }

    void Update()
    {
        if (linterna == null) return;

        // Verificar si cambió el estado de la linterna
        bool estadoActual = linterna.EstaEncendida();

        if (estadoActual != estadoAnterior)
        {
            ActualizarEstadoVisual();
            estadoAnterior = estadoActual;
        }

        // Aplicar efecto de brillo si está encendida
        if (usarEfectoBrillo && linterna.EstaEncendida())
        {
            AplicarEfectoBrillo();
        }
    }

    void ActualizarEstadoVisual()
    {
        if (linterna == null || imagenBoton == null) return;

        bool estaEncendida = linterna.EstaEncendida();

        // Cambiar color
        Color nuevoColor = estaEncendida ? colorEncendido : colorApagado;

        // Aplicar transparencia
        if (usarTransparencia)
        {
            nuevoColor.a = estaEncendida ? alfaEncendido : alfaApagado;
        }

        imagenBoton.color = nuevoColor;

        // Cambiar escala
        if (usarEscala)
        {
            float escala = estaEncendida ? escalaEncendido : escalaApagado;
            transform.localScale = escalaOriginal * escala;
        }

        // Vibración en móvil cuando cambia de estado
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    void AplicarEfectoBrillo()
    {
        if (imagenBoton == null) return;

        // Crear efecto de pulso/brillo cuando está encendida
        float brillo = Mathf.Sin(Time.time * velocidadBrillo) * 0.2f + 0.8f; // Oscila entre 0.6 y 1.0

        Color colorConBrillo = colorEncendido;
        colorConBrillo.a = brillo * (usarTransparencia ? alfaEncendido : 1f);

        imagenBoton.color = colorConBrillo;
    }

    // Método para llamar desde el botón UI si prefieres usar OnClick
    public void AlternarLinterna()
    {
        if (linterna != null)
            linterna.AlternarLinterna();
    }
}