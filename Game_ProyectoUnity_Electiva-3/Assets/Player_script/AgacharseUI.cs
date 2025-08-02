using UnityEngine;
using UnityEngine.UI;

public class AgacharseUI : MonoBehaviour
{
    [Header("Referencias")]
    public Crouch controladorAgacharse; // Referencia al controlador de agacharse
    public RawImage imagenBoton; // La imagen del botón

    [Header("Estados Visuales del Botón")]
    public Color colorNormal = Color.white;
    public Color colorAgachado = Color.red;

    [Header("Efectos Adicionales")]
    public bool usarEscala = true;
    public float escalaNormal = 1.0f;
    public float escalaAgachado = 0.9f; // Más pequeño cuando está agachado

    [Header("Efectos de Transparencia")]
    public bool usarTransparencia = true;
    public float alfaNormal = 0.7f;
    public float alfaAgachado = 1.0f;

    [Header("Efectos de Pulso (Opcional)")]
    public bool usarEfectoPulso = true;
    public float velocidadPulso = 3f; // Velocidad del efecto de pulso cuando está agachado

    private Vector3 escalaOriginal;
    private Color colorOriginal;
    private bool estadoAnterior = false;

    void Start()
    {
        // Buscar referencias automáticamente si no están asignadas
        if (controladorAgacharse == null)
            controladorAgacharse = FindFirstObjectByType<Crouch>();

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
        if (controladorAgacharse == null) return;

        // Verificar si cambió el estado de agacharse
        bool estadoActual = controladorAgacharse.activo;
        if (estadoActual != estadoAnterior)
        {
            ActualizarEstadoVisual();
            estadoAnterior = estadoActual;
        }

        // Aplicar efecto de pulso si está agachado
        if (usarEfectoPulso && controladorAgacharse.activo)
        {
            AplicarEfectoPulso();
        }
    }

    void ActualizarEstadoVisual()
    {
        if (controladorAgacharse == null || imagenBoton == null) return;

        bool estaAgachado = controladorAgacharse.activo;

        // Cambiar color
        Color nuevoColor = estaAgachado ? colorAgachado : colorNormal;

        // Aplicar transparencia
        if (usarTransparencia)
        {
            nuevoColor.a = estaAgachado ? alfaAgachado : alfaNormal;
        }

        imagenBoton.color = nuevoColor;

        // Cambiar escala
        if (usarEscala)
        {
            float escala = estaAgachado ? escalaAgachado : escalaNormal;
            transform.localScale = escalaOriginal * escala;
        }

        // Vibración en móvil cuando cambia de estado
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    void AplicarEfectoPulso()
    {
        if (imagenBoton == null) return;

        // Crear efecto de pulso cuando está agachado
        float pulso = Mathf.Sin(Time.time * velocidadPulso) * 0.15f + 0.85f; // Oscila entre 0.7 y 1.0
        Color colorConPulso = colorAgachado;
        colorConPulso.a = pulso * (usarTransparencia ? alfaAgachado : 1f);
        imagenBoton.color = colorConPulso;

        // También aplicar pulso a la escala si está activado
        if (usarEscala)
        {
            float escalaPulso = Mathf.Sin(Time.time * velocidadPulso) * 0.05f + escalaAgachado;
            transform.localScale = escalaOriginal * escalaPulso;
        }
    }

    // Método para llamar desde el botón UI si prefieres usar OnClick
    public void AlternarAgacharse()
    {
        if (controladorAgacharse != null)
            controladorAgacharse.CambiarEstadoAgacharse();
    }
}