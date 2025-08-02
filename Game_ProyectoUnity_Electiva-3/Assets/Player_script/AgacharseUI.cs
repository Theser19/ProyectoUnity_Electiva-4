using UnityEngine;
using UnityEngine.UI;

public class AgacharseUI : MonoBehaviour
{
    [Header("Referencias")]
    public Crouch controladorAgacharse; // Referencia al controlador de agacharse
    public RawImage imagenBoton; // La imagen del bot�n

    [Header("Estados Visuales del Bot�n")]
    public Color colorNormal = Color.white;
    public Color colorAgachado = Color.red;

    [Header("Efectos Adicionales")]
    public bool usarEscala = true;
    public float escalaNormal = 1.0f;
    public float escalaAgachado = 0.9f; // M�s peque�o cuando est� agachado

    [Header("Efectos de Transparencia")]
    public bool usarTransparencia = true;
    public float alfaNormal = 0.7f;
    public float alfaAgachado = 1.0f;

    [Header("Efectos de Pulso (Opcional)")]
    public bool usarEfectoPulso = true;
    public float velocidadPulso = 3f; // Velocidad del efecto de pulso cuando est� agachado

    private Vector3 escalaOriginal;
    private Color colorOriginal;
    private bool estadoAnterior = false;

    void Start()
    {
        // Buscar referencias autom�ticamente si no est�n asignadas
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

        // Verificar si cambi� el estado de agacharse
        bool estadoActual = controladorAgacharse.activo;
        if (estadoActual != estadoAnterior)
        {
            ActualizarEstadoVisual();
            estadoAnterior = estadoActual;
        }

        // Aplicar efecto de pulso si est� agachado
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

        // Vibraci�n en m�vil cuando cambia de estado
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    void AplicarEfectoPulso()
    {
        if (imagenBoton == null) return;

        // Crear efecto de pulso cuando est� agachado
        float pulso = Mathf.Sin(Time.time * velocidadPulso) * 0.15f + 0.85f; // Oscila entre 0.7 y 1.0
        Color colorConPulso = colorAgachado;
        colorConPulso.a = pulso * (usarTransparencia ? alfaAgachado : 1f);
        imagenBoton.color = colorConPulso;

        // Tambi�n aplicar pulso a la escala si est� activado
        if (usarEscala)
        {
            float escalaPulso = Mathf.Sin(Time.time * velocidadPulso) * 0.05f + escalaAgachado;
            transform.localScale = escalaOriginal * escalaPulso;
        }
    }

    // M�todo para llamar desde el bot�n UI si prefieres usar OnClick
    public void AlternarAgacharse()
    {
        if (controladorAgacharse != null)
            controladorAgacharse.CambiarEstadoAgacharse();
    }
}